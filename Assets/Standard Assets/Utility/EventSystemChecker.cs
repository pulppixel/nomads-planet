using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemChecker : MonoBehaviour
{
    //public GameObject eventSystem;

    // Use this for initialization
    [Obsolete("Obsolete")]
    private void Awake()
    {
        if (FindObjectOfType<EventSystem>())
        {
            return;
        }

        //Instantiate(eventSystem);
        GameObject obj = new GameObject("EventSystem");
        obj.AddComponent<EventSystem>();
        obj.AddComponent<StandaloneInputModule>().forceModuleActive = true;
    }
}