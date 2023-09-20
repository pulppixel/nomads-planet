using Cinemachine;
using UnityEngine;
using NomadsPlanet.Utils;

namespace NomadsPlanet
{
    public class GameTimelineManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] characters;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        private void Start()
        {
            int currentCharacter = ES3.Load(PrefsKey.AvatarTypeKey, 0);
            virtualCamera.Priority = 100;

            for (int i = 0; i < characters.Length; i++)
            {
                characters[i].SetActive(i == currentCharacter);
            }
        }

        public void InitCameraPriority()
        {
            virtualCamera.Priority = 100;
        }

        public void SetCameraPriority()
        {
            virtualCamera.Priority = -1;
        }
    }
}