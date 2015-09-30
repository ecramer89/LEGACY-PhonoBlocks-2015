using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections;

public class ArduinoLetterController : PhonoBlocksController
{

		public String EMPTY_USER_WORD;
		List<InteractiveLetter> lettersToFlash = new List<InteractiveLetter> ();
		private StringBuilder currUserControlledLettersAsString = new StringBuilder (); //maintains this along with the letter bar so that it's easy to quickly update and get the new colours.
		public string CurrentUserControlledLettersAsString {
				get {
						return currUserControlledLettersAsString.ToString ();
				}

		}

		public static int startingIndexOfUserLetters;
		public static int endingIndexOfUserLetters;
		LetterGridController letterGrid;
	
		public int StartingIndex {
				get {
						return startingIndexOfUserLetters;
				}
				set {
						startingIndexOfUserLetters = value;


				}
		}

		public int EndingIndex {
				get {
						return endingIndexOfUserLetters;
				}
				set {
						endingIndexOfUserLetters = value;
			
				}
		}

		private int maxUserLetters;

		public int MaxArduinoLetters {
				get {
						return maxUserLetters;
				}


				set {
						maxUserLetters = value;

				}

		}

		public void Initialize (LetterGridController lc)
		{
				
				for (int i= 0; i<maxUserLetters; i++) {
						currUserControlledLettersAsString.Append (" ");
						EMPTY_USER_WORD = currUserControlledLettersAsString.ToString ();
			
				}
				letterGrid = lc;

				AssignInteractiveLettersToTangibleCounterParts ();
		}

		public bool IsBlank (int indexInLetterGrid)
		{
				indexInLetterGrid -= startingIndexOfUserLetters; //re-scale to the indexes of the string that represents arduino letters only.
			
				return currUserControlledLettersAsString.ToString () [indexInLetterGrid] == ' ';
		}

		public void OverwriteLettersWith (string word, LetterGridController letterGridController, GameObject handler)
		{
        
				for (int i=0, j=startingIndexOfUserLetters; i<word.Length; i++,j++) {
						letterGridController.UpdateLetter (handler, j, word.Substring (i, 1));
						UpdateStringReperesentationOfUserLetters (i, word [i]);
				}

				UpdateColorsAndSoundsOfLetters (letterGridController, false); //do not flash.
		}
	
	
		//invoked by the arduino and the keyboard on screen
		public void UserChangedALetter (char newLetter, int atPosition)
		{
				StudentsDataHandler.instance.LogEvent ("change_letter", newLetter + "", atPosition + "");

      

				if (atPosition < maxUserLetters && atPosition >= StartingIndex) {
						if (IsUpper (newLetter))
								newLetter = ToLower (newLetter);
						//atPosition = TranslateRawPositionToPositionOfLetterInUILetterBar (atPosition);//re-scale.
						userInputRouter.RequestAddOrRemoveArduinoLetter (newLetter,
				atPosition, this);
				}
		}

		public void SaveNewLetterInStringRepresentation (char newLetter, int position)
		{
				//UpdateStringReperesentationOfUserLetters (TranslatePositionOfLetterInUILetterBarToRaw (position), newLetter);
        UpdateStringReperesentationOfUserLetters(position, newLetter);


    }

		bool IsUpper (char letter)
		{
				int asInt = (int)letter;
				return asInt > 64 && asInt < 91;


		}

		//97-122 lower case; 65-> upper case
		char ToLower (char newLetter)
		{
				return (char)((int)newLetter + 32);


		}


		//this won't be needed when we switch to just receiving inputs from the arduino... 
		//well, except considering the affix thing.
		void UpdateStringReperesentationOfUserLetters (int position, char letter)
		{
				if (position < 0)
						return;
				if (position >= currUserControlledLettersAsString.Length)
						currUserControlledLettersAsString.Append (letter);
				else {

						currUserControlledLettersAsString.Remove (position, 1);
						currUserControlledLettersAsString.Insert (position, letter);
				}
		
		}

		public bool NoUserLetters ()
		{

				return currUserControlledLettersAsString.ToString ().Equals (EMPTY_USER_WORD);
		}

