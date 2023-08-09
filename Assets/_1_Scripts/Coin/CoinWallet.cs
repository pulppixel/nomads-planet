using System;
using System.Collections;
using UnityEngine;
using Unity.Netcode;

namespace NomadsPlanet
{
    public class CoinWallet : NetworkBehaviour
    {
        public NetworkVariable<int> TotalCoins = new();
        public GameObject vfx;

        private void Start()
        {
            vfx.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out Coin coin))
            {
                return;
            }

            vfx.SetActive(false);
            int coinValue = coin.Collect();

            if (!IsServer)
            {
                return;
            }

            vfx.SetActive(true);
            TotalCoins.Value += coinValue;
        }
    }
}