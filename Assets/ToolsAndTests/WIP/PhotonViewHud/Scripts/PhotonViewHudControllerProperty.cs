// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomListView.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine.UI;

using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;

namespace ExitGames.Demos.Cockpit
{
    /// <summary>
    /// Photon view hud controller property.
    /// </summary>
    public class PhotonViewHudControllerProperty : PropertyListenerBase
    {

        public Text Text;

        Player _cache = null;

        PhotonViewHud _hud;

        void Awake()
        {
            _hud = this.GetComponentInParent<PhotonViewHud>();
        }

        void Update()
        {
            if (_hud.Target != null)
            {

                if (_cache == null || _hud.Target.photonView.Controller.ActorNumber != _cache.ActorNumber)
                {
                    _cache = _hud.Target.photonView.Controller;
                    Text.text = "" + _cache.ActorNumber;
                    this.OnValueChanged();
                }
            }
            else
            {
                if (_cache != null)
                {
                    _cache = null;
                    Text.text = "n/a";
                }
            }

        }
    }
}