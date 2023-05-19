using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class TestGui : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{

		if (GUILayout.Button ("reload scene"))
		{
			SceneManager.LoadScene ("DestroyTest");
		}

		if (GUILayout.Button ("Destroy All"))
		{
			PhotonNetwork.DestroyAll ();
		}

		if (GUILayout.Button ("Get ALL PhotonView"))
		{
			foreach (var _pv in PhotonNetwork.PhotonViews)
			{
				Debug.Log (_pv.ToString ());
			}
		}


			
	}

}
