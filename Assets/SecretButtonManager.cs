using UnityEngine;
using System.Collections;

public class SecretButtonManager : MonoBehaviour
{
    public GameObject BottomLeftButton;
    public GameObject BottomRightButton;
    public GameObject TopLeftButton;
    public GameObject TopRightButton;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        int total = BottomLeftButton.GetComponent<SecretButton>().active +
            BottomRightButton.GetComponent<SecretButton>().active +
            TopLeftButton.GetComponent<SecretButton>().active +
            TopRightButton.GetComponent<SecretButton>().active;

        if (total >= 0)
        {
            GetComponent<UserManagementController>().ActivateManageUI();            
        }

        if (UserManagementController.manageModeActive || UserManagementController.editModeActive)
            Hide();
        else
            Show();
	}

    void Hide()
    {
        BottomLeftButton.GetComponent<UITexture>().enabled = false;
        BottomRightButton.GetComponent<UITexture>().enabled = false;
        TopLeftButton.GetComponent<UITexture>().enabled = false;
        TopRightButton.GetComponent<UITexture>().enabled = false;
    }

    void Show()
    {
        BottomLeftButton.GetComponent<UITexture>().enabled = true;
        BottomRightButton.GetComponent<UITexture>().enabled = true;
        TopLeftButton.GetComponent<UITexture>().enabled = true;
        TopRightButton.GetComponent<UITexture>().enabled = true;
    }
}
