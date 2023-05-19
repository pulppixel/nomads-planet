using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// Can be attached to a GameObject to show info about the owner of the PhotonView.
/// </summary>
/// <remarks>
/// This is a Photon.Monobehaviour, which adds the property photonView (that's all).
/// </remarks>
[RequireComponent(typeof(PhotonView))]
public class ShowInfoOfPlayer : MonoBehaviourPun
{
    private GameObject textGo;
    private TextMesh tm;
    public float CharacterSize = 0;

    public Font font;
    public bool DisableOnOwnObjects;

    void Start()
    {
        if (font == null)
        {
            #if UNITY_3_5
            font = (Font)FindObjectsOfTypeIncludingAssets(typeof(Font))[0];
            #else
            font = (Font)Resources.FindObjectsOfTypeAll(typeof(Font))[0];
            #endif
            Debug.LogWarning("No font defined. Found font: " + font);
        }

        if (tm == null)
        {
            textGo = new GameObject("3d text");
            //textGo.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            textGo.transform.parent = this.gameObject.transform;
            textGo.transform.localPosition = Vector3.zero;

            MeshRenderer mr = textGo.AddComponent<MeshRenderer>();
            mr.material = font.material;
            tm = textGo.AddComponent<TextMesh>();
            tm.font = font;
            tm.anchor = TextAnchor.MiddleCenter;
            if (this.CharacterSize > 0)
            {
                tm.characterSize = this.CharacterSize;
            }
        }
    }

    void Update()
    {
        bool showInfo = !this.DisableOnOwnObjects || this.photonView.IsMine;
        if (textGo != null)
        {
            textGo.SetActive(showInfo);
        }
        if (!showInfo)
        {
            return;
        }


        Player owner = this.photonView.Owner;
        if (owner != null)
        {
            tm.text = (string.IsNullOrEmpty(owner.NickName)) ? "player"+owner.ActorNumber : owner.NickName;
        }
        else if (this.photonView.IsSceneView)
        {
            tm.text = "scn";
        }
        else
        {
            tm.text = "n/a";
        }
    }
}
