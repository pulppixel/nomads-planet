using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace NomadsPlanet
{
    public class InputFieldTextToCapital : MonoBehaviour
    {
        private TMP_InputField _inputField;

        private void Awake()
        {
            _inputField.GetComponent<TMP_InputField>();
        }

        public void UpdateInputField()
        {
            _inputField.text = Regex.Replace(
                _inputField.text,
                "[a-zA-Z]", match => match.Value.ToUpperInvariant()
            );
        }
    }
}