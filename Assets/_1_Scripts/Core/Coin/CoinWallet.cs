using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
using NomadsPlanet.Utils;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class CoinWallet : NetworkBehaviour
    {
        // ref
        [SerializeField] private BountyCoin coinPrefab;

        // settings
        private const float CoinSpread = 5f;
        private const int BountyCoinCount = 5;

        [SerializeField] private LayerMask layerMask;

        // 플레이어가 갖고 있는 전체 동전의 수
        public PlayerScore playerScore;

        public NetworkVariable<int> totalCoins = new();
        public GameObject vfx;

        private readonly Collider[] _coinBuffer = new Collider[1];
        private Vector3 _coinSize;

        private void Start()
        {
            vfx.SetActive(false);
        }

        public override void OnNetworkSpawn()
        {
            if (!IsServer)
            {
                return;
            }

            ES3.Save(PrefsKey.InGameCoinKey, 0);
            playerScore.playerWeapon.OnAttack += HandleAttack;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer)
            {
                return;
            }

            ES3.Save(PrefsKey.InGameCoinKey, 0);
            playerScore.playerWeapon.OnAttack -= HandleAttack;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Coin coin))
            {
                if (!IsServer)
                {
                    return;
                }

                vfx.SetActive(false);

                // Get Coin Logic
                int coinValue = coin.Collect();
                totalCoins.Value += coinValue;
                playerScore.GetScore(coinValue);
                ES3.Save(PrefsKey.InGameCoinKey, totalCoins.Value);

                // Vfx
                vfx.SetActive(true);
                SoundManager.Instance.PlayCoinGetSfx();
            }
        }

        private void HandleAttack(PlayerWeapon weapon)
        {
            int bountyValue = (int)(totalCoins.Value * .2f);
            int bountyCoinValue = bountyValue / BountyCoinCount;

            for (int i = 0; i < BountyCoinCount; i++)
            {
                var coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), quaternion.identity);
                coinInstance.SetValue(bountyCoinValue);
                coinInstance.NetworkObject.Spawn();
            }
        }

        private Vector3 GetSpawnPoint()
        {
            Vector3 spawnPoint = transform.position + Random.insideUnitSphere * CoinSpread;
            spawnPoint = new Vector3(spawnPoint.x, 1.5f, spawnPoint.z);
            return IsSpawnPointFree(spawnPoint) ? spawnPoint : GetSpawnPoint();
        }

        /// <summary>
        /// 생성 위치가 다른 누군가와 겹치지는 않는지 검사한다.
        /// </summary>
        /// <param name="spawnPoint">확인할 곳의 위치 좌표</param>
        /// <returns>위치가 겹치지 않으면 true, 겹치면!! false</returns>
        private bool IsSpawnPointFree(Vector3 spawnPoint)
        {
            int numColliders = Physics.OverlapBoxNonAlloc(spawnPoint, _coinSize, _coinBuffer);

            return numColliders == 0;
        }
    }
}