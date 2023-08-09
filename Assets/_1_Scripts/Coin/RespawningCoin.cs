using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class RespawningCoin : Coin
    {
        public event Action<RespawningCoin> OnCollected;

        private Animator _anims;
        private Vector3 previousPosition;

        private void Awake()
        {
            _anims = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(Random.Range(0, .5f));
            _anims.Rebind();
        }

        private void Update()
        {
            if (previousPosition != transform.position)
            {
                Show(true);
            }

            previousPosition = transform.position;
        }

        public override int Collect()
        {
            if (!IsServer)
            {
                Show(false);
                return 0;
            }

            if (alreadyCollected)
            {
                return 0;
            }

            alreadyCollected = true;

            OnCollected?.Invoke(this);

            return coinValue;
        }

        public void Reset()
        {
            alreadyCollected = false;
        }
    }
}