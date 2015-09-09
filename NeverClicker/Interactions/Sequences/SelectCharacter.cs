﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeverClicker.Interactions {
	public static partial class Sequences {

		public static bool SelectCharacter(Interactor intr, uint charZeroIdx) {
			if (intr.CancelSource.IsCancellationRequested) { return false; }

			intr.Log("Selecting character " + charZeroIdx.ToString() + " ...", LogEntryType.Info);

			// OLD
			//intr.ExecuteStatement("SelectCharacter(" + (charZeroIdx + 1).ToString() + ", 1, 0)");
			//intr.Log("Selecting character " + charZeroIdx);

			//string ClickX = "";
			//try {
			//	ClickX = intr.GameClient.GetSetting("CharSlotX", "ClickLocations");
			//} catch (Exception ex) {
			//	intr.Log("Error retreiving setting from GameClient.ini: [ClickLocations]CharSlotX -- ex: " + ex.ToString(), LogEntryType.Fatal);
			//}

			int maxChars = intr.GameAccount.GetSettingOrZero("CharZeroIdxCount", "NwAct");

			int charSlotX = intr.GameClient.GetSettingOrZero("CharSlotX", "ClickLocations");
			int topSlotY = intr.GameClient.GetSettingOrZero("TopSlotY", "ClickLocations");
			int clickableSlots = intr.GameClient.GetSettingOrZero("VisibleCharacterSelectSlots", "KeyBindAndUi");
			int scrollBarTopX = intr.GameClient.GetSettingOrZero("CharacterSelectScrollBarTopX", "KeyBindAndUi");
			int scrollBarTopY = intr.GameClient.GetSettingOrZero("CharacterSelectScrollBarTopY", "KeyBindAndUi");

			if ((maxChars == 0) || (charSlotX == 0) || (topSlotY == 0) || (clickableSlots == 0)
						|| (scrollBarTopX == 0) || (scrollBarTopY == 0)) {
				intr.Log("SelectCharacter(): Error loading ini file settings", LogEntryType.Fatal);
				return false;
			}

			//BotSlotY := TopSlotY + 45 + (70 * (clickable_slots - 1))
			int botSlotY = topSlotY + 45 + (70 * (clickableSlots - 1));

			bool mustScroll = false;

			if (charZeroIdx >= clickableSlots) {
				mustScroll = true;
			}

			int scrollChunks = 0;
			int slotPosition = 0;
			int scrollWheelPresses = 0;
			int clickY = 0;
			const int tileSize = 70;

			if (mustScroll) {
				//scrollChunks = Math.Floor(((float)charZeroIdx - ((float)clickableSlots + 1)) / 2);
				scrollChunks = ((int)charZeroIdx - clickableSlots) / 2;

				//Position:= Mod(n_invoke, 2)
				// 0 for 2nd to last, 1 for last
				slotPosition = (int)charZeroIdx % 2;

				//scroll_count:= (7 * ScrollChunks) + 5
				scrollWheelPresses = (7 * scrollChunks) + 5;

				//ClickY:= (BotSlotY - (70 * Position))
				if (((maxChars - charZeroIdx) <= 1) && ((maxChars % 2) == 1)) {
					clickY = botSlotY;
				} else {
					clickY = (botSlotY - tileSize) + (tileSize * slotPosition);
				}
			} else {
				//ClickY:= TopSlotY + (70 * (n_invoke - 1))
				clickY = topSlotY + (tileSize * ((int)charZeroIdx));
			}

			//Sleep 150
           // intr.Wait(150);
			
			//Loop 9 {
			//	SendEvent { Click % CharacterSelectScrollBarTopX %, % CharacterSelectScrollBarTopY %, 1}
			//	Sleep 100
			//}
			for (int i = 0; i < 9; i++) {
				//Keyboard.SendEvent(intr, "{ Click " + scrollBarTopX + ", " + scrollBarTopY + ", 1 }");
				Mouse.Click(intr, scrollBarTopX, scrollBarTopY);
				intr.Wait(10);
				//intr.Wait(100);
			}
			
			//Loop 20 {
			//	SendEvent { Click WheelUp}
			//	Sleep 100
			//}
			Mouse.WheelUp(intr, 5);


			//	SendEvent { Click % ClickX %, % ClickY %, 0}
			//	Sleep 170
			Mouse.Move(intr, charSlotX, clickY);
			intr.Wait(100);


			//	if (must_scroll) {
			//		Loop % scroll_count % {
			//			SendEvent { Click WheelDown}
			//			Sleep 25
			//          }
			//	}
			if (mustScroll) {
				Mouse.WheelDown(intr, scrollWheelPresses);
			}


			Mouse.DoubleClick(intr, charSlotX, clickY);




			//	; ---LOG IN-- -
			//    if (skip_char <> 1) {
			//		; SendEvent { Click % EwButtonX %, % EwButtonY %, 1}

			//		SendEvent { Click % ClickX %, % ClickY %, 1}
			//		Sleep 80
			//        SendEvent { Click % ClickX %, % ClickY %, 1}
			//		Sleep 370

			//		Send { Enter}
			//		Sleep AfterCharSelectDelay +Ran(120)

			//	} else {

			//		Sleep 50
			//        SendEvent { Click % ClickX %, % ClickY %, 1}
			//		Sleep 170
			//        return 0
			//    }



			//intr.ExecuteStatement("ClearSafeLogin()");			
			//intr.ExecuteStatement("ClearOkPopupBullshit()");
			ClearSafeLogin(intr);
			ClearOkButtons(intr);

			intr.Wait(3000);

			if (!intr.WaitUntil(90, ClientState.InWorld, Game.IsClientState, CharSelectFailure)) {
				ProduceClientState(intr, ClientState.CharSelect);
				SelectCharacter(intr, charZeroIdx);
			}

			return true;
		}


		public static bool CharSelectFailure<TState>(Interactor intr, TState state) {
			intr.Log("Failure to select character. Retrying...");
			return false;
		}
	}
}


