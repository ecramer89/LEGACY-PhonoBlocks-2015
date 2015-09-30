using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

public class StudentActivityController : PhonoBlocksController
{


		enum State
		{
				PLACE_INITIAL_LETTERS,
				MAIN_ACTIVITY,
				REMOVE_ALL_LETTERS,
			
				BREAK_BEFORE_END



		}

		State state = State.PLACE_INITIAL_LETTERS;
		LockedPositionHandler lockedPositionHandler;
		HintController hintController;
		Problem currProblem;
		public AudioClip correctFeedback;
		char[] usersMostRecentChanges;
		public AudioClip incorrectFeedback;
		public AudioClip sessionIsFinished;
		public Texture2D sessionFinishedImage;
		public AudioClip takeABreakBetweenSessions;
		public AudioClip pressOrTapScreenToContinue;



		//stupid hack to prevent skipping without removing all letters.
		char[] lettersThatUserNeedsToRemoveBeforeSkipping;

		public int NonBlankLettersThatUserHasPlaced {
				get {
						int result = 0;
						for (int i=0; i<usersMostRecentChanges.Length; i++)
								if (usersMostRecentChanges [i] != ' ')
										result++;
						return result;
				}


		}

		public string UserChangesAsString {
				get {
						StringBuilder result = new StringBuilder ();

						for (int i=0; i<usersMostRecentChanges.Length; i++)
								result.Append (usersMostRecentChanges [i]);

						return result.ToString ();
		
				}

		}

		public string TargetLettersUserHasYetToPlace {
				get {
						string target = currProblem.TargetWord (false);
						StringBuilder result = new StringBuilder ();
			
						for (int i=0; i<usersMostRecentChanges.Length; i++) {
								char targetLetter = target [i];
								if (usersMostRecentChanges [i] != targetLetter)
										result.Append (targetLetter);
								else
										result.Append (' ');

						}
			
						return result.ToString ();


				}

		}

		public void Initialize (GameObject hintButton)
		{
				usersMostRecentChanges = new char[UserInputRouter.numArduinoControlledLetters];
				lettersThatUserNeedsToRemoveBeforeSkipping = new char[UserInputRouter.numArduinoControlledLetters];
			
				lockedPositionHandler = gameObject.GetComponent<LockedPositionHandler> ();
				lockedPositionHandler.Initialize ();
				hintController = gameObject.GetComponent<HintController> ();
				hintController.Initialize (hintButton);
				SwipeDetector.MousePressed += HandleMousePress;
				SetUpNextProblem ();

	
		}

		public void SetUpNextProblem ()
		{  

				//get the next specific problem from the ProblemType class
				ClearSavedUserChanges ();
				hintController.Reset ();
			
				currProblem = ProblemsRepository.instance.GetNextProblem ();
	
				StudentsDataHandler.instance.RecordActivityTargetWord (currProblem.TargetWord (false));


				lockedPositionHandler.ResetForNewProblem ();
				lockedPositionHandler.RememberPositionsThatShouldNotBeChanged (currProblem.InitialWord, currProblem.TargetWord (false)); 
				lockedPositionHandler.NumTangibleLettersThatUserMustMatchToLockedUILetters = currProblem.NumInitialLetters;
				
		           
				userInputRouter.RequestSetAllArduinoLettersToBlank (gameObject);
				userInputRouter.RequestOverwriteArduinoControllerLettersWith (currProblem.InitialWord, gameObject);
			






				userInputRouter.ShutDownUI (true);
				hintController.DeActivateHintButton ();
				//play the instructions
				if (!SessionsDirector.IsAssessmentMode && ArduinoUnityInterface.communicationWithArduinoAchieved)
						PlayInstructions (); //dont bother telling to place initial letters during assessment mode

	
				state = State.PLACE_INITIAL_LETTERS;


				//In case the initial state is already correct (...which happens when the user needs to build the word "from scratch". This makes it so
				//we don;t need to trigger the check by adding a "blank"!
				CheckAndUpdateState ();
		         
				//
		}
        
