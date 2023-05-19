using System;
using TMPro;
using UnityEngine;

namespace MameshibaGames.Kekos.Models
{
    [RequireComponent(typeof(TMP_Text))]
    public class ShowHour : MonoBehaviour
    {
        private TMP_Text _glassesText;

        private void Start()
        {
            _glassesText = GetComponent<TMP_Text>();
            InvokeRepeating(nameof(SetHour), 0, 60);
        }

        private void SetHour()
        {
            DateTime now = DateTime.Now;

            _glassesText.text = $"{now.Hour:00}:{now.Minute:00}";
        }
    }
}