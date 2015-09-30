using UnityEngine;
using System.Collections;
using System.IO;

public class DropReceiver : MonoBehaviour {

    void OnDrop(GameObject drag)
    {
        if (drag.name == DataManager.currentUserID)
        {
            print(drag.name + " deleted");
            GameObject.Find("Controllers").GetComponent<UserGridController>().DeleteCurrentUser();
        }
    }
}