		public void PlayInstructions ()
		{
			
				currProblem.PlayCurrentInstruction ();
			
				
		}

		void ClearSavedUserChanges ()
		{
				for (int i=0; i<usersMostRecentChanges.Length; i++) {
						usersMostRecentChanges [i] = ' ';
						lettersThatUserNeedsToRemoveBeforeSkipping [i] = ' ';
				}


		}

		public void CheckAndUpdateState ()
		{
				if (lockedPositionHandler.AllLockedPositionsAreInCorrectState ()) {
						if (state == State.PLACE_INITIAL_LETTERS) {
								BeginMainProblemStateOrTellUserToRemoveNonInitialLetters ();
				
						}
						if (state == State.REMOVE_ALL_LETTERS) {
						
								HandleEndOfActivity ();
								return;
						}
						
						userInputRouter.ReactivateUI (true);

				
				}
		}

		void HandleEndOfActivity ()
		{
				if (ProblemsRepository.instance.AllProblemsDone ()) {
						StudentsDataHandler.instance.UpdateUserSessionAndWriteAllUpdatedDataToPlayerPrefs ();
						//StudentsDataHandler.WriteDataOfCurrentStudentToCSV ();
						AudioSourceController.PushClip (sessionIsFinished);
						state = State.BREAK_BEFORE_END;
				} else
						SetUpNextProblem ();

		}

		public void HandleMousePress (object sender, EventArgs args)
		{  
				if (state == State.BREAK_BEFORE_END) {

						userInputRouter.RequestDisplayImage (sessionFinishedImage, true, true);

				}
	
		}

		void BeginMainProblemStateOrTellUserToRemoveNonInitialLetters ()
		{
				if (NoLettersThatAreNotFromTheInitialOnesRemain ()) {
						BeginMainProblemState ();
				} else {
					
						//AudioSourceController.PushClip (remove_a_NonInitialLetter);
				}

		}

		void BeginMainProblemState ()
		{
				currProblem.AdvanceInstruction (userInputRouter.CurrentArduinoControlledLettersAsString);
				currProblem.PlayCurrentInstruction ();
			
				state = State.MAIN_ACTIVITY;

		}

		public void UserRequestsHint ()
		{
				if (hintController.UsedLastHint ()) {
						currProblem.PlayAnswer ();
						userInputRouter.RequestOverwriteArduinoControllerLettersWith (currProblem.TargetWord (false), gameObject);
						CurrentProblemCompleted (false, UserChangesAsString);
				} else 
						hintController.ProvideHint (currProblem);

		}

		bool NoEarlierLetterSpacesAreBlank (int positionOfNewestLetter)
		{
				for (int i=0; i<positionOfNewestLetter; i++)
						if (usersMostRecentChanges [i] == ' ')
								return false;
				return true;

		}

		bool UsersPreviousChangesMatch (int positionOfNewestLetter, string word)
		{
				for (int i=0; i<positionOfNewestLetter; i++) {
						if (usersMostRecentChanges [i] != word [i]) {
								return false;
					
						}
				}
				return true;


		}

