using UnityEngine;
using System.Collections;




/* We will make this into a class that allows teachers to pick the prolbem type, versus the session*/
public class ContentButton : MonoBehaviour
{

		public GameObject sessionManagerOB;
		SessionManager sessionManager;
		//public ProblemManager.Name problemType;
		ProblemManager problemTypeData;

		void Start ()
		{
				sessionManager = sessionManagerOB.GetComponent<SessionManager> ();

		}

		void OnPress (bool pressed)
		{
            
				if (pressed) {
					

						//sessionManager.IncreaseNumberOfProblemsOfType (problemType);
				}
			



		}




		



}
