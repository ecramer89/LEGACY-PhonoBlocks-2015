using UnityEngine;
using System.Collections;

public class UserManagementController : MonoBehaviour {

    public GameObject UserManagementUI;
    public GameObject CameraButton;
    public GameObject DeleteButton;
    public GameObject ReturnButton;
    public GameObject TeacherButton;

    public static bool manageModeActive = false;
    public static bool editModeActive = false;

	// Use this for initialization
	void Start () {
        ResetUI(); 

	}

    public void ActivateManageUI()
    {
        NGUITools.SetActive(CameraButton, true);
        NGUITools.SetActive(ReturnButton, true);     
        manageModeActive = true;
    }

    public void ResetUI()
    {
        bool flag = manageModeActive && DataManager.currentUserID != "";
        //NGUITools.SetActiveChildren(UserManagementUI, false);
        GetComponent<UserGridController>().EnableDrag();
        GetComponent<UserGridController>().ShowUsers();
        if (!DataManager.tempTeacherFlag)
            GetComponent<UserGridController>().DeselectUser();
        else
        {
            DataManager.tempTeacherFlag = false;
            GetComponent<UserGridController>().DeselectUser();
        }
        GetComponent<UserGridController>().RefreshGrid();
        GetComponent<FrontCameraController>().DeactivateCameraUI();        
        if (flag)
            ActivateManageUI();
        else
        {
            manageModeActive = false;
            editModeActive = false;
        }
    }

    public void ActivateEditUI()
    {        
        NGUITools.SetActive(TeacherButton, true);
        NGUITools.SetActive(DeleteButton, true);
        editModeActive = true;
    }

    public void DeactivateEditUI()
    {
        NGUITools.SetActive(TeacherButton, false);
        NGUITools.SetActive(DeleteButton, false);
        editModeActive = false;
    }
}
