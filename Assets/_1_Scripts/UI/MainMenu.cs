using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace NomadsPlanet.UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject queueBoard;
        [SerializeField] private TMP_Text queueStatusText;
        [SerializeField] private TMP_Text queueTimerText;
        [SerializeField] private TMP_InputField joinCodeField;
        [SerializeField] private TMP_Text findMatchButtonText;

        private bool _isMatchmaking;
        private bool _isCancelling;
        private float _timeInQueue;

        private void Start()
        {
            if (ClientSingleton.Instance == null)
            {
                return;
            }

            queueBoard.SetActive(false);
            queueStatusText.text = string.Empty;
            queueTimerText.text = string.Empty;
        }

        private void Update()
        {
            if (_isMatchmaking)
            {
                _timeInQueue += Time.deltaTime;
            }
        }

        public async void FindMatchPressed()
        {
            if (_isCancelling)
            {
                return;
            }

            if (_isMatchmaking)
            {
                queueStatusText.DOText("Cancelling...", .5f, scrambleMode: ScrambleMode.Lowercase);
                _isCancelling = true;
                // Cancel Matchmaking
                await ClientSingleton.Instance.GameManager.CancelMatchmaking();

                queueBoard.SetActive(false);
                _isCancelling = false;
                _isMatchmaking = false;
                findMatchButtonText.DOText("Find Match", .5f, scrambleMode: ScrambleMode.Lowercase);
                queueStatusText.DOText(string.Empty, .5f, scrambleMode: ScrambleMode.Lowercase);
                return;
            }

            // Start queue
            ClientSingleton.Instance.GameManager.MatchmakeAsync(OnMatchMade);
            findMatchButtonText.DOText("Cancel", .5f, scrambleMode: ScrambleMode.Lowercase);
            queueStatusText.DOText("Searching...", .5f, scrambleMode: ScrambleMode.Lowercase);
            _isMatchmaking = true;
            queueBoard.SetActive(true);
        }

        private void OnMatchMade(MatchmakerPollingResult result)
        {
            switch (result)
            {
                case MatchmakerPollingResult.Success:
                    queueStatusText.DOText("Connecting...", .5f, scrambleMode: ScrambleMode.Lowercase);
                    break;
                case MatchmakerPollingResult.TicketCreationError:
                    queueStatusText.DOText("TicketCreationError", .3f, scrambleMode: ScrambleMode.Lowercase);
                    break;
                case MatchmakerPollingResult.TicketCancellationError:
                    queueStatusText.DOText("TicketCreationError", .3f, scrambleMode: ScrambleMode.Lowercase);
                    break;
                case MatchmakerPollingResult.TicketRetrievalError:
                    queueStatusText.DOText("TicketCreationError", .3f, scrambleMode: ScrambleMode.Lowercase);
                    break;
                case MatchmakerPollingResult.MatchAssignmentError:
                    queueStatusText.DOText("TicketCreationError", .3f, scrambleMode: ScrambleMode.Lowercase);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        public async void StartHost()
        {
            await HostSingleton.Instance.GameManager.StartHostAsync();
        }

        public async void StartClient()
        {
            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
        }
    }
}