using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UserGridController : MonoBehaviour
{

    public GameObject UserGrid;
    public static GameObject border;
    public GameObject ReturnUserImage;

    public static int imageWidth = 200;
    public static int imageHeight = 200;

    List<string> sortedIDs = new List<string>();

    void Start()
    {
        border = GameObject.Find("Border");
        NGUITools.SetActive(border, false);

        foreach (KeyValuePair<string, Texture2D> kvp in DataManager.userImages)
        {
            CreateUser(kvp.Value as Texture2D, kvp.Key, false);
        }
        RefreshGrid();
    }

    public void SelectUser(GameObject user)
    {
        print(user);
        GetComponent<UserGridController>().DisableDrag();
        DataManager.currentUserID = user.name;
        NGUITools.SetActive(border, true);
        border.transform.parent = user.transform;
        border.transform.localPosition = new Vector3(0, 0, 0.5f);
        border.transform.parent = user.transform.parent;
        border.GetComponent<SpriteRenderer>().enabled = true;
        user.GetComponent<SpringPosition>().enabled = true;
        user.GetComponent<SpringPosition>().target = user.transform.localPosition;   
        if (UserManagementController.manageModeActive)
            GetComponent<UserManagementController>().ActivateEditUI();
        UITexture ut = ReturnUserImage.GetComponent<UITexture>();
        ut.depth = 10;
        ut.material = user.GetComponent<UITexture>().material;
        ut.mainTexture = user.GetComponent<UITexture>().mainTexture;
        ReturnUserImage.SetActive(true);
        ReturnUserImage.transform.parent.parent.gameObject.SetActive(false);
        ReturnUserImage.transform.parent.parent.gameObject.SetActive(true);
    }

    public void DeselectUser()
    {
        print("Deselect User");
        GetComponent<UserGridController>().EnableDrag();
       // border.GetComponent<SpriteRenderer>().enabled = false;
        GameObject userGO = GameObject.Find(DataManager.currentUserID);
        if (userGO != null)
            userGO.GetComponent<SpringPosition>().enabled = false;
        ReturnUserImage.GetComponent<UITexture>().depth = 0;
        DataManager.currentUserID = "";
        DataManager.tempTeacherFlag = false;
        NGUITools.SetActive(border, false);
        if (UserManagementController.editModeActive)
            GetComponent<UserManagementController>().DeactivateEditUI();
        ReturnUserImage.SetActive(false);
    }

    public void DeleteCurrentUser()
    {
        DataManager.RemoveCurrentUser();
        DeselectUser();
        RefreshGrid();
        RefreshGrid();
    }

    public void HideUsers()
    {
        NGUITools.SetActiveChildren(UserGrid, false);
    }

    public void ShowUsers()
    {
        NGUITools.SetActiveChildren(UserGrid, true);
    }

    public void EnableDrag()
    {
        UserGrid.transform.parent.GetComponent<UIDraggablePanel>().enabled = true;
    }

    public void DisableDrag()
    {
        UserGrid.transform.parent.GetComponent<UIDraggablePanel>().enabled = false;
    }

    public void CreateUser(Texture2D tex2d, string filename, bool hide)
    {
        Texture2D tex2dCopy = Instantiate(tex2d) as Texture2D;
        TextureScale.Bilinear(tex2dCopy, imageWidth, imageHeight);

        UITexture ut = NGUITools.AddChild<UITexture>(UserGrid);
        ut.material = new Material(Shader.Find("Unlit/Transparent Colored"));
        ut.shader = Shader.Find("Unlit/Transparent Colored");
        ut.mainTexture = tex2dCopy;
        ut.gameObject.AddComponent<BoxCollider>();
        ut.gameObject.AddComponent<UIDragPanelContents>();
        ut.gameObject.AddComponent<UserController>();
        ut.gameObject.name = filename;
        ut.MakePixelPerfect();
        SpringPosition springPosition = ut.gameObject.AddComponent<SpringPosition>();
        springPosition.enabled = false;     
        if (hide)
            NGUITools.SetActiveChildren(UserGrid, false);        
    }

    public void RefreshGrid()
    {
        sortedIDs.Clear();

        foreach (Transform t in UserGrid.transform)
        {
            if(t.name != "Border")
                sortedIDs.Add(DataManager.GetMostRecentPlay(t.name) + "_" + t.name);
        }

        sortedIDs.Sort();
        sortedIDs.Reverse();

        for (int i = 0; i < sortedIDs.Count; i++)
        {
            if (sortedIDs[i].Split('_')[1] != "Border")
                GameObject.Find(sortedIDs[i].Split('_')[1]).name = i + "_" + sortedIDs[i];
        }

        UserGrid.GetComponent<UIGrid>().Reposition();

        foreach (Transform t in UserGrid.transform)
        {
            if(t.name != "Border")
                t.name = t.name.Split('_')[2];
        }
    }
}