		public bool HandleNewArduinoLetter (char letter, int atPosition)
		{
				if (atPosition > -1 && atPosition < lettersThatUserNeedsToRemoveBeforeSkipping.Length)
						lettersThatUserNeedsToRemoveBeforeSkipping [atPosition] = letter;



				bool allowArduinoControlledLettersToUpdate = false; //note: this includes updating the string representation of the arduino
				//controlled letters. if this method returns false then the arduino letter controller's update method never gets called.
				bool letterRemoved = letter == ' ';
				bool positionWasLocked = lockedPositionHandler.IsLocked (atPosition);

				switch (state) {
				case State.PLACE_INITIAL_LETTERS:

						if (letterRemoved) {
								if (positionWasLocked) {
										lockedPositionHandler.HandleChangeToLockedPosition (atPosition, letter, currProblem.TargetWord (false), usersMostRecentChanges);
								}
								SaveUsersChange (atPosition, letter); 
								CheckAndUpdateState ();
						} else { //letter was added
								if (positionWasLocked) {
										if (UsersPreviousChangesMatchCurrentTargetNonBlankLetters (atPosition, currProblem.InitialWord)) {
												lockedPositionHandler.HandleChangeToLockedPosition (atPosition, letter, currProblem.TargetWord (false), usersMostRecentChanges);
												SaveUsersChange (atPosition, letter); 
												CheckAndUpdateState ();
										} else {
												//used to tell user to clear the locked slot (user has placed a letter in a locked slot). now we don;t.
										
										}
					
								} else { //added a letter in an unlocked position. not allowed to do this during initial placement,
										//however, we remember this change because we want users to "clear" the unlocked slots
										//of additional letters before starting the problem.

										if (userInputRouter.IsArduinoMode()) { //unless we're using the physical letters, simply dont allow the new letters to go thru
												
				
												SaveUsersChange (atPosition, letter); 
										}
								}
						}
						break;

				case State.MAIN_ACTIVITY:
						bool outOfRange = PositionIsOutsideBoundsOfTargetWord (atPosition);

						if (positionWasLocked && !outOfRange) {
								//does this include positio out of order?
								lockedPositionHandler.HandleChangeToLockedPosition (atPosition, letter, currProblem.TargetWord (false), usersMostRecentChanges);
								SaveUsersChange (atPosition, letter); //save the change becaue user needs to remedy the error;
								//possible that user just rememdied an error by removing a letter they should not have placed.
								CheckAndUpdateState ();

						} else {

								if (letterRemoved) {
										//removals are always legal;
										//(don't care about the order)
										//and we always remember that they occurred.
										//it's also possible that a removal puts the on screen letters into a "correct state"
										//so we remember that as well.
										SaveUsersChange (atPosition, letter); 
										CheckAndUpdateState ();
										allowArduinoControlledLettersToUpdate = true;
								} else {
										//if (NoEarlierLetterSpacesAreBlank (atPosition)) {
										SaveUsersChange (atPosition, letter); 
										CheckAndUpdateState ();
										allowArduinoControlledLettersToUpdate = true;
										
										//} else { //user added a letter "out of order".
						                     
										//we do not acknowledge the user's change in our
										//memory of the letters or on the screen or tangible letters.
										//PlayClearSlotAndAddLettersInOrderInstruction ();
										//}

								}
						}
						break;
				case State.REMOVE_ALL_LETTERS:
			//don't care about order; always pass to the locked position handler (when waiting for remove all,
			//the locked position handler considers all positions locked and it considers the target word to be a string (length of previous target)
			//composed of all blanks.
						lockedPositionHandler.HandleChangeToLockedPosition (atPosition, letter, currProblem.TargetWord (false), usersMostRecentChanges);
						SaveUsersChange (atPosition, letter); 
						CheckAndUpdateState ();
						break;
				}



				return allowArduinoControlledLettersToUpdate;

		}

		bool PositionIsOutsideBoundsOfTargetWord (int atPosition)
		{
				return atPosition >= currProblem.TargetWord (true).Length;
		}

		bool UserHasNoLettersToRemove ()
		{
				for (int i=0; i<lettersThatUserNeedsToRemoveBeforeSkipping.Length; i++)
						if (lettersThatUserNeedsToRemoveBeforeSkipping [i] != ' ')
								return false;
				return true;
		
		}

		bool CurrentlyRunningAnActivity ()
		{
				return state != State.BREAK_BEFORE_END;
		}

		public virtual void HandleSubmittedAnswer (string answer)
		{
				if (SessionsDirector.IsAssessmentMode) {
						CurrentProblemCompleted (SubmissionIsCorrect (answer), answer);
				} else {
						if (SubmissionIsCorrect (answer)) {
								AudioSourceController.PushClip (correctFeedback);
								currProblem.PlayAnswer ();
				
								CurrentProblemCompleted (true, answer);
				
						} else {
								HandleIncorrectAnswer ();				
				
						}

				}
				
			


		}

