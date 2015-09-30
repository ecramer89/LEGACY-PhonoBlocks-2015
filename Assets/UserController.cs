using UnityEngine;
using System.Collections;

public class UserController : MonoBehaviour
{
    static UserGridController userGridController;

    void Start()
    {
        userGridController = GameObject.Find("Controllers").GetComponent<UserGridController>();
    }

    void OnClick()
    {
        userGridController.SelectUser(this.gameObject);
        Debug.Log(name + " clicked");
        if (!UserManagementController.manageModeActive)
        {
            print("start game for " + name);
        }
    }

    void OnPress(bool isDown)
    {
        if (UserManagementController.editModeActive && !GetComponent<SpringPosition>().enabled && name == DataManager.currentUserID)
        {
            print(name + (isDown ? " pressed" : " released"));
            if (isDown)
            {
                GetComponent<Collider>().enabled = false;
                UIDragObject dragObject = gameObject.AddComponent<UIDragObject>();
                dragObject.target = transform;
                GetComponent<SpringPosition>().target = transform.localPosition;
            }
            else
            {
                GetComponent<Collider>().enabled = true;
                Destroy(GetComponent<UIDragObject>());
                GetComponent<SpringPosition>().enabled = true;
            }
        }
    }

    void Update()
    {        
    }
}

