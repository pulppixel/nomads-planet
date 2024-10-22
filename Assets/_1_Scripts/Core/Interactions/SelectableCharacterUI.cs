﻿using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace NomadsPlanet
{
    public class SelectableCharacterUI : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTr;
        [SerializeField] private SfxPlayer sfxPlayer;
        private bool _isSetupDone;
        private bool _isPlayed;

        private IEnumerator Start()
        {
            rectTr.localScale = Vector3.zero;

            yield return new WaitForSeconds(.5f);
            _isSetupDone = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isPlayed && _isSetupDone && other.CompareTag("Player"))
            {
                _isPlayed = true;
                rectTr.DOScale(1f, .5f)
                    .SetEase(Ease.OutBack);

                sfxPlayer.PlaySfx(2);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isSetupDone && other.CompareTag("Player"))
            {
                _isPlayed = false;
                rectTr.DOScale(0f, .5f)
                    .SetEase(Ease.InBack);
            }
        }
    }
}