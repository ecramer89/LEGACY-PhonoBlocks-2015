using UnityEngine;
using System.Collections;

public abstract class ColourCodingScheme: MonoBehaviour
{

		//requirement for all activities:
		//establish colours for all generally relevant LetterSoundComponents.
		//we can do so by adjusting the singleton (LetterSoundComponent Color Map)


		public string label;
		public Color orange; /* note: the values in rthe colour32 objects are */
		public Color purple;

		public ColourCodingScheme ()
		{
				float max = (float)255;
				orange = Color.black;
				orange.r = (float)250 / max;
				orange.g = (float)150 / max;
				orange.b = (float)25 / max;
				purple = Color.black; 
				purple.r = (float)127 / max;
				purple.g = (float)42 / max;
				purple.b = (float)242 / max;
		}
		
		public virtual Color GetColorsForLongVowel (char vowel)
		{
				return Color.red;
		
		}

		public virtual Color GetColorsForShortVowel (Color currentVowelColor)
		{
        return Color.yellow;
		}

		protected float AdjustChannelBy (float colorChannelValue, float by)
		{
				return colorChannelValue + by;
		}
	
		public virtual Color GetColorsForInitialBlends ()
		{
				Color c = Color.green;
				c.r = AdjustChannelBy (c.r, -.8f);
				c.g = AdjustChannelBy (c.g, -.8f);
				c.b = AdjustChannelBy (c.b, -.8f);
				return c;
		}

		public virtual Color GetColorsForFinalBlends ()
		{
				return Color.red;
		}

		public virtual Color GetColorsForMiddleBlends ()
		{
				return Color.cyan;
		}
	
		public virtual Color GetColorsForStableSyllables ()
		{
				return Color.red;
	
		}
	
		public virtual Color GetColorsForHardConsonant ()
		{
				return Color.white;
		}

		public virtual Color ModifyColorForSoftConsonant (Color currentConsonantColor)
		{
				currentConsonantColor.r = AdjustChannelBy (currentConsonantColor.r, .8f);
				currentConsonantColor.g = AdjustChannelBy (currentConsonantColor.g, .8f);
				currentConsonantColor.b = AdjustChannelBy (currentConsonantColor.b, .8f);
				return currentConsonantColor;
		}
	
		public virtual Color GetColorsForConsonantDigraphs ()
		{
			
				return Color.green;
				
		}

		public virtual Color GetColorsForVowelDigraphs ()
		{
				return orange;
		}

		public virtual Color GetColorsForRControlledVowel (char vowel)
		{
				return purple;
		}

		public virtual Color GetColourForSilent (Color currentColor)
		{
				return Color.black;
				//return new Color (AdjustChannelBy (currentColor.r, -.5f), AdjustChannelBy (currentColor.g, -.5f), AdjustChannelBy (currentColor.b, -.5f));
		}

		public virtual Color GetErrorColor ()
		{
				return Color.black;
		}

		public virtual Color GetColorsForPrefixes ()
		{
				return Color.green;

		}

		public virtual Color GetColorsForSuffixes ()
		{
		
				return Color.red;
		}
	
	
		//some concepts attempt to draw kids attention to special letters.
		//often, however, it is not just that a letter is a certain kind,
		//but also that it appears in a particular location.
		//the first step is:
		//is this letter special?
		//and if so
		//return colour for a special letter.


		//we need to get the context and the position of this letter again

		public virtual Color GetColorForRelevantLetterSoundComponent (LetterSoundComponent l)
		{
				return l.Color;
		}
	
		public virtual bool IsLetterSoundComponentRelevant (LetterSoundComponent l)
		{
				return false;
		}
	
}


//Colour coding schemes for Min's Study:


//changes:
//-don't highlight consonant-le unit
//silent letters (silent e) are just as they should be, given the letter's identity as vowel or consonant
class OpenClosedVowel : Categorical
{

		public OpenClosedVowel () : base()
		{
				label = "openClosedVowel";
		}

		public override Color GetColourForSilent (Color currentColor)
		{
				return currentColor;
		}



}

//changes to parent
//colour silent e black
class VowelInfluenceERule : OpenClosedVowel
{
	
		public VowelInfluenceERule () : base()
		{
				label = "vowelInfluenceE";
		}

		public override Color GetColourForSilent (Color currentColor)
		{
				return Color.black;
		}

	
}


//changes to parent
//colour consonant digraphs green
class ConsonantDigraphs : OpenClosedVowel
{
	
		public ConsonantDigraphs () : base()
		{
				label = "consonantDigraphs";
		}

		public override Color GetColorsForConsonantDigraphs ()
		{
				return Color.green;
		}

		public override Color GetColorsForMiddleBlends ()
		{
				return GetColorsForConsonantDigraphs ();
		}

		public override Color GetColorsForFinalBlends ()
		{
				return GetColorsForConsonantDigraphs ();
		}

		public override Color GetColorsForInitialBlends ()
		{
				return GetColorsForConsonantDigraphs ();
		}


    public override Color GetColorsForHardConsonant()
    {
        return Color.cyan;
    }



}