//SelectCharacter(n_invoke, scroll_top, skip_char) {
//	global

//	LogAppend("[Character:".CurrentCharacter. ", skip_char:".skip_char. ", TopSlotY:".TopSlotY. "]")
//	ClickX:= CharSlotX
//    must_scroll:= 0
//    scroll_count:= 0
//   ; clickable_slots:= 8
//    clickable_slots:= VisibleCharacterSelectSlots
//    BotSlotY:= TopSlotY + 45 + (70 * (clickable_slots - 1))

//	Sleep 500 + Ran(100)

//	if (n_invoke > clickable_slots) {
//	must_scroll:= 1
//    }

//	if (must_scroll) {
//	ScrollChunks:= Floor((n_invoke - (clickable_slots + 1)) / 2)
//        Position:= Mod(n_invoke, 2)
//        scroll_count:= (7 * ScrollChunks) + 5
//        ClickY:= (BotSlotY - (70 * Position))
//    } else {
//	ClickY:= TopSlotY + (70 * (n_invoke - 1))
//    }

//	if (scroll_top) {

//		Sleep 150

//		Loop 9 {
//			SendEvent { Click % CharacterSelectScrollBarTopX %, % CharacterSelectScrollBarTopY %, 1}
//			Sleep 100
//        }

//		Loop 20 {
//			SendEvent { Click WheelUp}
//			Sleep 100
//        }

//		Sleep 40

//		; Loop 140 {
//			; SendEvent { Click WheelUp}
//		 ; Sleep 20
//	   ;
//		}
//		; sleep 200
//    }

//	LogAppend("[SelectCharacter():".CurrentCharacter. ", ClickX:".ClickX. ", ClickY:".ClickY. "]")

//	SendEvent { Click % ClickX %, % ClickY %, 0}
//	Sleep 170

//	if (must_scroll) {
//		Loop % scroll_count % {
//			SendEvent { Click WheelDown}
//			Sleep 25
//          }
//	}


//	; ---LOG IN-- -
//    if (skip_char <> 1) {
//		; SendEvent { Click % EwButtonX %, % EwButtonY %, 1}

//		SendEvent { Click % ClickX %, % ClickY %, 1}
//		Sleep 80
//        SendEvent { Click % ClickX %, % ClickY %, 1}
//		Sleep 370

//		Send { Enter}
//		Sleep AfterCharSelectDelay +Ran(120)

//	} else {

//		Sleep 50
//        SendEvent { Click % ClickX %, % ClickY %, 1}
//		Sleep 170
//        return 0
//    }

//	return 1
//}