// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GameVersionField.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------
 
using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.SyncTester
{

	public class DragToMoveSpeedControl : MonoBehaviour
    {
		public DragToMove DragToMoveComponent;

        public InputField PropertyValueInput;

        private float _cache;

        private bool registered;

        private void OnEnable()
        {
            if (!this.registered)
            {
                this.registered = true;
                this.PropertyValueInput.onEndEdit.AddListener(this.OnEndEdit);
            }
        }

        private void OnDisable()
        {
            this.registered = false;
            this.PropertyValueInput.onEndEdit.RemoveListener(this.OnEndEdit);
        }

        private void Update()
        {
			if (DragToMoveComponent.speed != this._cache)
            {
				this._cache = DragToMoveComponent.speed;
				this.PropertyValueInput.text = this._cache.ToString();
            }
        }

        // new UI will fire "EndEdit" event also when loosing focus. So check "enter" key and only then submit form.
        public void OnEndEdit(string value)
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Tab))
            {
                this.SubmitForm(value.Trim());
            }
            else
            {
                this.SubmitForm(value);
            }
        }

        public void SubmitForm(string value)
        {
			this._cache = int.Parse(value);
			DragToMoveComponent.speed = this._cache;
			Debug.Log("DragToMoveSpeedControl speed = " + DragToMoveComponent.speed, this);
        }
    }
}