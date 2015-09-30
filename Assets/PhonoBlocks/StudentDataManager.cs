using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;

public class StudentDataManager : MonoBehaviour
{

		static readonly string DATA_FILE_EXTENSION = ".csv";
		static readonly string DATA_FILE_DIRECTORY = "data";
		static readonly string LOG_FILE_DIRECTORY = "logs";
		public readonly string ASSESSMENT_EXTENSION = "_t";
		public static StudentDataManager instance = new StudentDataManager ();

		//static string templateForExperimentWideParams = "[0,0]";
		static string templateForExperimentWideParams = "[0]";
		const string experimentWideParamStart = "[";
		const string experimentWideParamEnd = "]";
		//const int idxOfColorCodeSchemeInTemplate = 1;
		const int idxOfCurrentSessionInTemplate = 1;
		//const int idxOfCurrentSessionInTemplate = 3;
		const string activityDelimiter = "!";

		//these arent indexes of the values in a string. they are idx of the values in the array
		//that we store in the student data object and which we place values into before saving it a single
		//string in player prefs.


		public const int idxOfSolved = 0;

		//keep these the way they are so that we can exploit the hint integer argument to the record
		//requested hint method as the way of accessing the correct array index.
		/*public const int idxOfTimesListenedHint1 = 1;
		public const int idxOfTimesListenedHint2 = 2;
		public const int idxOfTimesListenedHint3 = 3;
		public const int idxOfNumAttempts = 4;*/
		public const int idxOfSkippedActivity = 1;
		const int NUM_NUMERIC_ACTIVITY_FIELDS = idxOfSkippedActivity + 1;
		const string valDelimiter = ",";
		const int asciiFor0 = 48;




		

		public class StudentData
		{
				int currSession;
				//ColourCodingScheme scheme;
				//int schemeAsInt;

				/*public int ColorCodingSchemeAsInt {
						get{ return schemeAsInt;}
				}*/

				public string studentName;
				//each activity finishes with the delimiter character !
				//when you load in the student data at the very end,
				//just split this string into an array by !
				//and then write each string to file as a separate line
				// (the string will already contain the comma delimiters between fields because
				//we will format the string made from the values in the array this way before we save them to
				//the player prefs
				public string dataFromPreviouslyCompletedActivities = "";
				public int[] numericDataForCurrentActivity = new int[NUM_NUMERIC_ACTIVITY_FIELDS];
				public string targetWordOfCurrentActivity;
				public string initialWordOfCurrentActivity;
				public string conceptOfCurrentActivity;
				public string finalSubmittedWord;
				//public string coloursApplied = "true";
				string asString;

				/*
				public ColourCodingScheme ColourCodingScheme {
						get {
								return scheme;
						}
		
			
				}*/

				public int CurrentSession {
						get {
								return currSession;
						}
		
				}

				/*public StudentData (string studentName, int colorCodingScheme, int currSession)
				{
						this.studentName = studentName;
						this.currSession = currSession;
						//this.scheme = colourCodes [colorCodingScheme];
						//this.schemeAsInt = colorCodingScheme;
					
					

				}*/

				public StudentData (string studentName, int currSession)
				{
						this.studentName = studentName;
						this.currSession = currSession;
						//this.scheme = colourCodes [colorCodingScheme];
						//this.schemeAsInt = colorCodingScheme;
			
			
			
				}

				public string ToString ()
				{
						return asString;


				}

				public void AppendDataForCurrentActivityToPreviousData ()
				{

						StringBuilder data = new StringBuilder (dataFromPreviouslyCompletedActivities);
						AppendDatum (data, currSession);
						AppendDatum (data, conceptOfCurrentActivity);
						AppendDatum (data, targetWordOfCurrentActivity);
						AppendDatum (data, initialWordOfCurrentActivity);
						//AppendDatum (data, coloursApplied);
						foreach (int val in numericDataForCurrentActivity) {
								AppendDatum (data, val);
						}
						AppendDatum (data, finalSubmittedWord);
						//append the activity delimiter (!) to the end of the string.
						data.Append (activityDelimiter);
						//overwrite the old string to the one that includes the new data
						dataFromPreviouslyCompletedActivities = data.ToString ();


				}

