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
		ArduinoLetterController arduinoLetterController;
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

		public void Initialize (GameObject hintButton, ArduinoLetterController arduinoLetterController)
		{
				this.arduinoLetterController = arduinoLetterController;
				usersMostRecentChanges = new char[UserInputRouter.numOnscreenLetterSpaces];
				lettersThatUserNeedsToRemoveBeforeSkipping = new char[UserInputRouter.numOnscreenLetterSpaces];
			
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
				lockedPositionHandler.RememberPositionsThatShouldNotBeChanged (currProblem.InitialWord.Trim(), currProblem.TargetWord (false).Trim()); 
				lockedPositionHandler.NumTangibleLettersThatUserMustMatchToLockedUILetters = currProblem.NumInitialLetters;
				
		           
				arduinoLetterController.ReplaceEachLetterWithBlank ();
				arduinoLetterController.PlaceWordInLetterGrid (currProblem.InitialWord);
				arduinoLetterController.UpdateDefaultColoursAndSoundsOfLetters (false);
				arduinoLetterController.LockAllLetters ();


				userInputRouter.RequestTurnOffImage ();

				hintController.DeActivateHintButton ();
			
				PlayInstructions (); //dont bother telling to place initial letters during assessment mode

	
				state = State.PLACE_INITIAL_LETTERS;


				//In case the initial state is already correct (...which happens when the user needs to build the word "from scratch". This makes it so
				//we don;t need to trigger the check by adding a "blank"!
				ChangeProblemStateIfAllLockedPositionsAHaveCorrectCharacter ();
		         
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
						//lettersThatUserNeedsToRemoveBeforeSkipping [i] = ' ';
				}


		}

		public void ChangeProblemStateIfAllLockedPositionsAHaveCorrectCharacter ()
		{
				if (lockedPositionHandler.AllLockedPositionsAreInCorrectState ()) {
						if (state == State.PLACE_INITIAL_LETTERS) {
								BeginMainProblemState ();
				
						}
						if (state == State.REMOVE_ALL_LETTERS) {
						
								HandleEndOfActivity ();
								return;
						}
						
						

				
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

		void BeginMainProblemState ()
		{
			
				arduinoLetterController.UnLockAllLetters (); //so that when we call update colours and sounds they appear in their new colours.
				state = State.MAIN_ACTIVITY;

		}

		public void UserRequestsHint ()
		{
				if (hintController.UsedLastHint ()) {
						currProblem.PlayAnswer ();
						arduinoLetterController.PlaceWordInLetterGrid (currProblem.TargetWord (false));
						CurrentProblemCompleted (false, UserChangesAsString);
				} else 
						hintController.ProvideHint (currProblem);

		}




		//if the position is locked then delegate control to the locked position handler.
		//otherwise tell the arduino letter controller to remember the change and update the (default) colours and sounds of all letters.
		//no matter what, we remember the users' change and we always check to see whether we can switch the game state 


		public void HandleNewArduinoLetter (char letter, int atPosition)
		{       
			
				bool positionWasLocked = lockedPositionHandler.IsLocked (atPosition); 
				//we treat all positions as "locked" when the state is the end of the activity.
				if (positionWasLocked || state == State.REMOVE_ALL_LETTERS) {
						lockedPositionHandler.HandleChangeToLockedPosition (atPosition, letter, currProblem.TargetWord (false), usersMostRecentChanges, arduinoLetterController);
				} else if (!lockedPositionHandler.AllLockedPositionsAreInCorrectState ()) //if the user adds a letter to a portion of the string that isn't locked, (e.g., initial word is __nt and the child places w and e then w and e wont appear coloured... are you sure you want this? I don't know if that makes sense.
						arduinoLetterController.LockASingleLetter (atPosition);
				RecordUsersChange (atPosition, letter); 
		        
				ChangeProblemStateIfAllLockedPositionsAHaveCorrectCharacter ();
			   
				arduinoLetterController.UpdateDefaultColoursAndSoundsOfLetters (state != State.PLACE_INITIAL_LETTERS);

		}

		bool PositionIsOutsideBoundsOfTargetWord (int wordRelativeIndex)
		{
				return wordRelativeIndex >= currProblem.TargetWord (true).Length; 
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
			
						if (SubmissionIsCorrect (answer)) {
								//TO DO!!! then if this was the first time that student submitted an answer (get the data from the current student object)
								//then play the good hint else play the less good hint
								AudioSourceController.PushClip (correctFeedback);
								currProblem.PlayAnswer ();
				
								CurrentProblemCompleted (true, answer);
				
						} else {
								HandleIncorrectAnswer ();				
				
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

				arduinoLetterController.LockAllLetters ();
		        
				userInputRouter.RequestDisplayImage (WordImages.instance.GetWordImage (currProblem.TargetWord (true)), false, true);
				StudentsDataHandler.instance.RecordActivitySolved (userSubmittedCorrectAnswer, answer);
			
				StudentsDataHandler.instance.SaveActivityDataAndClearForNext (currProblem.TargetWord (false), currProblem.InitialWord);


      
		}

		public void RecordUsersChange (int position, char change)
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



}
