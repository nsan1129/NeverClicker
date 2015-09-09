﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using NeverClicker.Properties;

namespace NeverClicker.Interactions {
	public static partial class Screen {
		public const string OUTPUT_VAR_X = "OutputVarX";
		public const string OUTPUT_VAR_Y = "OutputVarY";
		public const string ERROR_LEVEL = "ErrorLevel";
		//public const string OPTIONS = "*40";

		//public static Point ImageSearchAndClick(Interactor intr, string imgCode) {
		//	return new Point(0, 0);
		//}

		public static ImageSearchResult ImageSearch(Interactor intr, string imgCode) {
			//ImageSearch, ImgX, ImgY, 1, 1, 1920, 1080, *40 % image_file %
			var imageFileName = intr.GameClient.GetSetting(imgCode + "_ImageFile", "SearchRectanglesAnd_ImageFiles");
			if (string.IsNullOrWhiteSpace(imageFileName)) {
				intr.Log("Image code prefix '" + imgCode + "' not found in settings ini file.", LogEntryType.Error);
				return new ImageSearchResult() { Found = false, Point = new Point(0, 0) };
			}

			var imageFilePath = Settings.Default.ImagesFolderPath + "\\" + imageFileName;

			intr.Log(new LogMessage("ImageSearch(" + imgCode + "): Searching for image: '" + imageFilePath
				+ "' [ScreenWidth:" + intr.GetVar("A_ScreenWidth")
				+ " ScreenHeight:" + intr.GetVar("A_ScreenHeight") + "]",
				LogEntryType.Debug			
			));

			int outX = 0;
			int outY = 0;
			int errorLevel = 0;

			var imgSrcOptions = Settings.Default.ImageShadeVariation.ToString();

			intr.SetVar(OUTPUT_VAR_X, outX.ToString());
			intr.SetVar(OUTPUT_VAR_Y, outY.ToString());

			var statement = string.Format("ImageSearch, {0}, {1}, {2}, {3}, {4}, {5}, {6} {7}",
				 OUTPUT_VAR_X, OUTPUT_VAR_Y, "0", "0", "A_ScreenWidth", "A_ScreenHeight", "*" + imgSrcOptions, imageFilePath);

			//intr.Log(new LogMessage(""ImageSearch(" + imgCode + "): Executing: '" + statement + "'", LogEntryType.Detail));

			intr.Wait(20);
			intr.ExecuteStatement(statement);

			int.TryParse(intr.GetVar(OUTPUT_VAR_X), out outX);
			int.TryParse(intr.GetVar(OUTPUT_VAR_Y), out outY);
			int.TryParse(intr.GetVar(ERROR_LEVEL), out errorLevel);

			intr.Log(new LogMessage(
					"ImageSearch(" + imgCode + "): Results: "
					+ " OutputVarX:" + intr.GetVar(OUTPUT_VAR_X)
					+ " OutputVarY:" + intr.GetVar(OUTPUT_VAR_Y)
					+ " ErrorLevel:" + intr.GetVar(ERROR_LEVEL),
					LogEntryType.Debug					
			));

			//try {

			//	outX = int.Parse(intr.GetVar(OUTPUT_VAR_X));
			//	outY = int.Parse(intr.GetVar(OUTPUT_VAR_Y));
			//	errorLevel = int.Parse(intr.GetVar(ERROR_LEVEL));
			//} catch (Exception){
			//	throw new ProblemConductingImageSearchException("ImageSearch Results: "
			//		+ " OutputVarX:" + intr.GetVar(OUTPUT_VAR_X)
			//		+ " OutPutVarY:" + intr.GetVar(OUTPUT_VAR_Y)
			//		+ " ErrorLevel:" + intr.GetVar(ERROR_LEVEL)
			//	);
			//	//return new FindResult() { Found = false, At = new Point(0, 0) };
			//}

			switch (errorLevel) {
				case 0:
					intr.Log("ImageSearch(" + imgCode + "): Found.", LogEntryType.Info);
					return new ImageSearchResult() { Found = true, Point = new Point(outX, outY) };
				case 1:
					intr.Log("ImageSearch(" + imgCode + "): Not Found.", LogEntryType.Debug);
					return new ImageSearchResult() { Found = false, Point = new Point(outX, outY) };				
				case 2:
				default:
					intr.Log("ImageSearch(" + imgCode + "): Not Found.", LogEntryType.Fatal);
					return new ImageSearchResult() { Found = false, Point = new Point(outX, outY) };
					//throw new ProblemConductingImageSearchException();
			}

		}
	}

	public class ImageSearchResult {
		public Point Point;
		public bool Found;		
	}

	class ProblemConductingImageSearchException : Exception {
		public ProblemConductingImageSearchException() : base("There was a problem that prevented ImageSearch"  
			+ " from conducting the search(such as failure to open the image file or a badly formatted option)") { }
		public ProblemConductingImageSearchException(string message) : base(message) { }
		public ProblemConductingImageSearchException(string message, Exception inner) : base(message, inner) { }
	}
}


// ErrorLevel is set to 0 if the image was found in the specified region, 
// 1 if it was not found, or 2 if there was a problem that prevented the 
// command from conducting the search(such as failure to open the image file 
// or a badly formatted option).