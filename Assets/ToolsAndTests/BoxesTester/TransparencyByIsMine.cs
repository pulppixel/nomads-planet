using UnityEngine;
using System.Collections;
using Photon.Pun;


public class TransparencyByIsMine : MonoBehaviourPun
{
    private Renderer rendererCached;
    private Material alpha;

    void Start()
    {
        this.rendererCached = this.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update ()
    {
        Color c = this.rendererCached.material.color;
        c.a = this.photonView.IsMine ? 1.0f : 0.5f;
        this.rendererCached.material.color = c;
    }
}
