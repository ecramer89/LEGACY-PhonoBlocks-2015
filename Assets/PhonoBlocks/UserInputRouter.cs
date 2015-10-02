using UnityEngine;
using System.Text;
using System;
using System.Collections;
using System.Collections.Generic;

//to do... a more elegant way of handling different modes 
//versus just swirtching out things like this.


//...misnamed. is going to be the global controller.
public class UserInputRouter : MonoBehaviour
{

		public GameObject sessionParametersOB;
		bool screenMode;
		public static UserInputRouter global;
		public static int totalLengthOfUserInputWord;
		public static int numOnscreenLetterSpaces = 6; 

		//the first slot in the physical prototype does not function.
		//it was easier to just let the system create that slot but fill it with a blank than to change everything else in the code
		//the problem class needs to know how many actual useable letter spaces there are for this to function... 
		//so we created this other method to distinguish between number of on screen spaxces and number of those spaces that are useable
		/*public static int NumOfActiveAndUseableArduinoControlledLetters(){
		return numOnscreenLetterSpaces - 1;

	}*/
		public GameObject studentActivityControllerGO;
		//public GameObject arduinoAndAffixLetterControllerGO;
		public GameObject arduinoLetterControllerGO;
		public GameObject onscreenKeyboardControllerGO;
		public GameObject wordHistoryControllerGO;
		public GameObject arduinoLetterInterfaceG0;
		public GameObject uniduinoG0;
		public GameObject checkedWordImageControllerGO;
		public GameObject hintButtonGO;
		public SessionsDirector sessionManager;
		//public ArduinoAffixLetterController arduinoAndAffixLetterController;
		public ArduinoLetterController arduinoLetterController;
		public WordHistoryController wordHistoryController;
		public ArduinoUnityInterface arduinoLetterInterface;
		public ScreenKeyboardController screenKeyboardController;
		public CheckedWordImageController checkedWordImageController;
		public StudentActivityController studentActivityController;
		bool acceptUIInput = true;
		bool currentWordViolatesPhonotactics = false; //either the arduino controlled letters or one of the affixes was placed incorrectly.
		//dont add to the history.
	    

		public AudioClip SAVED_DATA_CLIP;

		public string CurrentArduinoControlledLettersAsString {
				get {
						return arduinoLetterController.GetUserControlledLettersAsString (false);

				}

		}
	  
		// Use this for initialization
		void Start ()
		{
		      
				global = this;
				sessionParametersOB = GameObject.Find ("SessionParameters");


             
				screenMode = sessionParametersOB.GetComponent<SessionsDirector> ().IsScreenMode ();

       


				


				if (screenMode) { //activate the onscreen keyboard object
						screenKeyboardController = onscreenKeyboardControllerGO.GetComponent<ScreenKeyboardController> ();
						screenKeyboardController.Initialize ();
						arduinoLetterInterfaceG0.SetActive (false);
						uniduinoG0.SetActive (false);


				} else { //tangible mode; activate the arduino unity interface and uniduino objects.
						arduinoLetterInterface = arduinoLetterInterfaceG0.GetComponent<ArduinoUnityInterface> ();
						arduinoLetterInterfaceG0.SetActive (true);
						arduinoLetterInterface.Initialize ();
						uniduinoG0.SetActive (true);
						uniduinoG0.GetComponent<Uniduino.Arduino> ().Connect ();


				}



	

				totalLengthOfUserInputWord = numOnscreenLetterSpaces;
	
				arduinoLetterController = arduinoLetterControllerGO.GetComponent<ArduinoLetterController> ();
				arduinoLetterController.Initialize (0, numOnscreenLetterSpaces - 1, arduinoLetterInterface);
				wordHistoryController = wordHistoryControllerGO.GetComponent<WordHistoryController> ();
				wordHistoryController.Initialize (totalLengthOfUserInputWord);
			
				checkedWordImageController = checkedWordImageControllerGO.GetComponent<CheckedWordImageController> ();
			
				foreach (PhonoBlocksController c in Resources.FindObjectsOfTypeAll<PhonoBlocksController>())
						c.UserInputRouter = global;

				hintButtonGO.SetActive (false);
			



				if (sessionParametersOB != null) {
					
						sessionManager = sessionParametersOB.GetComponent<SessionsDirector> ();
						if (SessionsDirector.DelegateControlToStudentActivityController) {
							
								studentActivityControllerGO = sessionManager.studentActivityControllerOB;
								studentActivityController = studentActivityControllerGO.GetComponent<StudentActivityController> ();
				                
								studentActivityController.Initialize (hintButtonGO, arduinoLetterController);

						} 
			            
				}
		    	

		      
		        
		}

		public bool IsScreenMode ()
		{

				return screenMode;


		}

		public bool IsArduinoMode ()
		{

				return !screenMode;


		}

		public void RequestReplayInstruction ()
		{
				if (SessionsDirector.DelegateControlToStudentActivityController)
						studentActivityController.PlayInstructions ();

		}

		public bool TeacherMode ()
		{
				return sessionParametersOB == null || SessionsDirector.IsTeacherMode;

		}


