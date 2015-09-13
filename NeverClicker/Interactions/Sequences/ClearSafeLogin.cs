﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeverClicker.Interactions {
	public static partial class Sequences {
		public static void ClearSafeLogin(Interactor intr) {
			Mouse.ClickImage(intr, "CharSelectSafeLoginButton");
		}
	}
}