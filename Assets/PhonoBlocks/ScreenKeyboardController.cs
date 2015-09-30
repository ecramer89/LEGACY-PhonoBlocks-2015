using UnityEngine;

using System;

/*
 * class that manages the on screen key board.
 * listens for presses to the button (which triggers the keyboard to appear)
 * initializes the keyboard with images of the interactive letters
 * responds to touches to the interactive letters
 * when an interactive letter is touched it tells the ardunio letter controller (who treats it like a standard keyboard issued letter)
 * */
public class ScreenKeyboardController : MonoBehaviour
{


		public GameObject onScreenKeyboardGO; //GO parent of the grid that has the letter images and the grid that has the highlights


		LetterImageTable letterImageTable;
		public GameObject lettersOfKeyboardGO;
		LetterGridController lettersOfKeyboard;
		public GameObject arduinoLetterControllerGO;
		ArduinoLetterController arduinoLetterController;
		public GameObject activateKeyBoardSwipeZoneGO;
		public GameObject selectedKey;
		string[] keys = new string[] {
		"a",
		"b",
		"c",
		"d",
		"e",
		"f",
		"g",
		"h",
		"i",
		"j",
		"k",
		"l",
		"m",
		"n",
		"o",
		"p",
		"q",
		"r",
		"s",
		"t",
		"u",
		"v",
		"w",
		"x",
		"y",
		"z",
		" " //backspace
	};

		public void Initialize ()
		{
				activateKeyBoardSwipeZoneGO.SetActive (true);
				Selectable s = activateKeyBoardSwipeZoneGO.GetComponent<Selectable> ();
				s.ActivateBySwipe ();
				s.SelectBySwipe ();
				s.Handler = gameObject;
				arduinoLetterController = arduinoLetterControllerGO.GetComponent<ArduinoLetterController> ();
				letterImageTable = GameObject.Find ("DataTables").GetComponent<LetterImageTable> ();
				InitializeKeyBoard ();

		}

		void InitializeKeyBoard ()
		{

				lettersOfKeyboard = lettersOfKeyboardGO.GetComponent<LetterGridController> ();
				int position = 0;
				foreach (string key in keys) {
						GameObject interactiveKey = lettersOfKeyboard.CreateLetterBarCell (key, letterImageTable.GetLetterImageFromLetter (key), (position++) + "", Color.white, gameObject);

						interactiveKey.AddComponent<BoxCollider> ();
				
						Selectable s = interactiveKey.GetComponent<Selectable> ();
						s.SelectByTouch ();
						s.Select (false);
						s.ForceActivate = true;
		

				
				}
				lettersOfKeyboard.RepositionGrids ();
				DeactivateKeyboard ();
				
		}

		void ActivateKeyboard ()
		{

				onScreenKeyboardGO.SetActive (true);
		}

		void DeactivateKeyboard ()
		{
		
				onScreenKeyboardGO.SetActive (false);
		}

		public void ObjectSelected (GameObject o)
		{
				if (ReferenceEquals (o, activateKeyBoardSwipeZoneGO)) {
						ActivateKeyboard ();
				}
		}
	
		public void ObjectDeselected (GameObject o)
		{
				if (ReferenceEquals (o, activateKeyBoardSwipeZoneGO)) {
						DeactivateKeyboard ();
				}
		}

		public void LetterClicked (GameObject clickedLetterCell)
		{

				lettersOfKeyboard.ToggleHighlightAt (Int32.Parse (clickedLetterCell.name));

				if (ReferenceEquals (selectedKey, clickedLetterCell)) {
						selectedKey = null;
						arduinoLetterController.ClearTestLetter ();

				} else {
						if (selectedKey)
								lettersOfKeyboard.ToggleHighlightAt (Int32.Parse (selectedKey.name));
						selectedKey = clickedLetterCell;
						string key = clickedLetterCell.GetComponent<InteractiveLetter> ().Letter ();
						arduinoLetterController.SetTestLetter (key);

				}

		}
	
	
	
	
	
	
	
	
		
		
		

	





}
