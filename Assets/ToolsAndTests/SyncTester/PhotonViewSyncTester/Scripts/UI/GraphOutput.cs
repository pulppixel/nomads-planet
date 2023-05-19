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

	public class GraphOutput : MonoBehaviour
    {
		public GrapherEditor GrapherEditorComponent;

		public Dropdown PropertyValueInput;

		private GrapherEditor.GraphOutputChoices _cache;

        private bool registered;

        private void OnEnable()
        {
            if (!this.registered)
            {
                this.registered = true;
				this.PropertyValueInput.onValueChanged.AddListener(this.SubmitForm);
            }
        }
			
        private void OnDisable()
        {
            this.registered = false;
			this.PropertyValueInput.onValueChanged.RemoveListener(this.SubmitForm);
        }

        private void Update()
        {
			if (GrapherEditorComponent.GraphOutput != this._cache)
            {
				this._cache = GrapherEditorComponent.GraphOutput;
				this.PropertyValueInput.value = (int)this._cache;
            }
        }

        public void SubmitForm(int value)
        {
			this._cache = (GrapherEditor.GraphOutputChoices)value;
			GrapherEditorComponent.GraphOutput = this._cache;
			Debug.Log("GrapherEditor GraphOutput = " + GrapherEditorComponent.GraphOutput, this);
        }
    }
}