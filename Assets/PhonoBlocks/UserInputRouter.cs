using UnityEngine;
using System.Text;
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
		public static int numArduinoControlledLetters = 7; 
		public GameObject studentActivityControllerGO;
		public GameObject arduinoAndAffixLetterControllerGO;
		public GameObject onscreenKeyboardControllerGO;
		public GameObject wordHistoryControllerGO;

		public GameObject arduinoLetterInterfaceG0;
		public GameObject uniduinoG0;
		public GameObject checkedWordImageControllerGO;
		public GameObject hintButtonGO;



		public SessionManager sessionManager;
		public ArduinoAffixLetterController arduinoAndAffixLetterController;
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
						return arduinoAndAffixLetterController.GetUserControlledLettersAsString (false);

				}

		}
	  
		// Use this for initialization
		void Start ()
		{
		      
				global = this;
				sessionParametersOB = GameObject.Find ("SessionParameters");


             
				screenMode = sessionParametersOB.GetComponent<SessionManager> ().IsScreenMode ();

       


				


				if (screenMode) { //activate the onscreen keyboard object
						screenKeyboardController = onscreenKeyboardControllerGO.GetComponent<ScreenKeyboardController> ();
						screenKeyboardController.Initialize ();
            arduinoLetterInterfaceG0.SetActive(false);
            uniduinoG0.SetActive(false);


        } else { //tangible mode; activate the arduino unity interface and uniduino objects.
            arduinoLetterInterface = arduinoLetterInterfaceG0.GetComponent<ArduinoUnityInterface>();
            arduinoLetterInterfaceG0.SetActive (true);
            arduinoLetterInterface.Initialize();
                        uniduinoG0.SetActive (true);
            uniduinoG0.GetComponent<Uniduino.Arduino>().Connect();


        }



	

				totalLengthOfUserInputWord = numArduinoControlledLetters;
				arduinoAndAffixLetterController = arduinoAndAffixLetterControllerGO.GetComponent<ArduinoAffixLetterController> ();
				arduinoAndAffixLetterController.Initialize (arduinoLetterInterface);
				
				wordHistoryController = wordHistoryControllerGO.GetComponent<WordHistoryController> ();
				wordHistoryController.Initialize (totalLengthOfUserInputWord);
			
				checkedWordImageController = checkedWordImageControllerGO.GetComponent<CheckedWordImageController> ();
			
				foreach (PhonoBlocksController c in Resources.FindObjectsOfTypeAll<PhonoBlocksController>())
						c.UserInputRouter = global;

				hintButtonGO.SetActive (false);
			



				if (sessionParametersOB != null) {
					
						sessionManager = sessionParametersOB.GetComponent<SessionManager> ();
						if (SessionManager.DelegateControlToStudentActivityController) {
							
								studentActivityControllerGO = sessionManager.studentActivityControllerOB;
								studentActivityController = studentActivityControllerGO.GetComponent<StudentActivityController> ();
				                
								studentActivityController.Initialize (hintButtonGO);

						} 
			            
				}
		    	

		      
		        
		}



    public bool IsScreenMode()
    {

        return screenMode;


    }

    public bool IsArduinoMode()
    {

        return !screenMode;


    }

    public void RequestReplayInstruction ()
		{
				if (SessionManager.DelegateControlToStudentActivityController)
						studentActivityController.PlayInstructions ();

		}

		public bool TeacherMode ()
		{
				return sessionParametersOB == null || SessionManager.IsTeacherMode;

		}

		public void UnlockNthArduinoControlledLetter (int rawPosition)
		{
				GameObject nthArduinoControlledLetter = arduinoAndAffixLetterController.GetNthUserControlledLetter (rawPosition);
				nthArduinoControlledLetter.GetComponent<Selectable> ().UnLock ();
				if (!screenMode)
						arduinoLetterInterface.ColorNthTangibleLetter (rawPosition, nthArduinoControlledLetter.GetComponent<InteractiveLetter> ().CurrentColor ());
		         


		}
	
		public void LockNthArduinoControlledLetter (int rawPosition)
		{
				arduinoAndAffixLetterController.GetNthUserControlledLetter (rawPosition).GetComponent<Selectable> ().Lock ();
		
		
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

		public void ShowNonBlankLettersInPlacesAsHintToUser (string missingLettersHint)
		{
				for (int i=0; i<missingLettersHint.Length; i++) 
						if (missingLettersHint [i] != ' ') {
								InteractiveLetter nthLetter = arduinoAndAffixLetterController.GetNthUserControlledLetter (i).GetComponent<InteractiveLetter> ();
								nthLetter.ChangeStateToSignalLocked ();
								nthLetter.SwitchImageTo (LetterImageTable.instance.GetLetterImageFromLetter (missingLettersHint [i]));
						}

		}
	  
		/*
	     * called by the selectable component.
*/
		public bool AcceptingSelectionOrActivation ()
		{
				return acceptUIInput;

		}

		public void ShowLockedImageAtBlankLockedArduinoControlledPosition (int rawPosition)
		{       
				arduinoAndAffixLetterController.GetNthUserControlledLetter (rawPosition).GetComponent<InteractiveLetter> ().SwitchImageTo (LetterImageTable.LockedLetterImage);

		}

		public void RemoveLockedLetterImageAtBlankLockedArduinoControlledPosition (int rawPosition)
		{
			
				arduinoAndAffixLetterController.GetNthUserControlledLetter (rawPosition).GetComponent<InteractiveLetter> ().SwitchImageTo (LetterImageTable.BlankLetterImage);


		}


	    


		public void DeselectArduinoControlledLetters ()
		{

				arduinoAndAffixLetterController.DeselectUserControlledLetters ();

		}

		public void ShutDownUI (bool removeColoursOfLetters)
		{
				//tell each letter to "shut down" (appear white) and change the colors of all the arduino letters
				//accepting input general is false
				//new letters are accepted but we set them to invisible
				acceptUIInput = false;
				if (removeColoursOfLetters) {
						LockAllSelectables ();

						arduinoAndAffixLetterController.AllNewLettersAreInvisible (); //aere we still doing this?
				
					
		        
				}

		}

		public void ReactivateUI (bool restoreColoursOfLetters)
		{
				acceptUIInput = true;
				if (restoreColoursOfLetters) {
						UnlockAllSelectables ();

						arduinoAndAffixLetterController.AllNewLettersAreVisible ();
						//the user may have changed things before unlocking that require us to update the colours of the se;ectable letters.
						UserWord newWord = arduinoAndAffixLetterController.RefreshAndRestoreColours ();
						if (!screenMode && newWord != null)
								arduinoLetterInterface.UpdateColoursOfTangibleLetters (newWord);
		        
				}


		}

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



		//
		public void RequestDisplayImage (Texture2D newimg, bool disableTextureOnPress)
		{
				if (acceptUIInput)
						checkedWordImageController.ShowImage (newimg, disableTextureOnPress);
		}

		public void RequestDisplayImage (Texture2D newimg, bool disableTextureOnPress, bool force)
		{
				checkedWordImageController.ShowImage (newimg, disableTextureOnPress);
		}

		//check word
		public void RequestCheckWord ()
		{
				if (acceptUIInput) {
						if (arduinoAndAffixLetterController.NoUserControlledLetters ())
								return;
						if (TeacherMode ())
								AddCurrentWordToHistory (true);//wordHistoryController.AddCurrentWordToHistory (arduinoAndAffixLetterController.GetAllUserInputLetters (false));
						else
								studentActivityController.HandleSubmittedAnswer (arduinoAndAffixLetterController.GetUserControlledLettersAsString (true));
				}
		}



		public void AddCurrentWordToHistory (bool playSound)
		{
				wordHistoryController.AddCurrentWordToHistory (arduinoAndAffixLetterController.GetAllUserInputLetters (false), playSound);

		}

		public void ClearWordHistory ()
		{
				wordHistoryController.ClearWordHistory ();


		}
	                                              

		//new arduino letter
		public void RequestAddOrRemoveArduinoLetter (char newLetter, int atPosition, ArduinoLetterController alc)
		{
				if (checkedWordImageController.WordImageIsOnDisplay ())
						checkedWordImageController.EndDisplay ();

				bool shouldUpdateArduinoLetterControlledLetters = true;
				if (sessionManager != null && SessionManager.DelegateControlToStudentActivityController)
            //shouldUpdateArduinoLetterControlledLetters = studentActivityController.HandleNewArduinoLetter (newLetter, alc.TranslatePositionOfLetterInUILetterBarToRaw (atPosition));
            shouldUpdateArduinoLetterControlledLetters = studentActivityController.HandleNewArduinoLetter(newLetter, atPosition);

        if (shouldUpdateArduinoLetterControlledLetters)  //
						arduinoAndAffixLetterController.UserChangedALetter (newLetter, atPosition);
					
				


		}

		public void RequestPlaySoundsOfSelectedArduinoLetters ()
		{
				if (acceptUIInput && !SessionManager.IsAssessmentMode) 
						arduinoAndAffixLetterController.PlaySoundsOfSelectedLetters ();
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
				arduinoAndAffixLetterController.GetArduinoLetterController.LetterClicked (cell);



		}

		public void TellUserToPlaceInitialLetters ()
		{
				studentActivityController.PlayInstructions ();

		}

		public List<InteractiveLetter> GetAllArduinoControlledLetters {
				get {
						return arduinoAndAffixLetterController.GetAllUserInputLetters (false);
			
				}
		}

		public void RequestOverwriteArduinoControllerLettersWith (string word, GameObject requester)
		{
				arduinoAndAffixLetterController.OverwriteUserLettersWith (word, requester);


		}

		public void RequestSetAllArduinoLettersToBlank (GameObject requester)
		{

				arduinoAndAffixLetterController.SetAllUserControlledLettersToBlank (requester);

		}

		public void UpdateColoursOfLetters ()
		{
				arduinoAndAffixLetterController.RefreshAndRestoreColours ();



		}

		




	

}
