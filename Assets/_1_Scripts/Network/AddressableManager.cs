using System.Collections;
using System.Collections.Generic;
using NomadsPlanet.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;

namespace NomadsPlanet
{
    public class AddressableManager : MonoBehaviour
    {
        [SerializeField] private Slider downloadProgressSlider;
        [SerializeField] private TMP_Text downloadText;

        private void Start()
        {
            downloadText.text = string.Empty;
        }

        public IEnumerator DownloadAllAssets()
        {
            yield break;

            // var checkHandle = Addressables.CheckForCatalogUpdates(false);
            // yield return checkHandle;
            //
            // if (checkHandle.Status == AsyncOperationStatus.Succeeded)
            // {
            //     List<string> catalogs = checkHandle.Result;
            //     if (catalogs is { Count: > 0 })
            //     {
            //         var dlHandle = Addressables.DownloadDependenciesAsync(catalogs, true);
            //
            //         // 여기서 진행 상황을 확인
            //         while (!dlHandle.IsDone)
            //         {
            //             downloadProgressSlider.value = dlHandle.PercentComplete;
            //             downloadText.text = $"Loading... {dlHandle.PercentComplete}";
            //             yield return null;
            //         }
            //
            //         if (dlHandle.Status == AsyncOperationStatus.Failed)
            //         {
            //             CustomFunc.ConsoleLog("Failed to download dependencies.");
            //         }
            //     }
            // }
            //
            // Addressables.Release(checkHandle);
        }
    }
}