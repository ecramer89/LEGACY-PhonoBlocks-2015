using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections;

public class ArduinoLetterController : PhonoBlocksController
{       public int TIMES_TO_FLASH_ERRORNEOUS_LETTER = 1;
	    public int TIMES_TO_FLASH_CORRECT_PORTION_OF_FINAL_GRAPHEME = 1;
		public int TIMES_TO_FLASH_ON_COMPLETE_TARGET_GRAPHEME = 3;
		public StudentActivityController studentActivityController;
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
	    string stringRepresentationOfPrevious;

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

		public void Initialize (
				int startingIndexOfArduinoLetters, 
				int endingIndexOfArduinoLetters, 
				ArduinoUnityInterface tangibleLetters)
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

		void ChangeTheImageOfASingleCell (int atPosition, Texture2D newImage)
		{
				InteractiveLetter i = letterGrid.GetInteractiveLetter (atPosition);
				i.SwitchImageTo (newImage);


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


		//updates letters and images of letter cells
		public void PlaceWordInLetterGrid (string word, bool updateUserControlled)
		{

				for (int i=0, j=startingIndexOfUserLetters; i<word.Length; i++,j++) {
						ChangeTheLetterOfASingleCell (j, word[i]);
						if(updateUserControlled){
								SaveNewLetterInStringRepresentation (word[i], i, currUserControlledLettersAsStringBuilder);
						}
					
				}
		
				 
		}

		public void activateLinesBeneathLettersOfWord (string word)
		{
		        
				letterGrid.setNumVisibleLetterLines (word.Length);

		        
		}

		//just updates the display images of the cells
		public void DisplayWordInLetterGrid (string word, bool ignoreBlanks=false)
		{
		
				for (int i=0, j=startingIndexOfUserLetters; i<word.Length; i++,j++) {
					if(!ignoreBlanks || word[i] != ' ')
						ChangeTheImageOfASingleCell (j, LetterImageTable.instance.GetLetterImageFromLetter (word.Substring (i, 1) [0]));
			
				}
		
		
		}

		public void ReplaceEachLetterWithBlank ()
		{

				PlaceWordInLetterGrid (EMPTY_USER_WORD, true);
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
				stringRepresentationOfPrevious = stringRepresentation.ToString ();
				stringRepresentation.Remove (position, 1);
				stringRepresentation.Insert (position, letter);

		}

	public bool ChangedFromPrevious(int position){
		return stringRepresentationOfPrevious!= null && CurrentUserControlledLettersAsString [position] != stringRepresentationOfPrevious [position];
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
			
				string userControlledLettersAsString = GetUserControlledLettersAsString (false);
				Debug.Log (userControlledLettersAsString);
				return LetterSoundComponentFactoryManager.Decode (userControlledLettersAsString, SessionsDirector.instance.IsSyllableDivisionActivity);
		
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
						                //the individual letters that compose a multi letter unit, for example the "b" in blend "bl"
												
												UpdateInterfaceLetters (lc, letterGridController, indexOfLetterBarCell, l);
												indexOfLetterBarCell++;
										}
								} else {
									
										UpdateInterfaceLetters (p, letterGridController, indexOfLetterBarCell);
										indexOfLetterBarCell++;
								}

						}
				}
		}

		bool IsVowel (char letter)
		{
				return letter == 'a' || letter == 'e' || letter == 'o' || letter == 'u' || letter == 'i';


		}




		bool flash = false;
		int timesToFlash=0;
		Color newDefaultColor;
		Color flashColor;
		InteractiveLetter asInteractiveLetter;

		void UpdateInterfaceLetters (
				LetterSoundComponent lc, 
				LetterGridController letterGridController, 
				int indexOfLetterBarCell, 
				LetterSoundComposite parent = null)
	{           

	
		        bool letterIsNew = ChangedFromPrevious (indexOfLetterBarCell);
		        bool isPartOfCompletedGrapheme = !ReferenceEquals (parent, null);
				char newLetter = lc.AsString [0];
				newDefaultColor = lc.GetColour ();
		

				if (SessionsDirector.instance.IsSyllableDivisionActivity) {
						handleSyllableDivisionMode (lc, letterGridController, 
								indexOfLetterBarCell);
				} else if(SessionsDirector.IsStudentMode) {
						handleStudentMode (
								letterIsNew, 
								isPartOfCompletedGrapheme, 
								indexOfLetterBarCell,
								newLetter, 
								parent);
				} else {
						handleTeacherMode(
								isPartOfCompletedGrapheme,
								letterIsNew, newLetter);
				}

				    Debug.Log ($"{newLetter} {newDefaultColor}");


					asInteractiveLetter = letterGridController.UpdateLetter (indexOfLetterBarCell, newDefaultColor); 
					asInteractiveLetter.LetterSoundComponentIsPartOf = lc;
		           
		
				//todo disabled until understand whats going on.
			    /*if (flash) {
						asInteractiveLetter.StartFlash(flashColor, timesToFlash);
				}*/

			}
						

		void handleSyllableDivisionMode(
				LetterSoundComponent lc,
				LetterGridController letterGridController, 
				int indexOfLetterBarCell
		){
				asInteractiveLetter = letterGridController.GetInteractiveLetter (indexOfLetterBarCell);
				asInteractiveLetter.UpdateDefaultColour (SessionsDirector.colourCodingScheme.GetColorsForWholeWord ());
				asInteractiveLetter.SetSelectColour (lc.GetColour ());
		}

		void handleTeacherMode(
				bool isPartOfCompletedGrapheme,
				bool letterIsNew,
				char newLetter

		){

				//in teacher mode. if rule is r controlled vowel, consonant or vowel digraphs, need to check if this
				//letter is the first letter of any valid of these graphemes and flash it in that color if it is.
				if(!isPartOfCompletedGrapheme){
						string currentRule = SessionsDirector.instance.GetCurrentRule;
						flash = letterIsNew;
						timesToFlash = TIMES_TO_FLASH_CORRECT_PORTION_OF_FINAL_GRAPHEME;
						switch(currentRule){
						case "rControlledVowel":
								if(SpeechSoundReference.IsFirstLetterOfRControlledVowel(newLetter)){
										flashColor = SessionsDirector.instance.CurrentActivityColorRules.GetColorsForRControlledVowel();
										newDefaultColor = Color.gray;
								}
								break;
						case "consonantDigraphs":
								if(SpeechSoundReference.IsFirstLetterOfConsonantDigraph(newLetter)){
										flashColor = SessionsDirector.instance.CurrentActivityColorRules.GetColorsForConsonantDigraphs();
										newDefaultColor = Color.gray;
								}
								break;
						case "vowel Digraphs":
								if(SpeechSoundReference.IsFirstLetterOfVowelDigraph(newLetter)){
										flashColor = SessionsDirector.instance.CurrentActivityColorRules.GetColorsForVowelDigraphs();
										newDefaultColor = Color.gray;
								}
								break;
						default:
								flash = false;
								timesToFlash = 0;
								break;

						}
				}
		}


		void handleStudentMode(
				bool letterIsNew,
				bool isPartOfCompletedGrapheme,
				int indexOfLetterBarCell, 
				char newLetter,
				LetterSoundComposite parent = null
		){
				if(studentActivityController.IsErroneous(indexOfLetterBarCell)){
						Color[] errorColors = SessionsDirector.colourCodingScheme.GetErrorColors();
						newDefaultColor = errorColors[0];
						flashColor = errorColors[1];
						flash = letterIsNew;
						timesToFlash=TIMES_TO_FLASH_ERRORNEOUS_LETTER;
						return;
				} 
						

				//correct letter; see whether it's part of a multi-letter unit.
				LetterSoundComponent targetComponent = 
						studentActivityController.GetTargetLetterSoundComponentFor(indexOfLetterBarCell);

				if( //did student place all the letters needed to instantiate the spelling rule they are practiscing?
						(isPartOfCompletedGrapheme && targetComponent.AsString.Equals(parent.AsString)) || 
						(SessionsDirector.instance.IsMagicERule && studentActivityController.IsSubmissionCorrect())) {

						flash = true;
						timesToFlash = TIMES_TO_FLASH_ON_COMPLETE_TARGET_GRAPHEME;
						return;
				} 

				//if the target grapheme for this index is part of a multi-letter unit (a blend, stable syllable, digraph or
				//ar vowel)
				if(!ReferenceEquals(targetComponent, null)){

						newDefaultColor = SessionsDirector.instance.
								CurrentActivityColorRules.GetColorForPortionOfTargetComposite ();

						//If it isn't blends, then should flash.
						//todo abstract this into the color coding scheme as well.
						if (!SessionsDirector.instance.IsConsonantBlends) {
								flash = true;
								flashColor = targetComponent.GetColour ();
								timesToFlash = TIMES_TO_FLASH_CORRECT_PORTION_OF_FINAL_GRAPHEME;
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

				foreach (string s in SpeechSoundReference.Vowels()) {
					
						if (Input.GetKeyDown (s)) {
								SetTestLetter (s);
								return true;
						}
				}

				foreach (string s in SpeechSoundReference.Consonants()) {
				
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
