using Unity.Netcode;
using UnityEngine;

namespace NomadsPlanet
{
    public class CoinSpawner : NetworkBehaviour
    {
        [SerializeField] private RespawningCoin coinPrefab;

        [SerializeField] private int maxCoins = 50;

        [SerializeField] private int coinValue = 10;

        [SerializeField] private Vector2 xSpawnRange;

        [SerializeField] private Vector2 zSpawnRange;

        private Collider[] coinBuffer = new BoxCollider[1];

        private Vector3 coinSize;

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                return;
            }

            coinSize = coinPrefab.GetComponent<BoxCollider>().size;

            for (int i = 0; i < maxCoins; i++)
            {
                SpawnCoin();
            }
        }

        private void SpawnCoin()
        {
            RespawningCoin coinInstance = Instantiate(
                coinPrefab,
                GetSpawnPoint(),
                Quaternion.identity);

            coinInstance.SetValue(coinValue);
            coinInstance.GetComponent<NetworkObject>().Spawn();
            coinInstance.transform.SetParent(this.transform);

            coinInstance.OnCollected += HandleCoinCollected;
        }

        private void HandleCoinCollected(RespawningCoin coin)
        {
            coin.transform.position = GetSpawnPoint();
            coin.Reset();
        }

        private Vector3 GetSpawnPoint()
        {
            float x = Random.Range(xSpawnRange.x, xSpawnRange.y);
            float z = Random.Range(zSpawnRange.x, zSpawnRange.y);

            Vector3 spawnPoint = new Vector3(x, 1.5f, z);
            int numColliders = Physics.OverlapBoxNonAlloc(spawnPoint, coinSize, coinBuffer);

            return numColliders == 0 ? spawnPoint : GetSpawnPoint();
        }
    }
}