		public UserWord UpdateColorsAndSoundsOfLetters (LetterGridController letterGridController, bool flash)
		{
				
				UserWord newLetterSoundComponents = GetNewColoursAndSoundsFromDecoder (letterGridController);
	
				AssignNewColoursAndSoundsToLetters (newLetterSoundComponents, letterGridController, flash);
				return newLetterSoundComponents;
		
		}
	
		UserWord GetNewColoursAndSoundsFromDecoder (LetterGridController letterGridController)
		{
				string currUserControlledLetters = AllSelectedLettersAsString (letterGridController);


				return LetterSoundComponentFactoryManager.Decode (currUserControlledLetters);
		
		}

		bool logSelectionDeselection;
		string selectedDeselected = "?";
		//creates a string consisting of all selected letters. 
		//non selected letters are treated as blanks.
		public string AllSelectedLettersAsString (LetterGridController letterGridController)
		{
				StringBuilder st = new StringBuilder ();
				for (int j=startingIndexOfUserLetters; j<=endingIndexOfUserLetters; j++) {

						InteractiveLetter i = letterGridController.GetLetterCell (j).GetComponent<InteractiveLetter> ();
						Selectable s = i.gameObject.GetComponent<Selectable> ();
						if (s.Selected)
								st.Append (i.Letter ());
						else
								st.Append (" ");

				}

				string result = st.ToString ();
				if (logSelectionDeselection) {
						StudentsDataHandler.instance.LogEvent ("selection_deselection", result, selectedDeselected);
						logSelectionDeselection = false;

				}
				return result;



		}

		void AssignInteractiveLettersToTangibleCounterParts ()
		{
				int indexOfLetterBarCell = startingIndexOfUserLetters;
				for (; indexOfLetterBarCell<=endingIndexOfUserLetters; indexOfLetterBarCell++) {
						GameObject letterCell = letterGrid.GetLetterCell (indexOfLetterBarCell);
     
						letterCell.GetComponent<InteractiveLetter> ().IdxAsArduinoControlledLetter = indexOfLetterBarCell;
				}
		}

		void AssignNewColoursAndSoundsToLetters (UserWord letterSoundComponents, LetterGridController letterGridController, bool flash)
		{   
				
				int indexOfLetterBarCell = startingIndexOfUserLetters;

				foreach (LetterSoundComponent p in letterSoundComponents) {
            //ending index == total number of letters minus one.
						if (indexOfLetterBarCell <= endingIndexOfUserLetters) { //no longer required because I fixed the bug in the LCFactoryManager that caused the error, but I'm leaving this here for redundant error protection...


								if (p is LetterSoundComposite) {
										LetterSoundComposite l = (LetterSoundComposite)p;
										foreach (LetterSoundComponent lc in l.Children) {

												
												UpdateInterfaceLetters (lc, letterGridController, indexOfLetterBarCell, flash);
												indexOfLetterBarCell++;
										}
								} else {
									
										UpdateInterfaceLetters (p, letterGridController, indexOfLetterBarCell, flash);
							
										indexOfLetterBarCell++;
								}

						}
				}
		}

		void UpdateInterfaceLetters (LetterSoundComponent lc, LetterGridController letterGridController, int indexOfLetterBarCell, bool flash)
		{
				
				InteractiveLetter i = letterGridController.UpdateLetter (indexOfLetterBarCell, lc.Color);

				Selectable s = i.gameObject.GetComponent<Selectable> ();
				if (s.Selected) {
						bool flashInteractiveLetter = flash && i.HasLetterOrSoundChanged (lc) && lc.Color == i.CurrentColor ();
		
						i.LetterSoundComponentIsPartOf = lc;
		
						if (flashInteractiveLetter) {
								i.StartCoroutine ("Flash");
			
						}

				}
		}

		int FindIndexOfGraphemeThatCorrespondsToLastNonBlankPhonogram (UserWord userWord)
		{
				int cursor = endingIndexOfUserLetters; 
				foreach (LetterSoundComponent p in userWord) {
						if (p is Blank)
								cursor -= p.Length;
						else
								return cursor;
				}
				return cursor;
		}

