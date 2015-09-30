using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class FrontCameraController : MonoBehaviour
{
    public Texture2D currentImage;
    static GameObject currentUserObject;

    public static bool cameraModeActive = false;

    static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0);
    static long timeSinceEpoch
    {
        get
        {
            return System.Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
        }
    }
    public GameObject cameraButton;

    // Use this for initialization
    void Start()
    {
    }

    public void ActivateCameraUI()
    {
        currentUserObject = GameObject.Find(DataManager.currentUserID);
        GetComponent<UserGridController>().HideUsers();
        GetComponent<UserManagementController>().DeactivateEditUI();
        cameraModeActive = true;
    }

    public void SavePicture()
    {
        print("Add User");
        string currentTime = timeSinceEpoch.ToString();
        GetComponent<UserGridController>().CreateUser(currentImage as Texture2D, currentTime, true);
        DataManager.AddUserImage(currentImage, currentTime);
        GetComponent<UserManagementController>().ResetUI();
        GetComponent<UserManagementController>().ActivateManageUI();
    }

    public void DeactivateCameraUI()
    {  
        cameraModeActive = false;
    }
}
