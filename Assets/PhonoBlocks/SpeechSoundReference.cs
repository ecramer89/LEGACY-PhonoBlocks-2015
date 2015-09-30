using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpeechSoundReference : MonoBehaviour
{

		public static readonly int MAX_BLEND_LENGTH = 3;
		public static readonly int MAX_STABLE_SYLLABLE_LENGTH = 4;
		public static readonly int MIN_STABLE_SYLLABLE_LENGTH = 3;
		public static readonly int NOT_A_BLEND = -1;
		public static readonly int MIDDLE_BLEND = 0;
		public static readonly int FINAL_BLEND = 2;
		public static readonly int INITIAL_BLEND = 3;
		static SpeechSoundLookup<char> vowels_ = new SpeechSoundLookup<char> ();
		static SpeechSoundLookup<char> consonants_ = new SpeechSoundLookup<char> ();
		static SpeechSoundLookup<string> consonant_digraphs_ = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> vowel_digraphs_ = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> stable_syllables_ = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> special_units = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> initial_blends_ = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> middle_blends_ = new SpeechSoundLookup<string> ();
		static SpeechSoundLookup<string> final_blends_ = new SpeechSoundLookup<string> ();
		static PhonotacticsManager.Phonotactics[] blankRules = new PhonotacticsManager.Phonotactics[]{PhonotacticsManager.NoRestrictions};

		public static PhonotacticsManager.Phonotactics[] BlankRules ()
		{
				return blankRules;
		}

		public class SpeechSoundLookup<Type>
		{

				Dictionary<Type, PhonotacticsManager.Phonotactics[]> rules = new Dictionary<Type,PhonotacticsManager.Phonotactics[]> ();

				public void Add (Type letters, PhonotacticsManager.Phonotactics rule)
				{
						this.rules.Add (letters, new PhonotacticsManager.Phonotactics[]{rule});



				}

				public void Add (Type letters, PhonotacticsManager.Phonotactics a, PhonotacticsManager.Phonotactics b)
				{
						this.rules.Add (letters, new PhonotacticsManager.Phonotactics[]{a,b});
		
				}

				public void Add (Type letters, PhonotacticsManager.Phonotactics a, PhonotacticsManager.Phonotactics b, PhonotacticsManager.Phonotactics c)
				{
						this.rules.Add (letters, new PhonotacticsManager.Phonotactics[]{a,b,});
			
				}

				public void Add (Type letters, PhonotacticsManager.Phonotactics[] rules)
				{
						this.rules.Add (letters, rules);
			
			
			
				}

				public bool Contains (Type letters)
				{

						return rules.ContainsKey (letters);

				}

				public PhonotacticsManager.Phonotactics[] TryGetValue (Type key)
				{
						PhonotacticsManager.Phonotactics[] val = null;
						rules.TryGetValue (key, out val);
						return val;

				}

				public Dictionary<Type,PhonotacticsManager.Phonotactics[]>.KeyCollection Units ()
				{
						return rules.Keys;

				}

		}


	   

		public static PhonotacticsManager.Phonotactics[] GetRulesForConsonant (char consonant)
		{

				return consonants_.TryGetValue (consonant);

		}

		public static PhonotacticsManager.Phonotactics[] GetRulesForVowel (char vowel)
		{
				if (IsY (vowel))
						return consonants_.TryGetValue (vowel);
				return vowels_.TryGetValue (vowel);
		
		}

		public static PhonotacticsManager.Phonotactics[] GetRulesForBlend (string blend, int type)
		{
				if (type == INITIAL_BLEND)
						return initial_blends_.TryGetValue (blend);
				if (type == FINAL_BLEND)
						return final_blends_.TryGetValue (blend);
				return middle_blends_.TryGetValue (blend);
		
		}

		public static PhonotacticsManager.Phonotactics[] GetRulesForConsonantDigraph (string cDigraph)
		{
		
				return consonant_digraphs_.TryGetValue (cDigraph);
		
		}

		public static PhonotacticsManager.Phonotactics[] GetRulesForVowelDigraph (string vDigraph)
		{
		
				return vowel_digraphs_.TryGetValue (vDigraph);
		
		}

		public static PhonotacticsManager.Phonotactics[] GetRulesForStableSyllable (string stable)
		{
		
				return stable_syllables_.TryGetValue (stable);
		
		}






		//set of "can be doubled". if we find a final stray consonant and its not the end of the word, then check and see if the neighbor of that consonant
		//is the same as itself (only in one direction? or both?)
		//and then if yes, check to see if it is in the set that can be doubled (or, you could make it so we provide another phonotactics rule--
		//can be doubled? (how would we know which??)
		//ss could possibly-- cache this all in some kin dof data structure, instead of using strings.
		//but for now a query of a set would suffice.


	     
		//we need another category of special units that aren't consonants blend, (have a silent e), 
		//and arent stable syllables, but have the ability to made the vowel sound short.
		//I don't think the tutors will want dge to take on the blend colour. (it should be- 
		//take on the colors of consonants....
		static bool initialized;

		public static void Initialize ()
		{

				AddVowels ();
				AddConsonants ();
				AddBlends ();
				AddConsonantDigraphs ();
				AddVowelDigraphs ();
				AddStableSyllables ();
				AddSpecialUnits ();


				initialized = true;

		}

		static void AddVowels ()
		{
				
				vowels_.Add ('a', PhonotacticsManager.NoRestrictions);
				vowels_.Add ('e', PhonotacticsManager.NoRestrictions);
				vowels_.Add ('i', PhonotacticsManager.NoRestrictions);
				vowels_.Add ('o', PhonotacticsManager.NoRestrictions);
				vowels_.Add ('u', PhonotacticsManager.NoRestrictions);
				

		}

		static void AddConsonants ()
		{
	
				consonants_.Add ('b', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('c', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('d', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('f', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('g', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('h', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('j', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('k', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('l', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('m', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('n', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('p', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('q', PhonotacticsManager.Q, PhonotacticsManager.NoRestrictions);
				consonants_.Add ('r', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('s', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('t', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('v', PhonotacticsManager.CannotBeLast, PhonotacticsManager.NoRestrictions);
				consonants_.Add ('w', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('x', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('y', PhonotacticsManager.NoRestrictions);
				consonants_.Add ('z', PhonotacticsManager.NoRestrictions);

		}

		static void AddBlends ()
		{    
				middle_blends_.Add ("st", PhonotacticsManager.NoRestrictions);
				middle_blends_.Add ("sp", PhonotacticsManager.NoRestrictions);
				middle_blends_.Add ("ll", PhonotacticsManager.NoRestrictions);

				initial_blends_.Add ("sl", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("bl", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("gl", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("cl", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("pl", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("fl", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("cr", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("tr", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
		
				initial_blends_.Add ("dr", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
		

				final_blends_.Add ("ft", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeFirst);

				final_blends_.Add ("nd", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeFirst);
				final_blends_.Add ("sk", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeFirst);
				final_blends_.Add ("mp", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeFirst);
				final_blends_.Add ("nt", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeFirst);
	
				initial_blends_.Add ("str", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("spr", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("scr", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("spl", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("squ", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("shr", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
				initial_blends_.Add ("thr", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);

	

		}

		static void AddConsonantDigraphs ()
		{
				consonant_digraphs_.Add ("th", PhonotacticsManager.NoRestrictions);
				consonant_digraphs_.Add ("qu", PhonotacticsManager.NoRestrictions);
				consonant_digraphs_.Add ("ck", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeFirst);
				consonant_digraphs_.Add ("ch", PhonotacticsManager.NoRestrictions);
				consonant_digraphs_.Add ("ng", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeFirst);
				consonant_digraphs_.Add ("nk", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeFirst);
				consonant_digraphs_.Add ("sh", PhonotacticsManager.NoRestrictions);
				consonant_digraphs_.Add ("wh", PhonotacticsManager.NoRestrictions, PhonotacticsManager.CannotBeLast);
		}
	
		static void AddVowelDigraphs ()
		{
				vowel_digraphs_.Add ("ea", PhonotacticsManager.NoRestrictions);
				vowel_digraphs_.Add ("ae", PhonotacticsManager.NoRestrictions);
				vowel_digraphs_.Add ("aa", PhonotacticsManager.NoRestrictions);
				vowel_digraphs_.Add ("ee", PhonotacticsManager.NoRestrictions);
				vowel_digraphs_.Add ("ie", PhonotacticsManager.NoRestrictions);
				vowel_digraphs_.Add ("oe", PhonotacticsManager.NoRestrictions);
				vowel_digraphs_.Add ("ue", PhonotacticsManager.NoRestrictions);
				vowel_digraphs_.Add ("ou", PhonotacticsManager.NoRestrictions);
				vowel_digraphs_.Add ("ay", PhonotacticsManager.NoRestrictions);
				vowel_digraphs_.Add ("oa", PhonotacticsManager.NoRestrictions);
		}

		static void AddStableSyllables ()
		{       

				
				stable_syllables_.Add ("ckle", PhonotacticsManager.ConsonantCannotPrecede);
				stable_syllables_.Add ("kle", PhonotacticsManager.ConsonantCannotPrecede);
				stable_syllables_.Add ("gle", PhonotacticsManager.ConsonantCannotPrecede);
				stable_syllables_.Add ("fle", PhonotacticsManager.ConsonantCannotPrecede);
				stable_syllables_.Add ("ble", PhonotacticsManager.ConsonantCannotPrecedeUnlessDoubled);
				stable_syllables_.Add ("dle", PhonotacticsManager.ConsonantCannotPrecedeUnlessDoubled);
				stable_syllables_.Add ("ple", PhonotacticsManager.ConsonantCannotPrecedeUnlessDoubled);
				stable_syllables_.Add ("tle", PhonotacticsManager.ConsonantCannotPrecedeUnlessDoubled);


		}


		//the rule for special units is that they query the 
		//concept to see if they should be coloured especially.
		//if not, then each component just takes whatever colour it has for being vowel, consonant, silent, etc.
		static void AddSpecialUnits ()
		{       
				special_units.Add ("dge", PhonotacticsManager.CannotBeFirst, PhonotacticsManager.ConsonantCannotPrecede);
				


		}

		public static bool IsStableSyllable (string candidate)
		{
				if (candidate.Length > MAX_STABLE_SYLLABLE_LENGTH || candidate.Length < MIN_STABLE_SYLLABLE_LENGTH)
						return false;
		
				if (!initialized)
						Initialize ();
				return stable_syllables_.Contains (candidate);
		
		
		
		}
	
		public static int IsBlendAndWhichType (string candidate)
		{
				if (candidate.Length > MAX_BLEND_LENGTH)
						return NOT_A_BLEND;

				if (!initialized)
						Initialize ();
				if (middle_blends_.Contains (candidate))
						return MIDDLE_BLEND;
				if (initial_blends_.Contains (candidate))
						return INITIAL_BLEND;
				if (final_blends_.Contains (candidate))
						return FINAL_BLEND;

				return NOT_A_BLEND;
		
		
		
		}

		public static bool IsConsonantDigraph (string candidate)
		{
				if (candidate.Length > 2)
						return false;
		
				if (!initialized)
						Initialize ();
				return consonant_digraphs_.Contains (candidate);

		}
	
		public static bool IsVowelDigraph (string candidate)
		{
				if (candidate.Length > 2)
						return false;
		
				if (!initialized)
						Initialize ();
				return vowel_digraphs_.Contains (candidate);

		}

		public static bool IsVowel (char candidate)
		{
				if (!initialized)
						Initialize ();
				return  vowels_.Contains (candidate);
		
		
		
		
		}

		public static bool IsY (char letter)
		{
				return letter == 'y';
		}

		public static bool IsConsonant (char candidate)
		{
				if (!initialized)
						Initialize ();
				return consonants_.Contains (candidate);

		}

		public static Dictionary<char,PhonotacticsManager.Phonotactics[]>.KeyCollection Vowels ()
		{	
				return vowels_.Units ();
		
		
		}

		public static Dictionary<char,PhonotacticsManager.Phonotactics[]>.KeyCollection Consonants ()
		{	
				return consonants_.Units ();
		
		
		}

}