		public void RequestHint ()
		{//and if the UI is not on lockdown.
				if (!TeacherMode () && acceptUIInput)
						studentActivityController.UserRequestsHint ();


		}


		//touch the desk to deselect any selected selectables.
		public void DeskTouched ()
		{
				if (checkedWordImageController != null)
						checkedWordImageController.EndDisplay ();
	

		}
	/*
		public void ShowNonBlankLettersInPlacesAsHintToUser (string missingLettersHint)
		{
				for (int i=0; i<missingLettersHint.Length; i++) 
						if (missingLettersHint [i] != ' ') {
								InteractiveLetter nthLetter = arduinoAndAffixLetterController.GetNthUserControlledLetter (i).GetComponent<InteractiveLetter> ();
								nthLetter.ChangeStateToSignalLocked ();
								nthLetter.SwitchImageTo (LetterImageTable.instance.GetLetterImageFromLetter (missingLettersHint [i]));
						}

		}*/
	  
		/*
	     * called by the selectable component.
*/
		public bool AcceptingSelectionOrActivation ()
		{
				return acceptUIInput;

		}

		/*

		public void BlockUserInputAndTurnOffLetters (bool turnAllLettersOff)
		{
				//tell each letter to "shut down" (appear white) and change the colors of all the arduino letters
				//accepting input general is false
				//new letters are accepted but we set them to invisible
				acceptUIInput = false;
				if (turnAllLettersOff) {
						LockAllSelectables ();
					
				
					
				}

		}

		public void ReactivateUI (bool restoreColoursOfLetters)
		{
				acceptUIInput = true;
				if (restoreColoursOfLetters) {
						UnlockAllSelectables ();

						arduinoAndAffixLetterController.RecolorAllLettersWhenAnyLetterChanges (true);
						//the user may have changed things before unlocking that require us to update the colours of the se;ectable letters.
						UserWord newWord = arduinoAndAffixLetterController.RefreshAndRestoreColours ();
						if (!screenMode && newWord != null)
								arduinoLetterInterface.UpdateColoursOfTangibleLetters (newWord);
		        
				}


		}*/

		public void UnlockAllSelectables ()
		{
	
				foreach (Selectable s in Resources.FindObjectsOfTypeAll<Selectable>()) {
						if (s.gameObject.activeSelf)
								s.UnLock (); 
				}
			
		}

		public void LockAllSelectables ()
		{
		
				foreach (Selectable s in Resources.FindObjectsOfTypeAll<Selectable>()) {
						if (s.gameObject.activeSelf)
								s.Lock (); 
				}
		
		}

		public void RequestTurnOffImage ()
		{
				checkedWordImageController.EndDisplay ();
		}

		//
		public void RequestDisplayImage (Texture2D newimg, bool disableTextureOnPress, bool indefinite=false)
		{
				
				checkedWordImageController.ShowImage (newimg, disableTextureOnPress, indefinite);
		}

		
		//check word
		public void RequestCheckWord ()
		{
				if (acceptUIInput) {
						if (arduinoLetterController.NoUserControlledLetters ())
								return;
						if (TeacherMode ())
								AddCurrentWordToHistory (true);//wordHistoryController.AddCurrentWordToHistory (arduinoAndAffixLetterController.GetAllUserInputLetters (false));
						else
								studentActivityController.HandleSubmittedAnswer (arduinoLetterController.GetUserControlledLettersAsString (true));
				}
		}

		public void AddCurrentWordToHistory (bool playSound)
		{
				wordHistoryController.AddCurrentWordToHistory (arduinoLetterController.GetAllUserInputLetters (false), playSound);

		}

		public void ClearWordHistory ()
		{
				wordHistoryController.ClearWordHistory ();


		}
	                                              

		/* if it is activity mode, then we delegate control of the new letter to the student activity controller. otherwise just update all of the letters*/
		public void HandleNewUserInputLetter (char newLetter, int atPosition, ArduinoLetterController alc)
		{
				if (sessionManager != null && SessionsDirector.DelegateControlToStudentActivityController)
			
						studentActivityController.HandleNewArduinoLetter (newLetter, atPosition);
				else 

						arduinoLetterController.UpdateDefaultColoursAndSoundsOfLetters (true);

		}


		public void RequestPlayWord (string word)
		{
				if (acceptUIInput)
						OSCWordReader.instance.Read (word);


		}
		
		void DeselectAllSelected ()
		{	
				foreach (Selectable s in Resources.FindObjectsOfTypeAll<Selectable>()) {
						if (s.gameObject.activeSelf && s.Selected)
								s.Deselect (false); 
				}
		
		}

		public void LetterClicked (GameObject cell)
		{
				arduinoLetterController.LetterClicked (cell);



		}

		public void TellUserToPlaceInitialLetters ()
		{
				studentActivityController.PlayInstructions ();

		}

		public List<InteractiveLetter> GetAllArduinoControlledLetters {
				get {
						return arduinoLetterController.GetAllUserInputLetters (false);
			
				}
		}

}
