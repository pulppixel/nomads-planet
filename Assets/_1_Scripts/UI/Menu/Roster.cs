using TMPro;
using UnityEngine;
using VivoxUnity;

namespace NomadsPlanet
{
    public class Roster : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;

        public IParticipant Participant { get; private set; }
        
        public void SetupRosterItem(IParticipant participant)
        {
            Participant = participant;
            nameText.text = participant.Account.DisplayName;
        }
    }
}