using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
 * session manager needs to instantiate and set up the variables of the SessionParameters component if the mode is student mode
 * 
 * */
public class SessionsDirector : MonoBehaviour
		//change this between students
{

		
		public static ColourCodingScheme colourCodingScheme = new RControlledVowel ();

		public ColourCodingScheme ActiveColourCodingScheme
		{ set {
						colourCodingScheme = value;
				} get {
						return colourCodingScheme;
				} }

		public INTERFACE_TYPE INTERFACE;
 

		public enum INTERFACE_TYPE
		{
				TANGIBLE, 
				SCREEN_KEYBOARD
    }
		;

		public bool IsScreenMode ()
		{
				return INTERFACE == INTERFACE_TYPE.SCREEN_KEYBOARD;

		}

		public static int currentUserSession; //will obtain from player prefs


		public static bool IsTheFirstTutorLedSession ()
		{

				return  currentUserSession == 0;

		}

		static Mode mode; //testing mode. can be student driven (usual phonoblocks practice session, phono reads words), test (assessment) or sandbox

		public static bool DelegateControlToStudentActivityController {
				get {
						return mode == Mode.STUDENT;
				}


		}



		/* also more like a "sandbox" mode; teacher can create whatever words they want */
		public static bool IsTeacherMode {
				get {
						return mode == Mode.TEACHER;
				}
		
		
		}

		public static bool IsStudentMode {
				get {
						return mode == Mode.STUDENT;
				}
		
		
		}

		public GameObject studentActivityControllerOB;
		public GameObject activitySelectionButtons;
		//public GameObject modeSelectionScreen;
		public GameObject teacherModeButton;
		public GameObject studentModeButton;
		public GameObject studentNameInputField;
		public GameObject dataTables;
		InputField studentName;
		public AudioClip noDataForStudentName;
		public AudioClip enterAgainToCreateNewFile;
		public static DateTime assessmentStartTime;



		public enum Mode
		{
				TEACHER,
				STUDENT
			
		}



		void Start ()
		{   
				assessmentStartTime = DateTime.Now;
				activitySelectionButtons.SetActive (false);
				SpeechSoundReference.Initialize ();


				studentName = studentNameInputField.GetComponent<InputField> ();




		}


		//Teacher mode is the current "sandbox" mode, which just defaults to rthe colour scheme chosen at the head of this file.
		//!!TO DO: change startTeacherMode so that the acrtive colour scheme depends upon the button that the teacher pressed.
		void StartTeacherMode ()
		{

				//if it's the first time we pressed the teacher mode button, then activate the content selection buttons and deactivate the student mode button.
				if (mode != Mode.TEACHER) {
						mode = Mode.TEACHER;
						activitySelectionButtons.SetActive(true);
						studentModeButton.SetActive(false);
			           
				} else {
						//if it's the second time we have pressed the teacher mode button, start the activity (...atm, I'm not doing any "checks" on whether the user pressed a buton to change the active scheme, which defaults to open/closed vowel.
						Application.LoadLevel ("Activity");
				}

		}


		void StartPracticeMode ()
		{
				string nameEntered = studentName.stringToEdit.Trim ().ToLower ();
				if (nameEntered.Length > 0) {
		
						nameEntered = CreateNewFileIfNeeded (nameEntered);


						bool wasStoredDataForName = StudentsDataHandler.instance.LoadStudentData (nameEntered);

		
						if (wasStoredDataForName) {
								mode = Mode.STUDENT;
								studentActivityControllerOB = (GameObject)GameObject.Instantiate (studentActivityControllerOB);
			
								SetParametersForStudentMode (studentActivityControllerOB);
								UnityEngine.Object.DontDestroyOnLoad (studentActivityControllerOB);
			
								Application.LoadLevel ("Activity");
						} else {
								AudioSourceController.PushClip (noDataForStudentName);
			
						}
				}
		
		}

		string CreateNewFileIfNeeded (string nameEntered)
		{     
				bool createNewFile = nameEntered [nameEntered.Length - 1] == '*'; //mark new file with asterik
				
				if (createNewFile) {
				
						
						nameEntered = nameEntered.Substring (0, nameEntered.Length - 1);
						
						
						StudentsDataHandler.instance.CreateNewStudentFile (nameEntered);
		
		
		
				}

				return nameEntered;
		}

		public void SetParametersForStudentMode (GameObject studentActivityController)
		{
				currentUserSession = StudentsDataHandler.instance.GetUsersSession ();
			


				ProblemsRepository.instance.Initialize (currentUserSession);
			
				colourCodingScheme = ProblemsRepository.instance.ActiveColourScheme;

				StudentActivityController sc = studentActivityControllerOB.GetComponent<StudentActivityController> ();

		}
}