				void AppendDatum (StringBuilder data, object datum)
				{
						data.Append (datum);
						data.Append (valDelimiter);

				}

				//clear the array that stores the summary numeric activity data.
				public void ClearCurrentActivityData ()
				{

						numericDataForCurrentActivity = new int[NUM_NUMERIC_ACTIVITY_FIELDS];

				}

		}



		static Dictionary<int,StudentData> dummyData;
		static StudentData currUser;

		public void CreateNewStudentFile (string studentName)
		{
				Debug.Log ("Created file for " + studentName);
				PlayerPrefs.SetString (studentName, templateForExperimentWideParams); 
		}





	  
		// writes all of the student's stored data (for each session)
		// to a csv file. Each student receives their own csv file.

		public void WriteDataOfCurrentStudentToCSV ()
		{

				//WriteStudentDataToCSV (currUser.studentName);



		}

		char IntAsChar (int i)
		{
				return (char)(i + asciiFor0);

		}

		int CharAsInt (char c)
		{
				return (int)c - asciiFor0;

		}

		string UpdateSavedSessionAsString (int currentSession)
		{
				StringBuilder s = new StringBuilder (templateForExperimentWideParams);
		
				s [idxOfCurrentSessionInTemplate] = IntAsChar (currentSession);
		
				return s.ToString ();
		
		}

		public bool LoadStudentData (string name)
		{     
				
				string studentData = PlayerPrefs.GetString (name);
			
				bool wasData = studentData != "";
				if (wasData) {
			            
						currUser = ParseStudentData (name, studentData);

				}
				return wasData;
	
		}

		StudentData ParseStudentData (string studentName, string studentData)
		{
				string expWideParams = studentData.Substring (0, templateForExperimentWideParams.Length);
				//int colorCodingScheme = CharAsInt (expWideParams [idxOfColorCodeSchemeInTemplate]);
				int currSession = CharAsInt (expWideParams [idxOfCurrentSessionInTemplate]);
				//StudentData data = new StudentData (studentName, colorCodingScheme, currSession);
				StudentData data = new StudentData (studentName, currSession);
				if (ThereIsDataFromEarlierSessions (studentData))
						data.dataFromPreviouslyCompletedActivities = studentData.Substring (templateForExperimentWideParams.Length, studentData.Length - templateForExperimentWideParams.Length);
				return data;

		}

		public bool ThereIsDataFromEarlierSessions (string studentData)
		{
				return studentData.Length > templateForExperimentWideParams.Length;

		}

		void RecordActivityConcept (string concept)
		{
				currUser.conceptOfCurrentActivity = concept;
		
		}

		public void RecordThatActivityWasSkipped ()
		{
				currUser.numericDataForCurrentActivity [idxOfSkippedActivity] = 1;
		}

		public void RecordActivityTargetWord (string word)
		{
				currUser.targetWordOfCurrentActivity = word;

		}

		void RecordActivityInitialWord (string word)
		{
				currUser.initialWordOfCurrentActivity = word;
		
		}

		public void RecordActivitySolved (bool solved, string finalSubmittedAnswer)
		{
				currUser.numericDataForCurrentActivity [idxOfSolved] = (solved ? 1 : 0);
				currUser.finalSubmittedWord = finalSubmittedAnswer;
			
		}





		//will save to player prefs, in case something bad happens.


		public void SaveActivityDataAndClearForNext (string activityTargetWord, string activityInitialWord)
		{       //store all the activity data (contained in an array) to the string representing all activity data.
				
				
				RecordActivityInitialWord (activityInitialWord);
				currUser.AppendDataForCurrentActivityToPreviousData ();

				LogActivitySummaryData (currUser.targetWordOfCurrentActivity, currUser.finalSubmittedWord);
				currUser.ClearCurrentActivityData ();

		}

