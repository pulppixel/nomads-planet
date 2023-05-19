// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameVersionField.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
 
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

namespace Photon.Pun.UtilityScripts
{

	public class HideIfMasterClient : MonoBehaviourPunCallbacks
    {
		public bool invert = false;

		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			if (invert)
			{
				this.gameObject.SetActive (newMasterClient != PhotonNetwork.LocalPlayer);
			}
			else
			{
				this.gameObject.SetActive (newMasterClient == PhotonNetwork.LocalPlayer);
			}	
		}
    }
}