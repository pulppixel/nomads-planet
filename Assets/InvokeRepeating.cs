using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokeRepeating : MonoBehaviour
{
    private int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(this.Invoked), 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Invoked()
    {
        Debug.Log("Invoked: " + i++);
    }
}
