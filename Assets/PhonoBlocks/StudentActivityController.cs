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
			
		}

		State state = State.PLACE_INITIAL_LETTERS;
		LockedPositionHandler lockedPositionHandler;
		HintController hintController;
		ArduinoLetterController arduinoLetterController;
		Problem currProblem;
		public AudioClip correctOnFirstTryFeedBack;
		char[] usersMostRecentChanges;
		public AudioClip incorrectFeedback;
		public AudioClip sessionIsFinished;
		public Texture2D sessionFinishedImage;
		public AudioClip correctFeedback;

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

		public void Initialize (GameObject hintButton, ArduinoLetterController arduinoLetterController)
		{
				this.arduinoLetterController = arduinoLetterController;
				usersMostRecentChanges = new char[UserInputRouter.numOnscreenLetterSpaces];
			
				lockedPositionHandler = gameObject.GetComponent<LockedPositionHandler> ();
			
				hintController = gameObject.GetComponent<HintController> ();
				hintController.Initialize (hintButton);
			
				SetUpNextProblem ();
				InteractiveLetter.LetterSelectedDeSelected += LetterSelectDeselect;

	
		}


		//impose "reflection"- if the child waits three seconds after swiping,
		//then check the word
		//and if it works
		//colour pink and play sound
		int selectTimer = -1;

		public void LetterSelectDeselect (bool wasSelected, GameObject selectedLetter)
		{     
				selectTimer = 60 * 3; 

		}

		bool wholeWordIsColoured = true;

		void Update ()
		{
				if (selectTimer > 0)
						selectTimer--;
				if (selectTimer == 0) {
						string selectedletters = arduinoLetterController.SelectedUserControlledLettersAsString.Trim ();
						bool matchesTargetWord = selectedletters.Equals (currProblem.TargetWord (true));
						if (matchesTargetWord) {
								currProblem.PlayTargetWord ();
								wholeWordIsColoured = true;
								for (int i=0; i<selectedletters.Length; i++)
										arduinoLetterController.ChangeDisplayColourOfASingleCell (i, SessionsDirector.colourCodingScheme.GetColorsForWholeWord ());
						} else if (wholeWordIsColoured) {
								for (int i=0; i<selectedletters.Length; i++) {
										arduinoLetterController.RevertASingleLetterToDefaultColour (i);
								}
								wholeWordIsColoured = false;
				
						}
			        
						selectTimer = -1;
				}


		}

		public void SetUpNextProblem ()
		{  

				//get the next specific problem from the ProblemType class
				ClearSavedUserChanges ();
				hintController.Reset ();
			
				currProblem = ProblemsRepository.instance.GetNextProblem ();
	
				StudentsDataHandler.instance.RecordActivityTargetWord (currProblem.TargetWord (false));


				lockedPositionHandler.ResetForNewProblem ();
				lockedPositionHandler.RememberPositionsThatShouldNotBeChanged (currProblem.InitialWord, currProblem.TargetWord (false).Trim ()); 
	
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
					
				}


		}

		bool CurrentStateOfLettersMatches (string targetLetters)
		{       
				for (int i=0; i<usersMostRecentChanges.Length; i++) {
						if (i >= targetLetters.Length) {
								if (usersMostRecentChanges [i] != ' ')
										return false;
						} else if (usersMostRecentChanges [i] != targetLetters [i])
								return false;
				}
				return true;

		}

		public void ChangeProblemStateIfAllLockedPositionsAHaveCorrectCharacter ()
		{
			
				if (state == State.PLACE_INITIAL_LETTERS) {
						if (CurrentStateOfLettersMatches (currProblem.InitialWord))
								BeginMainProblemState ();
				
				}
				if (state == State.REMOVE_ALL_LETTERS) {
					
						if (CurrentStateOfLettersMatches (currProblem.TargetWord (false))) {
							
								HandleEndOfActivity ();
						}
					
				}
						
						

				

		}

		void HandleEndOfActivity ()
		{
				if (ProblemsRepository.instance.AllProblemsDone ()) {
						StudentsDataHandler.instance.UpdateUserSessionAndWriteAllUpdatedDataToPlayerPrefs ();
						AudioSourceController.PushClip (sessionIsFinished);
					
				
				} else
						SetUpNextProblem ();

		}

		void BeginMainProblemState ()
		{
				arduinoLetterController.UnLockAllLetters (); //we do this to unlock the letters that are outside the range of the initial word.
				//during the initial stage only the letters in the initial word will absolutely be locked.
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
	
		public void HandleNewArduinoLetter (char letter, int atPosition)
		{       
				if (LetterIsActuallyNew (letter, atPosition)) {
						bool positionWasLocked = lockedPositionHandler.IsLocked (atPosition); 
						//we treat all positions as "locked" when the state is the end of the activity.
						if (positionWasLocked || state == State.REMOVE_ALL_LETTERS) {
								lockedPositionHandler.HandleChangeToLockedPosition (atPosition, letter, currProblem.TargetWord (false), usersMostRecentChanges, arduinoLetterController);
						}

						if (state == State.PLACE_INITIAL_LETTERS)
								arduinoLetterController.UnLockASingleLetter (atPosition);

						RecordUsersChange (atPosition, letter); 
		        
						ChangeProblemStateIfAllLockedPositionsAHaveCorrectCharacter ();
			   
						arduinoLetterController.UpdateDefaultColoursAndSoundsOfLetters (state != State.PLACE_INITIAL_LETTERS);

				}
		}

		/// <summary>
		/// Letters the is actually new.
		/// </summary>
		/// <returns><c>true</c>, if the user actually changed the letter, <c>false</c> otherwise, i.e., if we're reading a "new value" from an arduino circuit or input error.</returns>
		/// <param name="letter">Letter.</param>
		/// <param name="atPosition">At position.</param>
		bool LetterIsActuallyNew (char letter, int atPosition)
		{
				return usersMostRecentChanges [atPosition] != letter;


		}

		bool PositionIsOutsideBoundsOfTargetWord (int wordRelativeIndex)
		{
				return wordRelativeIndex >= currProblem.TargetWord (true).Length; 
		}

		public virtual void HandleSubmittedAnswer (string answer)
		{
				currProblem.IncrementTimesAttempted ();
	
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
			

				state = State.REMOVE_ALL_LETTERS;

				currProblem.SetTargetWordToEmpty ();
				userInputRouter.AddCurrentWordToHistory (false);
				arduinoLetterController.LockAllLetters ();
		   
				userInputRouter.RequestDisplayImage (currProblem.TargetWord (true), false, true);

				bool solvedOnFirstTry = currProblem.TimesAttempted == 1;
				if (solvedOnFirstTry) {
						AudioSourceController.PushClip (correctOnFirstTryFeedBack);
						userInputRouter.DisplayNewStarOnScreen ();

				}


				StudentsDataHandler.instance.RecordActivitySolved (userSubmittedCorrectAnswer, answer, solvedOnFirstTry);
			
				StudentsDataHandler.instance.SaveActivityDataAndClearForNext (currProblem.TargetWord (false), currProblem.InitialWord);


      
		}

		public void RecordUsersChange (int position, char change)
		{
		
				usersMostRecentChanges [position] = change;

		
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
