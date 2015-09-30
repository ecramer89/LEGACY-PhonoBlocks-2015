using UnityEngine;
using System.Collections;




/* We will make this into a class that allows teachers to pick the prolbem type, versus the session*/
public class ContentButton : MonoBehaviour
{

		public GameObject sessionManagerOB;
	SessionsDirector sessionManager;
		//public ProblemManager.Name problemType;
		ProblemsRepository problemTypeData;

		void Start ()
		{
				sessionManager = sessionManagerOB.GetComponent<SessionsDirector> ();

		}

		void OnPress (bool pressed)
		{
            
				if (pressed) {
					

						//sessionManager.IncreaseNumberOfProblemsOfType (problemType);
				}
			



		}




		



}
