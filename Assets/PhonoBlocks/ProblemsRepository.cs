using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;

//Problem Manager-- not the student data manager-- should have the information about what colour coding scheme to use (since now that is associated with the problem type)
public class ProblemsRepository : MonoBehaviour
{   
		Problem[] problemsForSession; //I think if we do go this way, we should change NGram class so that problems aere associated directly with them
		int currProblem = 0;
		public readonly int PROBLEMS_PER_SESSION = 3;
		public readonly int TOTAL_NUMBER_OF_EXPERIMENTAL_PROBLEMS_PER_PROBLEM_TYPE;
		ColourCodingScheme colourSchemeForSession = new NoColour ();
		public static ProblemsRepository instance = new ProblemsRepository ();

		public ColourCodingScheme ActiveColourScheme {
				get {
						return colourSchemeForSession;

				}


		}


		public enum ProblemType
		{
				OPEN_CLOSED_VOWEL,
				MAGIC_E,
				SYLLABLE_DIVISION,
				CONSONANT_DIGRAPHS,
				R_CONTROLLED_VOWEL,
				VOWEL_DIGRAPHS

		}

		public ColourCodingScheme GetColourCodingSchemeGivenProblemType (ProblemType type)
		{
				switch (type) {
				case ProblemType.OPEN_CLOSED_VOWEL:
						return new OpenClosedVowel ();
						break;
				case ProblemType.MAGIC_E:
						return new VowelInfluenceERule ();
						break;
				case ProblemType.SYLLABLE_DIVISION:
						return new SyllableDivision ();
						break;
				case ProblemType.CONSONANT_DIGRAPHS:
						return new ConsonantDigraphs ();
						break;
				case ProblemType.R_CONTROLLED_VOWEL:
						return new RControlledVowel ();
						break;
				case ProblemType.VOWEL_DIGRAPHS:
						return new NoColour (); //change after we make the vowel digraphs scheme
						break;
				default: 
						return new NoColour ();
						break;

				}
		       

		}
	
		static readonly int INITIAL_WORD_IDX = 1;
		static readonly int TARGET_WORD_IDX = 0;
		string[][][] activity_words = {

		new string[][]{
			new string[]{"bet","dad","tin"}, //target words
			new string[]{"b t","d d","t n"} //initial versions of target words
		},

		new string[][]{
			new string[]{"pup","hit","web"},
			new string[]{"p p","h t","w b"}
		},
		
		new string[][]{
			new string[]{"flag","quit","thin"},
			new string[]{"fl","it","in"}
		},
		
		new string[][]{
			new string[]{"trip","drop","crab"},
			new string[]{" "," "," "}
		},

		new string[][]{
			new string[]{"wish","lunch","last"},
			new string[]{" "," "," "}
		},

		new string[][]{
			new string[]{"thing","lift","pill"},
			new string[]{" "," "," "}
		},


		new string[][]{
			new string[]{"game","tape","side"},
			new string[]{" "," "," "}
		},


		new string[][]{
			new string[]{"home","tune","fine"},
			new string[]{" "," "," "}
		},

		new string[][]{
			new string[]{"house","play","sleep"},
			new string[]{" "," "," "}
		},

		new string[][]{
			new string[]{"eat","boat","pie"},
			new string[]{" "," "," "}
		},

	    new string[][]{
		    new string[]{"car","her","stir"},
			new string[]{" "," "," "}
        },

        new string[][]{
			new string[]{"hurt","horn","part"},
			new string[]{" "," "," "}
        }
	
	};

		void Problems (Problem[] problems)
		{
				this.problemsForSession = problems;
				idxToSwapUsedProblemIn = problems.Length - 1;
		}

		int idxToSwapUsedProblemIn;
	
		public void Initialize (int sessionIndex)
		{
				InitializeProblems (sessionIndex);
				SetSessionColourScheme (sessionIndex);

		
		}

		void InitializeProblems (int sessionIndex)
		{

				string[][] wordsForSessionProblems = activity_words [sessionIndex % activity_words.Length];
				problemsForSession = new Problem[PROBLEMS_PER_SESSION];
				
		      
				for (int i=0; i<PROBLEMS_PER_SESSION; i++) {
			        
						problemsForSession [i] = new Problem (wordsForSessionProblems [INITIAL_WORD_IDX] [i], wordsForSessionProblems [TARGET_WORD_IDX] [i]);
				}

		}

		void SetSessionColourScheme (int sessionIndex)
		{
				switch (sessionIndex) {
				case 0:
				case 1:
						colourSchemeForSession = new OpenClosedVowel ();
						return;
				case 2:
				case 3:
				case 6:
				case 7:
						colourSchemeForSession = new ConsonantDigraphs ();
						return;

				case 4:
				case 5:

						colourSchemeForSession = new VowelInfluenceERule ();
						return;
				case 8:
				case 9:
						colourSchemeForSession = new SyllableDivision ();
						return;
				case 10:
				case 11:

						colourSchemeForSession = new RControlledVowel ();
						return;
		
				}
			


		}
		/* only if we want sub directories for words for different types of problems (slightly faster searching)*/
		string GetPathToWord ()
		{
				StringBuilder s = new StringBuilder ("audio/words/");
				/*if (id.Equals (Name.CON_LE))
						s.Append ("conle/");
				else
						s.Append ("voweld/");
			*/
				return s.ToString ();
		}

		public Problem GetNextProblem ()
		{       
				if (!AllProblemsDone ()) {
						Problem result = problemsForSession [currProblem];
						currProblem++;
						return result;
				} else
						return problemsForSession [0];

	
		}

		public bool AllProblemsDone ()
		{
				return currProblem == problemsForSession.Length;

		}

		public Problem GetRandomProblem ()
		{
				if (idxToSwapUsedProblemIn == 0)//loop if necessary
						idxToSwapUsedProblemIn = problemsForSession.Length - 1;

				int upper = idxToSwapUsedProblemIn - 1;
				int randomIdx = (int)UnityEngine.Random.Range (0, upper);
				Problem next = problemsForSession [randomIdx];
				Problem temp = problemsForSession [idxToSwapUsedProblemIn];
				problemsForSession [randomIdx] = temp;
				problemsForSession [idxToSwapUsedProblemIn] = next;
				idxToSwapUsedProblemIn--;
				next.Reset ();
				return next;
				
		}

		


}
