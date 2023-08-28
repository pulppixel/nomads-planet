using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class SpawnPoint : MonoBehaviour
    {
        private static List<SpawnPoint> spawnPoints = new();

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
            return spawnPoints.Count == 0
                ? new Vector3(0f, 3f, 0f)
                : spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 5f);
        }
    }
}