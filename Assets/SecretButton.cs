using UnityEngine;
using System.Collections;

public class SecretButton : MonoBehaviour
{
    public int active = 0;

    void OnPress(bool isDown)
    {
        active = isDown ? 1 : 0;
    }
}
