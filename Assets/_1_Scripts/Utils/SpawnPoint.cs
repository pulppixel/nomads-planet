using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class SpawnPoint : MonoBehaviour
    {
        private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

        private void OnEnable()
        {
            spawnPoints.Add(this);
        }

        private void OnDisable()
        {
            spawnPoints.Remove(this);
        }

        public static Vector3 GetRandomSpawnPos()
        {
            if (spawnPoints.Count == 0)
            {
                return new Vector3(Random.value < .5f ? -25f : 25f, 2f, Random.value < .5f ? -25f : 25f);
            }

            return spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 5f);
        }
    }
}