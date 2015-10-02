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
		ArduinoUnityInterface tangibleLetters;
		public GameObject letterGridControllerGO;

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

		public void Initialize (int startingIndexOfArduinoLetters, int endingIndexOfArduinoLetters, ArduinoUnityInterface tangibleLetters)
		{
				StartingIndex = startingIndexOfArduinoLetters;
				EndingIndex = endingIndexOfArduinoLetters;
				maxUserLetters = EndingIndex + 1 - StartingIndex;
				for (int i= 0; i<maxUserLetters; i++) {
						currUserControlledLettersAsString.Append (" ");
						EMPTY_USER_WORD = currUserControlledLettersAsString.ToString ();
			
				}
				letterGrid = letterGridControllerGO.GetComponent<LetterGridController> ();
				letterGrid.InitializeBlankLetterSpaces (maxUserLetters, gameObject);
			

				AssignInteractiveLettersToTangibleCounterParts ();
		}



	
		//invoked by the arduino and the keyboard on screen
		public void ReceiveNewUserInputLetter (char newLetter, int atPosition)
		{
				StudentsDataHandler.instance.LogEvent ("change_letter", newLetter + "", atPosition + "");

		
				if (atPosition < maxUserLetters && atPosition >= StartingIndex) {
						if (IsUpper (newLetter))
								newLetter = ToLower (newLetter);
						//...automatically remember the change but don't necessarily update the colours.
						ChangeTheLetterOfASingleCell (atPosition, newLetter);
						userInputRouter.HandleNewUserInputLetter (newLetter,
			                                          atPosition, this);
				}
		}



		//unlocks the letter at position atPosition.
		//the effect of unlocking a letter is to change its colour from the locked colour to whatever the colour of that letter should be
		//given then other letters in the word
		public void UnLockASingleLetter (int atPosition)
		{
				GameObject nthArduinoControlledLetter = letterGrid.GetLetterCell (atPosition);
				nthArduinoControlledLetter.GetComponent<InteractiveLetter> ().UnLock ();

				
		}


		//locks the letter at position atPosition.
		//the effect of locking a letter is to make that letter appear black
		//instead of appearing in its usual colour.
		//letters that are locked will have their default colours updated
		//but this class will not tell the letters to instantly assume their new colour
		public void LockASingleLetter (int atPosition)
		{

				GameObject nthArduinoControlledLetter = letterGrid.GetLetterCell (atPosition);
				nthArduinoControlledLetter.GetComponent<InteractiveLetter> ().Lock ();


		}

		public void LockAllLetters ()
		{
				List<InteractiveLetter> letters = letterGrid.GetLetters (false);
				foreach (InteractiveLetter il in letters) {
						il.Lock ();

				}

		}

		public void UnLockAllLetters ()
		{
				List<InteractiveLetter> letters = letterGrid.GetLetters (false);
				foreach (InteractiveLetter il in letters) {
						il.UnLock ();
			
				}

		}

		public void ChangeTheLetterOfASingleCell (int atPosition, char newLetter)
		{
				SaveNewLetterInStringRepresentation (newLetter, atPosition);
				letterGrid.UpdateLetter (gameObject, atPosition, newLetter + "");

		}

		public void PlaceWordInLetterGrid (string word)
		{

				for (int i=0, j=startingIndexOfUserLetters; i<word.Length; i++,j++) {
						ChangeTheLetterOfASingleCell (j, word.Substring (i, 1) [0]);
					
				}
		
				 
		}

		public void ReplaceEachLetterWithBlank ()
		{

				PlaceWordInLetterGrid (EMPTY_USER_WORD);
		}

		public UserWord UpdateDefaultColoursAndSoundsOfLetters (bool flash)
		{
		
				UserWord newLetterSoundComponents = GetNewColoursAndSoundsFromDecoder (letterGrid);
		
				AssignNewColoursAndSoundsToLetters (newLetterSoundComponents, letterGrid, flash);
				return newLetterSoundComponents;
		
		}

		public List<InteractiveLetter> GetAllUserInputLetters (bool skipBlanks)
		{
		
				return letterGrid.GetLetters (skipBlanks);
		
		}

		public string GetUserControlledLettersAsString (bool onlySelected)
		{
				if (onlySelected)
						return AllSelectedLettersAsString ();
				return CurrentUserControlledLettersAsString;
		
		}

		void SaveNewLetterInStringRepresentation (char letter, int position)
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

		public bool IsBlank (int indexInLetterGrid)
		{
				indexInLetterGrid -= startingIndexOfUserLetters; //re-scale to the indexes of the string that represents arduino letters only.
		
				return currUserControlledLettersAsString.ToString () [indexInLetterGrid] == ' ';
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

		public bool NoUserControlledLetters ()
		{

				return currUserControlledLettersAsString.ToString ().Equals (EMPTY_USER_WORD);
		}
	
		UserWord GetNewColoursAndSoundsFromDecoder (LetterGridController letterGridController)
		{
			

				return LetterSoundComponentFactoryManager.Decode (GetUserControlledLettersAsString(false));
		
		}

		bool logSelectionDeselection;
		string selectedDeselected = "?";
		//creates a string consisting of all selected letters. 
		//non selected letters are treated as blanks.
		public string AllSelectedLettersAsString ()
		{
				StringBuilder st = new StringBuilder ();
				for (int j=startingIndexOfUserLetters; j<=endingIndexOfUserLetters; j++) {

						InteractiveLetter i = letterGrid.GetLetterCell (j).GetComponent<InteractiveLetter> ();
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
     
						letterCell.GetComponent<InteractiveLetter> ().IdxAsArduinoControlledLetter = ConvertScreenToArduinoIndex (indexOfLetterBarCell);//plus 1 because the indexes are shifted.
				}
		}

		int ConvertScreenToArduinoIndex (int screenIndex)
		{ //arduino starts counting at 1
				return screenIndex + 1;
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
		       
				PlaySoundsOfSelectedLetters ();
		      
			
		}

		public void PlaySoundsOfSelectedLetters ()
		{
				//retrieve sounds from the 
				StringBuilder selectedLetters = new StringBuilder ();
				List<InteractiveLetter> selectableLetters = letterGrid.GetLetters (false);
				foreach (InteractiveLetter l in selectableLetters) {
						Selectable s = l.gameObject.GetComponent<Selectable> ();
						//we are going to distinguish between prefixes and affixes
						if (s.Selected) {
								//s.Deselect (false);
				
								selectedLetters.Append (l.Letter ().Trim ());
						} else
								selectedLetters.Append (" ");
				}
		
				string subword = selectedLetters.ToString ().Trim ();
				AudioClip syll = AudioSourceController.GetSyllableFromResources (subword);
				AudioSourceController.PushClip (syll);
		
				StudentsDataHandler.instance.LogEvent ("played_letter_sounds", subword, "NA");
		
		}

		public void ObjectSelected (GameObject selectedLetter)
		{
				//SetupVariablesToLogSelectionDeselectionUponChangingColours (selectedLetter);
				UpdateDefaultColoursAndSoundsOfLetters (true);
		      
		
		}
	
		public void ObjectDeselected (GameObject selectedLetter)
		{      
				//SetupVariablesToLogSelectionDeselectionUponChangingColours (selectedLetter);
				UpdateDefaultColoursAndSoundsOfLetters (true);

				//TO DO when working out selection stuff, only the arduino letter controller will handler it
				//if (SessionsDirector.DelegateControlToStudentActivityController) {
				//		userInputRouter.studentActivityController.ObjectDeselected (selectedLetter, true);
				//}
		
		}

		/*
		void SetupVariablesToLogSelectionDeselectionUponChangingColours (GameObject selectedLetter)
		{
				logSelectionDeselection = true;
				selectedDeselected = selectedLetter.GetComponent<InteractiveLetter> ().Letter ();


		}*/




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
						
						ReceiveNewUserInputLetter (testLetter [0], testPosition);
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
