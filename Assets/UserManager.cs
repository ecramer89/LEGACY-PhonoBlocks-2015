using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class UserManager : MonoBehaviour
{
    static GameObject userGrid;

    public static int imageWidth = 200;
    public static int imageHeight = 200;

    public static List<string> userIDs
    {
        get
        {
            return new List<string>(DataManager.userImages.Keys);
        }
    }

    // Use this for initialization
    void Start()
    {
        userGrid = gameObject;
    }
    
    public static void CreateUser(Texture2D tex2d, string filename)
    {
        Texture2D tex2dCopy = Instantiate(tex2d) as Texture2D;
        TextureScale.Bilinear(tex2dCopy, imageWidth, imageHeight);

        UITexture ut = NGUITools.AddWidget<UITexture>(userGrid);
        ut.material = new Material(Shader.Find("Unlit/Transparent Colored"));
        ut.shader = Shader.Find("Unlit/Transparent Colored");
        ut.mainTexture = tex2dCopy;
        ut.gameObject.AddComponent<BoxCollider>();
        ut.gameObject.AddComponent<UIDragPanelContents>();
        ut.gameObject.AddComponent<UserController>();
        ut.gameObject.name = filename;
        ut.MakePixelPerfect();
    }

    public static void SetActive(bool state)
    {
        NGUITools.SetActiveChildren(userGrid, state);
        GameObject.Find("Controllers").GetComponent<UserGridController>().RefreshGrid();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (this.transform.childCount == 0)
        {
            foreach (KeyValuePair<string, Texture2D> kvp in DataManager.userImages)
            {
                print("asd");
                //CreateUser(kvp.Value, kvp.Key);
            }
        }
    }    
}