		int IsLetterBarEmpty ()
		{
				if (currUserControlledLettersAsString.ToString ().Equals (EMPTY_USER_WORD))
						return 0;
				return 1;
		
		}
 
		public void SelectedObjectActivated (GameObject selectedAndActivatedLetterCell)
		{
		       
				userInputRouter.RequestPlaySoundsOfSelectedArduinoLetters ();
		      
			
		}

		public void ObjectSelected (GameObject selectedLetter)
		{
				SetupVariablesToLogSelectionDeselectionUponChangingColours (selectedLetter);
				UpdateColorsAndSoundsOfLetters (letterGrid, true);
		      
		
		}
	
		public void ObjectDeselected (GameObject selectedLetter)
		{      
				SetupVariablesToLogSelectionDeselectionUponChangingColours (selectedLetter);
				UpdateColorsAndSoundsOfLetters (letterGrid, true);
				if (SessionsDirector.DelegateControlToStudentActivityController) {
						userInputRouter.studentActivityController.ObjectDeselected (selectedLetter, true);
				}
		
		}

		void SetupVariablesToLogSelectionDeselectionUponChangingColours (GameObject selectedLetter)
		{
				logSelectionDeselection = true;
				selectedDeselected = selectedLetter.GetComponent<InteractiveLetter> ().Letter ();


		}




		//all of this is for testing; simulates arduino functionality.
		static int testPosition = -1;

		public void SetTestPosition (int newPosition)
		{
				testPosition = newPosition;
				letterGrid.SetHighlightAt (testPosition, true);
				UpdateLetterBarIfPositionAndLetterSpecified ();
		}
	    
		static String testLetter;

		public void SetTestLetter (String newLetter)
		{
				testLetter = newLetter;
				UpdateLetterBarIfPositionAndLetterSpecified ();
				
		}

		public void ClearTestLetter ()
		{
				testLetter = null;
				

		}

		public void ClearTestPosition ()
		{
				
				letterGrid.SetHighlightAt (testPosition, false);
				testPosition = -1;

		}
	
		public void LetterClicked (GameObject cell)
		{
	

				int position = Int32.Parse (cell.name);
				if (position == testPosition) {
						ClearTestPosition ();
				} else {
						letterGrid.SetHighlightAt (testPosition, false); //turn off highlight at previous selection
						SetTestPosition (position);
				}
				

		}

		void Update ()
		{
				if (Input.anyKeyDown) {
						if (Input.GetMouseButton (0) || Input.GetMouseButton (1) || Input.GetMouseButton (2))
								return;

						if (ParseNumericKey () || ParseLetterKey ())
								UpdateLetterBarIfPositionAndLetterSpecified ();




				}



		}

		bool ParseNumericKey ()
		{
				String s;
	
				for (int i=0; i<maxUserLetters; i++) {
						s = "" + i;
						if (Input.GetKey (s)) {
								//testPosition = i;
								SetTestPosition (i);
								return true;
						}
				}
				return false;
		}

		bool ParseLetterKey ()
		{       

				//deleting a character
				if (Input.GetKeyDown (KeyCode.Backspace)) {
						//testLetter = " ";
						SetTestLetter (" ");
						return true;
				}

				foreach (char c in SpeechSoundReference.Vowels()) {
						string s = c + "";
						if (Input.GetKeyDown (s)) {
								SetTestLetter (s);
								return true;
						}
				}

				foreach (char c in SpeechSoundReference.Consonants()) {
						string s = c + "";
						if (Input.GetKeyDown (s)) {
								//testLetter = s;
								SetTestLetter (s);
								return true;
						}
						


				}
				return false;

		}

		void UpdateLetterBarIfPositionAndLetterSpecified ()
		{
				if (testPosition != -1 && testLetter != null) {
						
						UserChangedALetter (testLetter [0], testPosition);
						ClearTestPosition ();
						
				}
		}
		/*
	
		public void TestAddingLetters (int position, char letter)
		{
				//Debug.Log ("test adding letters called");
				UserChangedALetter (letter, position);
				//....send a message (notify the controller of the letter and affix drop zones).
				//send in the appropriate data:
				//what is the new letter nb, it can be blank) and where was it?
		
		}*/



	  









	
}
