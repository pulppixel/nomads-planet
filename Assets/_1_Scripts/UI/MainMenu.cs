using System;
using DG.Tweening;
using NomadsPlanet.Utils;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace NomadsPlanet
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private LoadingFaderController fadeController;
        [SerializeField] private GameObject queueBoard;
        [SerializeField] private TMP_Text queueStatusText;
        [SerializeField] private TMP_Text queueTimerText;
        [SerializeField] private TMP_Text findMatchButtonText;

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
                queueStatusText.DOText(GetLocalizedString("Menu_Cancelling"), .25f,
                    scrambleMode: ScrambleMode.Lowercase);
                _isCancelling = true;

                // Cancel Matchmaking
                await ClientSingleton.Instance.GameManager.CancelMatchmaking();
                _isCancelling = false;
                _isMatchmaking = false;
                _isBusy = false;
                queueBoard.SetActive(false);
                findMatchButtonText.DOText(GetLocalizedString("Menu_Find Match"), .25f,
                    scrambleMode: ScrambleMode.Lowercase);
                queueStatusText.DOText(string.Empty, .25f, scrambleMode: ScrambleMode.Lowercase);
                queueTimerText.text = string.Empty;
                return;
            }

            if (_isBusy)
            {
                return;
            }

            // Start queue
            ClientSingleton.Instance.GameManager.MatchmakeAsync(false, OnMatchMade);
            findMatchButtonText.DOText(GetLocalizedString("Menu_Cancel"), .25f, scrambleMode: ScrambleMode.Lowercase);
            queueStatusText.DOText(GetLocalizedString("Menu_Searching"), .25f, scrambleMode: ScrambleMode.Lowercase);
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
                    queueStatusText.DOText(GetLocalizedString("Menu_Connecting"), .25f,
                        scrambleMode: ScrambleMode.Lowercase);
                    MenuInteraction.IsInteracting = false;
                    StartCoroutine(fadeController.FadeIn());
#if !UNITY_SERVER
                    VivoxVoiceManager.Instance.DisconnectAllChannels();
                    VivoxVoiceManager.Instance.Logout();
#endif
                    break;
                case MatchmakerPollingResult.TicketCreationError:
                    queueStatusText.DOText(GetLocalizedString("Menu_Error"), .2f, scrambleMode: ScrambleMode.Lowercase);
                    break;
                case MatchmakerPollingResult.TicketCancellationError:
                    queueStatusText.DOText(GetLocalizedString("Menu_Error"), .2f, scrambleMode: ScrambleMode.Lowercase);
                    break;
                case MatchmakerPollingResult.TicketRetrievalError:
                    queueStatusText.DOText(GetLocalizedString("Menu_Error"), .2f, scrambleMode: ScrambleMode.Lowercase);
                    break;
                case MatchmakerPollingResult.MatchAssignmentError:
                    queueStatusText.DOText(GetLocalizedString("Menu_Error"), .2f, scrambleMode: ScrambleMode.Lowercase);
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

#if !UNITY_SERVER
            VivoxVoiceManager.Instance.DisconnectAllChannels();
            VivoxVoiceManager.Instance.Logout();
#endif
            StartCoroutine(fadeController.FadeIn());
            MenuInteraction.IsInteracting = false;
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

        private static string GetLocalizedString(string keyName)
        {
            const string tableName = "LocaleTable";

            var stringOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(tableName, keyName);
            return stringOperation is { IsDone: true, Status: AsyncOperationStatus.Succeeded }
                ? stringOperation.Result
                : string.Empty;
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