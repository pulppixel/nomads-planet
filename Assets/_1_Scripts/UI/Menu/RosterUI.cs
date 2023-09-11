using System;
using System.Collections;
using System.Linq;
using NomadsPlanet.Utils;
using System.Collections.Generic;
using UnityEngine;
using VivoxUnity;

namespace NomadsPlanet
{
    public class RosterUI : MonoBehaviour
    {
        [SerializeField] private Roster rosterPrefab;
        [SerializeField] private Roster otherPrefab;

        private readonly Dictionary<ChannelId, List<Roster>> _rosterObjects = new Dictionary<ChannelId, List<Roster>>();

        private void Awake()
        {
#if !UNITY_SERVER
            VivoxVoiceManager.Instance.OnParticipantAddedEvent += OnParticipantAdded;
            VivoxVoiceManager.Instance.OnParticipantRemovedEvent += OnParticipantRemoved;
            VivoxVoiceManager.Instance.OnUserLoggedOutEvent += OnUserLoggedOut;
#endif
        }

        private IEnumerator Start()
        {
#if !UNITY_SERVER
            yield return new WaitUntil(() => VivoxVoiceManager.Instance.LoginState == LoginState.LoggedIn);

            if (VivoxVoiceManager.Instance && VivoxVoiceManager.Instance.ActiveChannels.Count > 0)
            {
                var lobbyChannel =
                    VivoxVoiceManager.Instance.ActiveChannels.FirstOrDefault(ac =>
                        ac.Channel.Name == SceneName.MenuScene);

                if (lobbyChannel == null)
                {
                    yield break;
                }

                foreach (var participant in VivoxVoiceManager.Instance.LoginSession
                             .GetChannelSession(lobbyChannel.Channel).Participants)
                {
                    UpdateParticipantRoster(participant, participant.ParentChannelSession.Channel, true);
                }
            }
#endif
        }

        private void UpdateParticipantRoster(IParticipant participant, ChannelId channel, bool isAddParticipant)
        {
            if (isAddParticipant)
            {
                List<Roster> thisChannelList;

                if (_rosterObjects.ContainsKey(channel))
                {
                    Roster newRosterItem = Instantiate(rosterPrefab, gameObject.transform);
                    _rosterObjects.TryGetValue(channel, out thisChannelList);
                    newRosterItem.SetupRosterItem(participant);
                    if (thisChannelList != null)
                    {
                        thisChannelList.Add(newRosterItem);
                        _rosterObjects[channel] = thisChannelList;
                    }
                }
                else
                {
                    Roster newRosterItem = Instantiate(otherPrefab, gameObject.transform);
                    thisChannelList = new List<Roster> { newRosterItem };
                    newRosterItem.SetupRosterItem(participant);
                    _rosterObjects.Add(channel, thisChannelList);
                }

                CleanRoster(channel);
            }
            else
            {
                if (_rosterObjects.ContainsKey(channel))
                {
                    var removedItem = _rosterObjects[channel]
                        .FirstOrDefault(p => p.Participant.Account.Name == participant.Account.Name);
                    if (removedItem != null)
                    {
                        _rosterObjects[channel].Remove(removedItem);
                        Destroy(removedItem.gameObject);
                        CleanRoster(channel);
                    }
                    else
                    {
                        CustomFunc.ConsoleLog("Trying to remove a participant that has no roster item.", true);
                    }
                }
            }
        }

        private void OnParticipantAdded(string userName, ChannelId channel, IParticipant participant)
        {
            CustomFunc.ConsoleLog("OnPartAdded: " + userName);
            UpdateParticipantRoster(participant, channel, true);
        }

        private void OnParticipantRemoved(string userName, ChannelId channel, IParticipant participant)
        {
            CustomFunc.ConsoleLog("OnPartRemoved: " + participant.Account.Name);
            UpdateParticipantRoster(participant, channel, false);
        }


        public void ClearAllRosters()
        {
            foreach (List<Roster> rosterList in _rosterObjects.Values)
            {
                foreach (Roster item in rosterList)
                {
                    Destroy(item.gameObject);
                }

                rosterList.Clear();
            }

            _rosterObjects.Clear();
        }

        public void ClearChannelRoster(ChannelId channel)
        {
            List<Roster> rosterList = _rosterObjects[channel];
            foreach (Roster item in rosterList)
            {
                Destroy(item.gameObject);
            }

            rosterList.Clear();
            _rosterObjects.Remove(channel);
        }

        private void CleanRoster(ChannelId channel)
        {
            RectTransform rt = gameObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(0, _rosterObjects[channel].Count * 50);
        }


        private void OnUserLoggedOut()
        {
            ClearAllRosters();
        }

        private void OnDestroy()
        {
#if !UNITY_SERVER
            VivoxVoiceManager.Instance.OnParticipantAddedEvent -= OnParticipantAdded;
            VivoxVoiceManager.Instance.OnParticipantRemovedEvent -= OnParticipantRemoved;
            VivoxVoiceManager.Instance.OnUserLoggedOutEvent -= OnUserLoggedOut;
#endif
        }
    }
}