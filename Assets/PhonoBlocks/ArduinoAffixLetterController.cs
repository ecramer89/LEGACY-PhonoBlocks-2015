using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System;

public class ArduinoAffixLetterController : PhonoBlocksController
{

		public GameObject letterGridControllerGO;
		public GameObject arduinoLetterControllerGO;
    public Texture2D noLetterHere; //a blank (no line) image to put in the place of the inactive 0th letter

    ArduinoLetterController arduinoLetterController;

		public ArduinoLetterController GetArduinoLetterController {
				get {
						return arduinoLetterController;
				}


		}

		LetterGridController letterGridController;
		ArduinoUnityInterface tangibleLetters;
		bool updateColorsAndImagesOfUILettersOnChange = true;
		UserWord currUserWord;
	
		public UserWord CurrentUserWord {
				get {
						return currUserWord;
				}
		
		
		}

		public bool IsAnOutstandingPhonotacticsError ()
		{
				return currUserWord.ViolatesPhonotactics;

		}

		public void Initialize (ArduinoUnityInterface tangibleLetters)
		{
				int totalLengthOfWord = UserInputRouter.numArduinoControlledLetters;
				letterGridController = letterGridControllerGO.GetComponent<LetterGridController> ();
       

     
        letterGridController.InitializeBlankLetterSpaces (UserInputRouter.numArduinoControlledLetters, arduinoLetterControllerGO);
        //we aren't using the letter cell at position 0 in Min's study because that slot is not working.
        //easiest way to deal with this right now is to make it so the arduino only recognizes inpiuts that come from positions 1 through 6 (inclusive)
        //and to here, retrieve the 0th cell, and change the picture from the blank with the line to the straight blank.
        letterGridController.UpdateLetterImage(0, noLetterHere);
		
		arduinoLetterController = arduinoLetterControllerGO.GetComponent<ArduinoLetterController> ();

				arduinoLetterController.StartingIndex = 1;

        arduinoLetterController.EndingIndex = UserInputRouter.numArduinoControlledLetters - 1;
        //arduinoLetterController.EndingIndex = 6; //because of changed alignment of the platform
                arduinoLetterController.MaxArduinoLetters = UserInputRouter.numArduinoControlledLetters;
				arduinoLetterController.Initialize (letterGridController);

				this.tangibleLetters = tangibleLetters;


				
				
		}


		public void DeselectUserControlledLetters ()
		{
				foreach (InteractiveLetter i in letterGridController.GetLetters(true))
						i.GetComponent<Selectable> ().Deselect (false);
           


		}




		public void OverwriteUserLettersWith (string word, GameObject requester)
		{
				
				arduinoLetterController.OverwriteLettersWith (word, letterGridController, requester);

		}

		public void SetAllUserControlledLettersToBlank (GameObject requester)
		{
				letterGridController.SetAllLettersToBlank (requester);
		
		
		}




		/*
		 * method called by the arduino unity interface and the onscreen keyboard when the user changes a letter
		 * 
		 * */

		public UserWord UserChangedALetter (char newLetter, int atPosition)
		{
				
				arduinoLetterController.SaveNewLetterInStringRepresentation (newLetter, atPosition);
				UserWord updatedLetterSoundUnits = null;
				InteractiveLetter l = letterGridController.UpdateLetter (arduinoLetterControllerGO, atPosition, newLetter + "");
			
				if (updateColorsAndImagesOfUILettersOnChange)
						updatedLetterSoundUnits = arduinoLetterController.UpdateColorsAndSoundsOfLetters (letterGridController, true);
				else
						l.gameObject.GetComponent<Selectable> ().Lock ();

				currUserWord = updatedLetterSoundUnits;

				return updatedLetterSoundUnits;
		
		}





		//includes those controled by the affixes. if you just want the ones that srtore the arduino controlled letters,
		//then use GetAllArduinoControlledLetters.
		public List<InteractiveLetter> GetAllUserInputLetters (bool skipBlanks)
		{

				return letterGridController.GetLetters (skipBlanks);

		}

		public void AllNewLettersAreInvisible ()
		{
				updateColorsAndImagesOfUILettersOnChange = false;


		}

		public void AllNewLettersAreVisible ()
		{
				updateColorsAndImagesOfUILettersOnChange = true;
				

		}



		public UserWord RefreshAndRestoreColours ()
		{      
		
				return arduinoLetterController.UpdateColorsAndSoundsOfLetters (letterGridController, true);
		
		}

		public GameObject GetNthUserControlledLetter (int rawPosition)
		{
        //return letterGridController.GetLetterCell (arduinoLetterController.TranslateRawPositionToPositionOfLetterInUILetterBar (rawPosition));
        return letterGridController.GetLetterCell(rawPosition);

    }


	    
		int FindFirstNonBlankIndex (int from, int dir)
		{
				dir = (dir < 0 ? -1 : 1);
				int end = (dir > 0 ? letterGridControllerGO.transform.childCount : -1);
		
				for (int i=from; i!=end; i+=dir) {
						GameObject cell = letterGridControllerGO.transform.Find (i + "").gameObject;
						if (!cell.GetComponent<InteractiveLetter> ().Letter ().Equals (" "))
								return i;
			
			
				}
				return -1;
		}


		public bool NoUserControlledLetters ()
		{

				return arduinoLetterController.NoUserLetters ();

		}

		public string GetUserControlledLettersAsString (bool onlySelected)
		{
				if (onlySelected)
						return arduinoLetterController.AllSelectedLettersAsString (letterGridController);
				return arduinoLetterController.CurrentUserControlledLettersAsString;

		}
	
		public void PlaySoundsOfSelectedLetters ()
		{
				//retrieve sounds from the 
				StringBuilder selectedLetters = new StringBuilder ();
				List<InteractiveLetter> selectableLetters = letterGridController.GetLetters (false);
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
				if (syll == null)
						OSCWordReader.instance.Read (subword);
				else
						AudioSourceController.PushClip (syll);
		
		StudentsDataHandler.instance.LogEvent ("played_letter_sounds", subword, "NA");
		
		}





}
