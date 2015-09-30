using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
 * session manager needs to instantiate and set up the variables of the SessionParameters component if the mode is student mode
 * 
 * */
public class SessionManager : MonoBehaviour
		//change this between students
{

		public static ColourCodingScheme activeColourScheme = new RControlledVowel(); //the active scheme depends on the current session, so we should create a hash map or an array connecting the session number to colour code (if its cyclical we can use modulus)
		public static ColourCodingScheme cachedUserScheme = new RControlledVowel ();
    public INTERFACE_TYPE INTERFACE;
    //public static readonly int TANGIBLE = 1;  //no longer going to be a variable in Min's study but I'll keep the functionality in anyway
    //public static readonly int SCREEN_KEYBOARD = 0;
 

    public enum INTERFACE_TYPE
    {
        TANGIBLE, 
        SCREEN_KEYBOARD
    };

		public bool IsScreenMode ()
		{
				return INTERFACE == INTERFACE_TYPE.SCREEN_KEYBOARD;

		}

		public static void RemoveColours ()
		{
				activeColourScheme = new NoColour ();
				
		}
		/*
		public static void ApplyColours ()
		{
				activeColourScheme = cachedUserScheme;
			
		}*/

		public static readonly int STARTING_INDEX_OF_TEST_SESSIONS = 50; //



		public static int currentUserSession; //will obtain from player prefs


		public static bool IsTheFirstTutorLedSession ()
		{

				return  currentUserSession == 0;

		}

		static Mode mode; //testing mode. can be student driven (usual phonoblocks practice session, phono reads words), test (assessment) or sandbox

		public static bool DelegateControlToStudentActivityController {
				get {
						return mode == Mode.STUDENT || mode == Mode.TEST;
				}


		}

		public static bool IsAssessmentMode {
				get {
						return mode == Mode.TEST;
				}


		}


		/* also more like a "sandbox" mode; teacher can create whatever words they want */
		public static bool IsTeacherMode {
				get {
						return mode == Mode.TEACHER;
				}
		
		
		}

		public static bool IsActivityMode {
				get {
						return mode == Mode.STUDENT;
				}
		
		
		}

		public GameObject studentActivityControllerOB;
		public GameObject contentSelectionScreen;
		public GameObject modeSelectionScreen;
		public GameObject studentNameInputField;
		public GameObject dataTables;
		InputField studentName;
		public AudioClip noDataForStudentName;
		public AudioClip enterAgainToCreateNewFile;
		bool randomizeProblemTypePresentation;
		public static DateTime assessmentStartTime;

		public void Randomize ()
		{
				randomizeProblemTypePresentation = !randomizeProblemTypePresentation;


		}


		public enum Mode
		{
				TEACHER,
				STUDENT,
				TEST
		}



		void Start ()
		{   
				assessmentStartTime = DateTime.Now;
				contentSelectionScreen.SetActive (false);
				SpeechSoundReference.Initialize ();


				studentName = studentNameInputField.GetComponent<InputField> ();




		}


		//Teacher mode is the current "sandbox" mode, which just defaults to rthe colour scheme chosen at the head of this file.
		//!!TO DO: change startTeacherMode so that the acrtive colour scheme depends upon the button that the teacher pressed.
		void StartTeacherMode ()
		{
				mode = Mode.TEACHER;

				Application.LoadLevel ("Activity");

		}

		void StartAssessmentMode ()
		{
				string nameEntered = studentName.stringToEdit.Trim ().ToLower ();
				nameEntered = CreateNewFileIfNeeded (nameEntered);
			
				string nameEnteredTest = nameEntered + StudentDataManager.instance.ASSESSMENT_EXTENSION;
				bool wasStoredDataForName = StudentDataManager.instance.LoadStudentData (nameEnteredTest);
				if (wasStoredDataForName) {
						mode = Mode.TEST;
						studentActivityControllerOB = (GameObject)GameObject.Instantiate (studentActivityControllerOB);
	
						SetParametersForExperimentalSession (studentActivityControllerOB);
						UnityEngine.Object.DontDestroyOnLoad (studentActivityControllerOB);
			
						Application.LoadLevel ("Activity");
				} else {
		
						AudioSourceController.PushClip (noDataForStudentName);
						
					
			
				}

		}

		void StartStudentMode ()
		{
				string nameEntered = studentName.stringToEdit.Trim ().ToLower ();
		
				nameEntered = CreateNewFileIfNeeded (nameEntered);


				bool wasStoredDataForName = StudentDataManager.instance.LoadStudentData (nameEntered);



				if (wasStoredDataForName) {
						mode = Mode.STUDENT;
						studentActivityControllerOB = (GameObject)GameObject.Instantiate (studentActivityControllerOB);
			
						SetParametersForExperimentalSession (studentActivityControllerOB);
						UnityEngine.Object.DontDestroyOnLoad (studentActivityControllerOB);
			
						Application.LoadLevel ("Activity");
				} else {
						AudioSourceController.PushClip (noDataForStudentName);
			
				}
		
		}

		string CreateNewFileIfNeeded (string nameEntered)
		{
				bool createNewFile = nameEntered [nameEntered.Length - 1] == '*'; //mark new file with asterik
				
				if (createNewFile) {
				
						
						nameEntered = nameEntered.Substring (0, nameEntered.Length - 1);
						
						
						StudentDataManager.instance.CreateNewStudentFile (nameEntered);
		
		
		
				}

				return nameEntered;
		}

		void ShowContentSelectionScreen ()
		{
				contentSelectionScreen.SetActive (true);
				modeSelectionScreen.SetActive (false);

		}

		public void SetParametersForExperimentalSession (GameObject studentActivityController)
		{
				currentUserSession = StudentDataManager.instance.GetUsersSession ();
				//Debug.Log ("Current Session Is " + currentUserSession);


				ProblemManager.instance.Initialize (currentUserSession);
			
				cachedUserScheme = ProblemManager.instance.ActiveColourScheme;

				StudentActivityController sc = studentActivityControllerOB.GetComponent<StudentActivityController> ();

		}
}