using System;
using DG.Tweening;
using NomadsPlanet.Utils;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace NomadsPlanet
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private LoadingFaderController fadeController;
        [SerializeField] private GameObject queueBoard;
        [SerializeField] private TMP_Text queueStatusText;
        [SerializeField] private TMP_Text queueTimerText;
        [SerializeField] private TMP_Text findMatchButtonText;
        [SerializeField] private Toggle teamToggle;

        // [SerializeField] private TMP_InputField joinCodeField;

        private bool _isMatchmaking;
        private bool _isCancelling;
        private bool _isBusy;
        private float _timeInQueue;

        private void Start()
        {
            if (ClientSingleton.Instance == null)
            {
                return;
            }

            queueStatusText.text = string.Empty;
            queueTimerText.text = string.Empty;
            queueBoard.SetActive(false);
        }

        private void Update()
        {
            if (_isMatchmaking)
            {
                _timeInQueue += Time.deltaTime;
                TimeSpan ts = TimeSpan.FromSeconds(_timeInQueue);
                queueTimerText.text = $"{ts.Minutes:00}:{ts.Seconds:00}";
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
                queueStatusText.DOText("Cancelling...", .25f, scrambleMode: ScrambleMode.Lowercase);
                _isCancelling = true;

                // Cancel Matchmaking
                await ClientSingleton.Instance.GameManager.CancelMatchmaking();
                _isCancelling = false;
                _isMatchmaking = false;
                _isBusy = false;
                queueBoard.SetActive(false);
                findMatchButtonText.DOText("Find Match", .25f, scrambleMode: ScrambleMode.Lowercase);
                queueStatusText.DOText(string.Empty, .25f, scrambleMode: ScrambleMode.Lowercase);
                queueTimerText.text = string.Empty;
                return;
            }

            if (_isBusy)
            {
                return;
            }

            // Start queue
            ClientSingleton.Instance.GameManager.MatchmakeAsync(teamToggle.isOn, OnMatchMade);
            findMatchButtonText.DOText("Cancel", .25f, scrambleMode: ScrambleMode.Lowercase);
            queueStatusText.DOText("Searching...", .25f, scrambleMode: ScrambleMode.Lowercase);
            _timeInQueue = 0f;
            _isMatchmaking = true;
            _isBusy = true;
            queueBoard.SetActive(true);
        }

        private void OnMatchMade(MatchmakerPollingResult result)
        {
            switch (result)
            {
                case MatchmakerPollingResult.Success:
                    queueStatusText.DOText("Connecting...", .25f, scrambleMode: ScrambleMode.Lowercase);
                    StartCoroutine(fadeController.FadeIn());
                    break;
                case MatchmakerPollingResult.TicketCreationError:
                    queueStatusText.DOText("TicketCreationError", .2f, scrambleMode: ScrambleMode.Lowercase);
                    break;
                case MatchmakerPollingResult.TicketCancellationError:
                    queueStatusText.DOText("TicketCreationError", .2f, scrambleMode: ScrambleMode.Lowercase);
                    break;
                case MatchmakerPollingResult.TicketRetrievalError:
                    queueStatusText.DOText("TicketCreationError", .2f, scrambleMode: ScrambleMode.Lowercase);
                    break;
                case MatchmakerPollingResult.MatchAssignmentError:
                    queueStatusText.DOText("TicketCreationError", .2f, scrambleMode: ScrambleMode.Lowercase);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        public async void StartHost()
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;
            StartCoroutine(fadeController.FadeIn());
            await HostSingleton.Instance.GameManager.StartHostAsync(false);

            _isBusy = false;
        }

        public async void JoinAsync(Lobby lobby)
        {
            if (_isBusy)
            {
                return;
            }

            _isBusy = true;

            try
            {
                Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
                string joinCode = joiningLobby.Data[NetworkSetup.JoinCode].Value;

                StartCoroutine(fadeController.FadeIn());
                await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
            }
            catch (LobbyServiceException lobbyServiceException)
            {
                StartCoroutine(fadeController.FadeOut());
                Debug.LogError(lobbyServiceException);
            }

            _isBusy = false;
        }

        // public async void StartClient()
        // {
        //     if (_isBusy)
        //     {
        //         return;
        //     }
        //
        //     _isBusy = true;
        //
        //     StartCoroutine(fadeController.FadeIn());
        //     bool result = await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
        //
        //     if (!result)
        //     {
        //         StartCoroutine(fadeController.FadeOut());
        //     }
        //
        //     _isBusy = false;
        // }
    }
}