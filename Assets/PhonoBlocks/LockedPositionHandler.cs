using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockedPositionHandler : PhonoBlocksController
{
		HashSet<int> lockedPositions;
		int numLockedPositionsWithIncorrectLetter;
		StudentActivityController studentActivityController;

		public void Initialize ()
		{
				
				studentActivityController = gameObject.GetComponent<StudentActivityController> ();
				
		}

		public void ResetForNewProblem ()
		{
				lockedPositions = new HashSet<int> ();
				
			

		}

		public int UserMustRemoveAllTheLettersOf {
				get {
						return numLockedPositionsWithIncorrectLetter;
				}
		
				set {
			
						numLockedPositionsWithIncorrectLetter = value;
					
				}
		
		
		}

		public int NumTangibleLettersThatUserMustMatchToLockedUILetters {
				get {
						return numLockedPositionsWithIncorrectLetter;
				}

				set {

						numLockedPositionsWithIncorrectLetter = value;
				}


		}


		//REQUIRES the 2 strings have equal length.
		//(i.e., the "shorter" has had blanks appended to it.
		public void RememberPositionsThatShouldNotBeChanged (string initialLetters, string targetLetters)
		{
				
				//a position is locked iff the char at that position is the same between initial and target letters
				for (int i=0; i<initialLetters.Length; i++) 
						if (initialLetters [i] == targetLetters [i])
								lockedPositions.Add (i);

		}

		public bool IsLocked (int position)
		{

				return lockedPositions.Contains (position);
		}

		public bool HandleChangeToLockedPosition (int position, char change, string targetState, char[] usersMostRecentChanges)
		{
				
				if (StateBeforeChangeWasDesireable (position, targetState, usersMostRecentChanges)) {
						//we have changed something that we should not have 
						if (PreviousStateAndNewLetterAreNotIdentical (position, targetState, change)) //handles a case only really possible with screen interface
				//where I accidentially "remove" a letter that is not there.
								return HandleError (position, change, targetState);
				} else { //state before change was wrong. check if we have pushed it towards the better.
			
						if (StateIsNowAsDesired (change, targetState [position])) {
								//letter that was added was the correct letter.
								return HandleRestorationOfDesireableState (position, change, targetState);
						} else { //state is not as desird.
								/*if (AddedALetter (change))
					//letter that was added was the incorrect letter (but the state was already wrong because blank)
										studentActivityController.UserPlacedALetterTheyShouldNotHavePlaced ();
								else
										studentActivityController.UserRemovedAMisplacedLetter ();
								//otherwise we removed a letter. if we removed a letter and state did not match then we removed a letter that was wrong.
						*/}
				}
				return false;

			

		}

		bool PreviousStateAndNewLetterAreNotIdentical (int positionOfChange, string desiredStateString, char change)
		{
				return change != desiredStateString [positionOfChange];


		}

		public bool AllLockedPositionsAreInCorrectState ()
		{
				return numLockedPositionsWithIncorrectLetter == 0;

		}

		bool HandleRestorationOfDesireableState (int position, char change, string targetLetters)
		{
				numLockedPositionsWithIncorrectLetter--;
				if (UserModifiedLockedEmptySlot (targetLetters, position)) {
						userInputRouter.RemoveLockedLetterImageAtBlankLockedArduinoControlledPosition (position);

				} else 
						userInputRouter.UnlockNthArduinoControlledLetter (position);

				if (numLockedPositionsWithIncorrectLetter == 0) {
					
						return true;
						
				}
				return false;

		}

		bool HandleError (int position, char change, string targetLetters)
		{
				
				numLockedPositionsWithIncorrectLetter++;
				if (UserModifiedLockedEmptySlot (targetLetters, position)) { 
					//used to play a sound, but don't bother any more
					
				} else {
					
						userInputRouter.LockNthArduinoControlledLetter (position);
				}
				if (numLockedPositionsWithIncorrectLetter == 1)
						userInputRouter.ShutDownUI (true);

	
				StudentDataManager.instance.LogEvent ("unproductive_error", change + "", position + "");

				return false;
		}

		bool AddedALetter (char change)
		{
				return change != ' ';

		}

		bool StateIsNowAsDesired (char change, char desiredLetter)
		{
				return LettersAreSameIgnoreCase (change, desiredLetter);
			

		}

		bool StateBeforeChangeWasDesireable (int position, string targetLetters, char[] usersMostRecentChanges)
		{
				
				return LettersAreSameIgnoreCase (targetLetters [position], usersMostRecentChanges [position]);

		}

		bool UserModifiedLockedEmptySlot (string targetLetters, int position)
		{
				return targetLetters [position] == ' ';

		}

		bool LettersAreSameIgnoreCase (char a, char b)
		{
			   
				int ai = (int)a;
				int bi = (int)b;
				return ai == bi || ai + 32 == bi || ai - 32 == bi;


		}




}
