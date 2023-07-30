using Unity.Netcode;
using UnityEngine;

namespace NomadsPlanet
{
    public abstract class Coin : NetworkBehaviour
    {
        protected int coinValue;
        protected bool alreadyCollected;

        public abstract int Collect();

        public void SetValue(int value)
        {
            coinValue = value;
        }

        protected void Show(bool show)
        {
            gameObject.SetActive(show);
        }
    }
}