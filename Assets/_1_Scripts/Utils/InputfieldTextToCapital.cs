using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;

namespace NomadsPlanet
{
    public class InputFieldTextToCapital : MonoBehaviour
    {
        public TMP_InputField inputField;

        private void Start()
        {
            inputField.onValueChanged.AddListener((_) => UpdateInputField());
        }

        private void UpdateInputField()
        {
            inputField.text = Regex.Replace(
                inputField.text,
                "[a-zA-Z]", match => match.Value.ToUpperInvariant()
            );
        }
    }
}