//changes to parent
//colour r controlled vowels purple
class RControlledVowel: ConsonantDigraphs
{
		public RControlledVowel () : base()
		{
				label = "rControlledVowel";
		}

		public override Color GetColorsForVowelDigraphs ()
		{
				return Color.black;
		}

		public override Color GetColorsForRControlledVowel (char vowel)
		{   
				return Color.magenta;
		}

    public override Color GetColorsForLongVowel(char vowel)
    {
        return Color.black;
    }

    public override Color GetColorsForShortVowel(Color currentVowelColor)
    {
        return Color.black;
    }


}

//changes to parent
//colour vowel digraphs orange
class SyllableDivision : ConsonantDigraphs
{
		public SyllableDivision () : base()
		{
				label = "rsyllableDivision";
		}

		public override Color GetColorsForVowelDigraphs ()
		{
				return Color.red;
		}
    


}

public class SoundBased : ColourCodingScheme
{

		Color a = Color.red;
		Color i = Color.yellow;
		Color o = Color.blue;
		Color u = Color.cyan;
		Color e = Color.green;

		public SoundBased () : base()
		{
				label = "sounds";
		}
	
		public override Color GetColorsForLongVowel (char vowel)
		{      
				switch (vowel) {
				case 'a':
						return a;
				case 'e':
						return e;
				case 'i':
						return i;
				case 'u':
						return u;
				case 'o':
						return o;
				}
			
				return Color.white; 
		
		}

		public override Color GetColorsForRControlledVowel (char vowel)
		{
		
				return Color.magenta;

		}
	
		public override Color GetColorsForShortVowel (Color currentVowelColor)
		{
				return currentVowelColor;
	
		}

		public override Color GetColorsForHardConsonant ()
		{
				return Color.white;
		}
	
		public override Color GetColorsForInitialBlends ()
		{
				return GetColorsForHardConsonant ();
	
		}
	
		public override Color GetColorsForMiddleBlends ()
		{
				return GetColorsForHardConsonant ();
		}

		public override Color GetColorsForFinalBlends ()
		{
				return GetColorsForHardConsonant ();
	
		}

		public override Color GetColorsForStableSyllables ()
		{
				return Color.magenta;

		}
	
		public override Color ModifyColorForSoftConsonant (Color color)
		{
				return GetColorsForHardConsonant ();
		}
		/*
		public override Color GetColorsForConsonantDigraphs ()
		{
				return GetColorsForHardConsonant ();
		}*/
	

	
	
}

public class Categorical : ColourCodingScheme
{
		public Categorical () : base()
		{
				label = "categorical";
		}
	
		public override Color GetColorsForLongVowel (char vowel)
		{
				return Color.red;
		
		}
	
		public override Color GetColorsForShortVowel (Color currentVowelColor)
		{
				return Color.yellow;
		}

		public override Color GetColorsForInitialBlends ()
		{
				return GetColorsForHardConsonant ();
		}
	
		public override Color GetColorsForMiddleBlends ()
		{
				return GetColorsForHardConsonant ();
		}

		public override Color GetColorsForFinalBlends ()
		{
				return GetColorsForHardConsonant ();
		}
	
		public override Color GetColorsForStableSyllables ()
		{
				return Color.magenta;
		}
	
		public override Color ModifyColorForSoftConsonant (Color color)
		{
				return GetColorsForHardConsonant ();
		}
		
		public override Color GetColorsForConsonantDigraphs ()
		{
				return GetColorsForHardConsonant ();
		}

		public override Color GetColorsForRControlledVowel (char vowel)
		{
				return GetColorsForLongVowel (vowel);
		}

		public virtual Color GetColourForSilent (Color currentColor)
		{
				return currentColor;
		}

	
	
	
	
}





//different activities can "opt out" of certain distinctions, and add new ones.
public class NoColour : ColourCodingScheme
{

		public NoColour () : base()
		{
				label = "control";
		}
	   
		public override Color GetColorsForLongVowel (char vowel)
		{
				return Color.white;
		
		}
	
		public override Color GetColorsForShortVowel (Color currentVowelColor)
		{
				return Color.white;
		}

		public override Color GetColorsForInitialBlends ()
		{
				return Color.white;
		}

		public override Color GetColorsForMiddleBlends ()
		{
				return Color.white;
		}

		public override Color GetColorsForFinalBlends ()
		{
				return Color.white;
		}
	
		public override Color GetColorsForStableSyllables ()
		{
				return Color.white;
		}
		
		public override Color ModifyColorForSoftConsonant (Color color)
		{
				return Color.white;
		}
		/*
		public override Color GetColorsForConsonantDigraphs ()
		{
				return Color.white;
		}*/

		public override Color GetColorsForVowelDigraphs ()
		{
				return Color.white;
		}

		public override Color GetColorsForRControlledVowel (char letter)
		{

				return Color.white;
		}


}

class Nick : Categorical
{

		public Nick () : base()
		{
				label = "Nick";
		}

		public override Color GetColorsForStableSyllables ()
		{
				return Color.green;
		}

		public override Color GetColorsForShortVowel (Color currentVowelColor)
		{
				return Color.blue;
		}

		public override Color GetColorsForLongVowel (char vowel)
		{
				return Color.yellow;
		
		}









}