		public void UpdateUserSessionAndWriteAllUpdatedDataToPlayerPrefs ()
		{       //update the current session number so that next time we retrieve this students data we set up the right session
				int nextSession = currUser.CurrentSession + 1;
				//we are going to overwrite the stored string for this student in player prefs;
				//the first part of that string is the substing [int, int] which has the experiment wide session parameters
				//string experimentWideParametersOfStudent = BuildStringForExperimentWideParams (currUser.ColorCodingSchemeAsInt, nextSession);
				string experimentWideParametersOfStudent = UpdateSavedSessionAsString (nextSession);
				StringBuilder studentData = new StringBuilder (experimentWideParametersOfStudent);
				//nbe sure to save the activity data after each activity and just re-save the data when done
				studentData.Append (currUser.dataFromPreviouslyCompletedActivities);
				string updatedData = studentData.ToString ();
				PlayerPrefs.SetString (currUser.studentName, updatedData);


		       
				

		}

		public int GetUsersSession ()
		{
				if (!ReferenceEquals (currUser, null))
						return currUser.CurrentSession;
				return 0;
		
		
		}





		//stupid. just going to log the summary data here too.
		public void LogActivitySummaryData (string targetWord, string answer)
		{       //only log events in activity mode.
		
				
				string fileName = currUser.studentName + "_" + currUser.CurrentSession + "_" + SessionManager.assessmentStartTime.ToString ("yyyyMMdd_hh_mm_ss") + ".csv";
				
				string filePath = System.IO.Path.Combine (DATA_FILE_DIRECTORY, fileName);
				System.IO.StreamWriter file = new System.IO.StreamWriter (filePath, true);
			
				//thanks http://stackoverflow.com/questions/18757097/writing-data-into-csv-file
				List<string> elements = new List<string> ();
			
				elements.Add (DateTime.Now.ToString ("u"));

				elements.Add (targetWord);
				elements.Add (answer);
				
			
				var csv = string.Join (",", elements.ToArray ());
			
			
				file.WriteLine (csv);
			
				file.Close ();
			
			
		}
		


















		/*
	 * distinct from summary data.
	 * events are a record of every input the user put into the program
	 * (simmilar to the event logger from caravan)
	 * code is ripped from caravan event logger.
	 * 
	 * 
	 * each message is the event we want to log.
	 * in caravan, we made it to that each event would correlate with other info
	 * but, in this case, we dont need a lot of that because each child is unique.
	 * 
	 * */




		static string fileName = "";

		void SetFileName ()
		{
				fileName = currUser.studentName + "_" + currUser.CurrentSession + "_" + SessionManager.assessmentStartTime.ToString ("yyyyMMdd_hh_mm_ss") + ".csv";
		}

		public void LogEvent (string eventName, string eventParam1, string eventParam2)
		{       //only log events in activity mode.

				if (SessionManager.DelegateControlToStudentActivityController) {
						// Write the string to a file.append mode is enabled so that the log
						// lines get appended to  test.txt than wiping content and writing the log
		
						string assessType = (SessionManager.IsActivityMode ? "activity" : "assessment");
						if (fileName.Length < 1)
								SetFileName ();
						string filePath = System.IO.Path.Combine (LOG_FILE_DIRECTORY, fileName);
						System.IO.StreamWriter file = new System.IO.StreamWriter (filePath, true);
			
						//thanks http://stackoverflow.com/questions/18757097/writing-data-into-csv-file
						List<string> elements = new List<string> ();
	
						elements.Add (DateTime.Now.ToString ("u"));
					
					

			
						elements.Add (assessType);
						elements.Add (eventName);
						elements.Add (eventParam1);
						elements.Add (eventParam2);
			
						var csv = string.Join (",", elements.ToArray ());
			
					
						file.WriteLine (csv);
			
						file.Close ();
			
			
				}

		}

}
		