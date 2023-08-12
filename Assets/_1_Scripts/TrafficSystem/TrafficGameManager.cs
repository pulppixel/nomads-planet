using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NomadsPlanet
{
    public class TrafficGameManager : MonoBehaviour
    {
        public static TrafficGameManager Instance { get; private set; }
        public CarHandler carPrefab;
        public List<TrafficFlow> trafficFlows;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }

            Instance = this;
        }

        private IEnumerator Start()
        {
            foreach (var obj in trafficFlows)
            {
                // 각 2개씩 생성
                var enu = Enumerable.Range(0, 7).ToList();
                var targets = obj.GetTargetValues();

                for (int i = 0; i < Random.Range(1, 4); i++)
                {
                    var car = Instantiate(carPrefab, transform);
                    var pos = targets[enu[i]].position;
                    car.transform.position = new Vector3(pos.x, -1, pos.z);
                    yield return null;
                }
            }
        }
    }
}