using UnityEngine;
using System.Collections;

public abstract class ColourCodingScheme: MonoBehaviour
{

		//requirement for all activities:
		//establish colours for all generally relevant LetterSoundComponents.
		//we can do so by adjusting the singleton (LetterSoundComponent Color Map)


		public string label;
		protected int alternate = 0;

		public virtual Color GetColorsForWholeWord ()
		{
				return Color.magenta;
		
		}
	
		public virtual Color GetColorsForLongVowel (char vowel)
		{
				return Color.white;
		
		}

		public virtual Color GetColorsForHardConsonant (int indexInWord=0)
		{
				return Color.white;
		}

		public virtual Color GetColourForSilent (char letter)
		{
				return Color.white;
		}
	
		public virtual Color GetColorsForShortVowel (Color currentVowelColor)
		{
				return Color.white;
		}
	
		public virtual Color GetColorsForInitialBlends (int letterInBlendPos=0)
		{
				return Color.white;
		}
	
		public virtual Color GetColorsForMiddleBlends (int letterInBlendPos=0)
		{
				return Color.white;
		}
	
		public virtual Color GetColorsForFinalBlends (int letterInBlendPos=0)
		{
				return Color.white;
		}
	
		public virtual Color GetColoursForSyllables (int indexInWord=0)
		{
				return Color.white;
		}
	
		public virtual Color ModifyColorForSoftConsonant (Color color)
		{
				return Color.white;
		}
	
		public virtual Color GetColorsForConsonantDigraphs ()
		{
				return Color.white;
		}
	
		public virtual Color GetColorsForVowelDigraphs ()
		{
				return Color.white;
		}
	
		public virtual Color GetColorsForRControlledVowel ()
		{
		
				return Color.white;
		}

		protected virtual Color AlternatingColours (int alternate)
		{
				//we basically distinguish between first and not first, at least for consonants and syllables.
				if (alternate == 0)
						return GetAlternateA ();
				else
						return GetAlternateB ();
				

		}

		protected virtual Color GetAlternateA ()
		{
				return Color.cyan;
		}

		protected virtual Color GetAlternateB ()
		{
				return Color.green;
		}

	
}


//Colour coding schemes for Min's Study:

class ConsonantBlends : NoColour
{

		public ConsonantBlends (): base()
		{

				label = "Blends";

		}

		public override Color GetColorsForInitialBlends (int letterInBlendPos=0)
		{
				
				return (letterInBlendPos == 0 ? Color.cyan : Color.green);
		}
	
		public override Color GetColorsForMiddleBlends (int letterInBlendPos=0)
		{

				return GetColorsForInitialBlends (letterInBlendPos);
		}
	
		public override Color GetColorsForFinalBlends (int letterInBlendPos=0)
		{
			
				return GetColorsForInitialBlends (letterInBlendPos);
		}




}

class OpenClosedVowel : NoColour
{

		public OpenClosedVowel () : base()
		{
				label = "openClosedVowel";
		}

		public override Color GetColorsForShortVowel (Color currentVowelColor)
		{
				return Color.yellow;
		}

		public override Color GetColorsForLongVowel (char vowel)
		{
				return Color.red;
		}

		/* Min wants each consonant in the word to have a different colour. Most of the words she uses are CVC, so the rule
	     * that we alternate blue and green should work*/
		

		public override Color GetColorsForHardConsonant (int indexInWord=0)
		{
				return AlternatingColours (indexInWord);
		}


}

//changes to parent
//colour silent e black
class VowelInfluenceERule : NoColour
{
	
		public VowelInfluenceERule () : base()
		{
				label = "vowelInfluenceE";
		}

		public override Color GetColorsForLongVowel (char vowel)
		{
				return Color.red;
		}

		public override Color GetColourForSilent (char letter)
		{
				if (letter == 'e')
						return GetColorsForLongVowel ('e'); //silent e colour matches long vowel it influences.
				return GetColorsForHardConsonant ();
		}

	
}


//changes to parent
//colour consonant digraphs green
class ConsonantDigraphs : NoColour
{
	
		public ConsonantDigraphs () : base()
		{
				label = "consonantDigraphs";
		}

		public override Color GetColorsForConsonantDigraphs ()
		{
				return Color.green;
		}

		public override Color GetColorsForHardConsonant (int indexInWord=0)
		{
				return Color.cyan;
		}

		public override Color GetColorsForInitialBlends (int letterInBlendPos=0)
		{
				return GetColorsForHardConsonant ();
		}
	
		public override Color GetColorsForMiddleBlends (int letterInBlendPos=0)
		{
				return GetColorsForHardConsonant ();
		}
	
		public override Color GetColorsForFinalBlends (int letterInBlendPos=0)
		{
				return GetColorsForHardConsonant ();
		}
	

}


//changes to parent
//colour r controlled vowels purple
class RControlledVowel: NoColour
{
		public RControlledVowel () : base()
		{
				label = "rControlledVowel";
		}

		public override Color GetColorsForRControlledVowel ()
		{   
				return Color.magenta;
		}

	

}

//changes to parent
//colour vowel digraphs orange
class VowelDigraphs : NoColour
{
		public VowelDigraphs () : base()
		{
				label = "vowel Digraphs";
		}

		public override Color GetColorsForVowelDigraphs ()
		{
				return Color.red;
		}

		public override Color GetColorsForRControlledVowel ()
		{   
				return GetColorsForShortVowel (Color.black);
		}

		public override Color GetColorsForLongVowel (char vowel)
		{
				return GetColorsForShortVowel (Color.black);
		}

		public override Color GetColorsForShortVowel (Color currentVowelColor)
		{      
				return Color.gray;
		}

    


}

class SyllableDivision : NoColour
{
		public SyllableDivision () : base()
		{
				label = "syllableDivision";
		}

		public virtual Color GetColoursForSyllables (int indexInWord=0)
		{
				return AlternatingColours (indexInWord);
		}
	

	

	
	
}




//different activities can "opt out" of certain distinctions, and add new ones.
public class NoColour : ColourCodingScheme
{

		




}