		protected void HandleIncorrectAnswer ()
		{
				hintController.ActivateHintButton ();
		
				AudioSourceController.PushClip (incorrectFeedback);
		
				if (hintController.OnLastHint ()) {
						//AudioSourceController.PushClip (offer_answer);
				} else {//it should not provide the hint until the user clicks the hint button
			
						//AudioSourceController.PushClip (offer_hint);
				}
				hintController.AdvanceHint ();

		}

		public void CurrentProblemCompleted (bool userSubmittedCorrectAnswer, string answer)
		{
			
				lockedPositionHandler.UserMustRemoveAllTheLettersOf = NonBlankLettersThatUserHasPlaced;
				state = State.REMOVE_ALL_LETTERS;

				currProblem.SetTargetWordToEmpty ();
				if (!SessionsDirector.IsAssessmentMode) //dont add the word to the history if this is test (assessment) mode.
						userInputRouter.AddCurrentWordToHistory (false);

				userInputRouter.DeselectArduinoControlledLetters (); 
				userInputRouter.ShutDownUI (false);

				StudentsDataHandler.instance.RecordActivitySolved (userSubmittedCorrectAnswer, answer);
			
				StudentsDataHandler.instance.SaveActivityDataAndClearForNext (currProblem.TargetWord (false), currProblem.InitialWord);
      
		}

		public void SaveUsersChange (int position, char change)
		{
		
				usersMostRecentChanges [position] = change;

		
		}

		bool NoLettersThatAreNotFromTheInitialOnesRemain ()
		{
				for (int i=0; i<usersMostRecentChanges.Length; i++) {
				
						if (usersMostRecentChanges [i] != currProblem.InitialWord [i]) {
								
								return false;
						}

				}
				return true;

		}

		
		//could make this faster by just checking the indexes that arent locked.
		//you should chanhge this method so that it refers to the user controlled letters (which would be fine)
		protected bool SubmissionIsCorrect (string answer)
		{      
				string target = currProblem.TargetWord (true);
			
				bool result = answer.Trim ().Equals (target);

				StudentsDataHandler.instance.LogEvent ("submitted_answer", answer, target);
				return result;

		}
	 
		bool UsersPreviousChangesMatchCurrentTargetNonBlankLetters (int positionOfNewestLetter, string currentTarget)
		{
	
				for (int i=0; i<positionOfNewestLetter; i++) {
						if (currentTarget [i] != ' ') {
								if (usersMostRecentChanges [i] != currentTarget [i]) {
										return false;

								}

						}
				}
				return true;

		}

		public void SelectedObjectActivated (GameObject interactiveLetterPressed)
		{
				userInputRouter.RequestPlaySoundsOfSelectedArduinoLetters ();
		}

		public void ObjectSelected (GameObject selectedLetter)
		{
			
						
				ArduinoLetterController ard = GameObject.Find ("ArduinoLetterController").GetComponent<ArduinoLetterController> ();
				ard.ObjectSelected (selectedLetter);
			
		
		}
	
		public void ObjectDeselected (GameObject selectedLetter)
		{      //during screen mode we can remove by swiping.
		
				if (state == State.REMOVE_ALL_LETTERS && userInputRouter.IsScreenMode()) {
						ObjectDeselected (selectedLetter, true);
			
				} else {
			
						userInputRouter.UpdateColoursOfLetters ();
			
				}
		
		
		
		}
	
		public void ObjectDeselected (GameObject selectedLetter, bool alreadyUpdated)
		{      //during screen mode we can remove by swiping.
	
				if (state == State.REMOVE_ALL_LETTERS && userInputRouter.IsScreenMode()) {
						int idx;
						Int32.TryParse (selectedLetter.name, out idx);
						ArduinoLetterController ard = GameObject.Find ("ArduinoLetterController").GetComponent<ArduinoLetterController> ();
						//idx = ard.TranslatePositionOfLetterInUILetterBarToRaw (idx);
						if (idx > -1 && idx < UserInputRouter.numArduinoControlledLetters)
								HandleNewArduinoLetter (' ', idx);
			
				} 



		}

}
