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
		private StringBuilder currUserControlledLettersAsStringBuilder = new StringBuilder (); //maintains this along with the letter bar so that it's easy to quickly update and get the new colours.
		public string CurrentUserControlledLettersAsString {
				get {
						return currUserControlledLettersAsStringBuilder.ToString ();
				}

		}

		private StringBuilder selectedUserControlledLettersAsStringBuilder;

		public string SelectedUserControlledLettersAsString {
				get {
						return selectedUserControlledLettersAsStringBuilder.ToString ();
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
				for (int i= 0; i<maxUserLetters; i++) 
						currUserControlledLettersAsStringBuilder.Append (" ");
						
				EMPTY_USER_WORD = currUserControlledLettersAsStringBuilder.ToString ();
				selectedUserControlledLettersAsStringBuilder = new StringBuilder (EMPTY_USER_WORD);
		
				letterGrid = letterGridControllerGO.GetComponent<LetterGridController> ();
				letterGrid.InitializeBlankLetterSpaces (maxUserLetters);
			
				AssignInteractiveLettersToTangibleCounterParts ();
				InteractiveLetter.LetterSelectedDeSelected += LetterSelectDeselect;
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
				SaveNewLetterInStringRepresentation (newLetter, atPosition, currUserControlledLettersAsStringBuilder);
				letterGrid.UpdateLetter (atPosition, newLetter + "");

		}

		public void ChangeDisplayColourOfCells (Color newColour, bool onlySelected=false, int start=-1, int count=7)
		{
				start = (start < StartingIndex ? StartingIndex : start);
				count = (count > MaxArduinoLetters ? MaxArduinoLetters : count);
				if (!onlySelected) {
						for (int i=start; i<count; i++) {
								ChangeDisplayColourOfASingleCell (i, newColour);

						}
				} else {
					
						for (int i=start; i<count; i++) {
								if (selectedUserControlledLettersAsStringBuilder [i] != ' ')
										ChangeDisplayColourOfASingleCell (i, newColour);
						}

				}
		}

		public void ChangeDisplayColourOfASingleCell (int atPosition, Color newColour)
		{
				letterGrid.UpdateLetter (atPosition, newColour, false);
		}

		public void RevertLettersToDefaultColour (bool onlySelected=false, int start=-1, int count=7)
		{
				start = (start < StartingIndex ? StartingIndex : start);
				count = (count > MaxArduinoLetters ? MaxArduinoLetters : count);
				if (!onlySelected) {
						for (int i=start; i<count; i++) {
								RevertASingleLetterToDefaultColour (i);
						}
				} else {
						for (int i=start; i<count; i++) {
								if (selectedUserControlledLettersAsStringBuilder [i] != ' ')
										RevertASingleLetterToDefaultColour (i);
				
						}

				}

		}

		public void RevertASingleLetterToDefaultColour (int atPosition)
		{
				InteractiveLetter l = letterGrid.GetLetterCell (atPosition).GetComponent<InteractiveLetter> ();
				l.RevertToDefaultColour ();
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
						return SelectedUserControlledLettersAsString;
				return CurrentUserControlledLettersAsString;
		
		}

		void SaveNewLetterInStringRepresentation (char letter, int position, StringBuilder stringRepresentation)
		{

				//replace character that was at l with new character
				stringRepresentation.Remove (position, 1);
				stringRepresentation.Insert (position, letter);

		}

		public bool IsBlank (int indexInLetterGrid)
		{
				indexInLetterGrid -= startingIndexOfUserLetters; //re-scale to the indexes of the string that represents arduino letters only.
		
				return currUserControlledLettersAsStringBuilder.ToString () [indexInLetterGrid] == ' ';
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

				return currUserControlledLettersAsStringBuilder.ToString ().Equals (EMPTY_USER_WORD);
		}
	
		UserWord GetNewColoursAndSoundsFromDecoder (LetterGridController letterGridController)
		{
			

				return LetterSoundComponentFactoryManager.Decode (GetUserControlledLettersAsString (false));
		
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
				
				InteractiveLetter i = letterGridController.UpdateLetter (indexOfLetterBarCell, lc.GetColour());
	

			
				bool flashInteractiveLetter = flash && i.HasLetterOrSoundChanged (lc) && lc.GetColour() == i.CurrentColor ();
				
				i.LetterSoundComponentIsPartOf = lc;
		
				if (flashInteractiveLetter) {
						i.StartCoroutine ("Flash");
			
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
				if (currUserControlledLettersAsStringBuilder.ToString ().Equals (EMPTY_USER_WORD))
						return 0;
				return 1;
		
		}

		bool selectButtonOnSelection = false;
	
		public void SelectDeselectAllButtonPressed ()
		{       
				selectButtonOnSelection = !selectButtonOnSelection;	 
				InteractiveLetter l;
				//select or deselect all letters
				for (int i=0, j=StartingIndex; i<currUserControlledLettersAsStringBuilder.Length; i++,j++) {
						SaveNewLetterInStringRepresentation ((selectButtonOnSelection ? currUserControlledLettersAsStringBuilder [i] : ' '), i, selectedUserControlledLettersAsStringBuilder);
						l = letterGrid.GetInteractiveLetter (j);
						if (selectButtonOnSelection)
								l.Select (false);
						else
								l.DeSelect (false);
				}

				//pass along to user inpur router
				userInputRouter.HandleLetterSelection (SelectedUserControlledLettersAsString.Trim ()); //pretend that all letters are selected when we press the button
		
		}

		public void LetterSelectDeselect (bool wasSelected, GameObject selectedLetter)
		{       
				char letter = selectedLetter.GetComponent<InteractiveLetter> ().Letter () [0];
				if (selectedLetter.name.Length == 1) {
						int position = Int32.Parse (selectedLetter.name);
						if (wasSelected) {
								SaveNewLetterInStringRepresentation (letter, position, selectedUserControlledLettersAsStringBuilder);


						} else {

								SaveNewLetterInStringRepresentation (' ', position, selectedUserControlledLettersAsStringBuilder);
						}
						
						userInputRouter.HandleLetterSelection (SelectedUserControlledLettersAsString);    
		
				}
		}



	

	
		//all of this is for testing; simulates arduino functionality.
		static int testPosition = -1;

		public void SetTestPosition (int newPosition)
		{
				testPosition = newPosition;
			
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
			
				testPosition = -1;

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


}
