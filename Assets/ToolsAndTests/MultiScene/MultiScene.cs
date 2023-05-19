using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MultiScene : MonoBehaviour {

	void Start () {
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	void OnGUI()
	{

		if (PhotonNetwork.InRoom) {

			bool _newAuto = GUILayout.Toggle (PhotonNetwork.AutomaticallySyncScene, "auto sync level");

			if (PhotonNetwork.AutomaticallySyncScene != _newAuto)
			{
				PhotonNetwork.AutomaticallySyncScene =  _newAuto;
			}

			if (GUILayout.Button ("Load Multi Scene")) {
				PhotonNetwork.LoadLevel ("TestMultiScene");	
			}

			if (GUILayout.Button ("Photon Load Scene A")) {
				PhotonNetwork.LoadLevel ("Scene A");	
			}

			if (GUILayout.Button ("Photon Load Scene B")) {
				PhotonNetwork.LoadLevel ("Scene B");	
			}

			if (GUILayout.Button ("Unity Load Scene A")) {
				SceneManager.LoadScene("Scene A");	
			}
			
			if (GUILayout.Button ("Unity Load Scene B")) {
                SceneManager.LoadScene("Scene B");	
			}
		}
	}
}
