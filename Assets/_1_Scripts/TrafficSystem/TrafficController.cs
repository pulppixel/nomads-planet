using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NomadsPlanet.Utils;
using Sirenix.OdinInspector;
using LightType = NomadsPlanet.Utils.LightType;

namespace NomadsPlanet
{
    // 초록불 60초
    // 노랑불 5초
    // 나머지는 무조건 빨간불
    public class TrafficController : MonoBehaviour
    {
        [ShowInInspector] private List<TrafficFlow> _trafficFlows;

        [Button]
        private void Awake()
        {
            _trafficFlows = new List<TrafficFlow>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var flow = transform.GetChild(i).GetComponent<TrafficFlow>();
                _trafficFlows.Add(flow);
            }

            _trafficFlows.ShuffleList();
        }

        private void Start()
        {
            foreach (var flow in _trafficFlows)
            {
                flow.SetLightType(LightType.Red);
            }

            StartCoroutine(TrafficCycle());
        }

        private IEnumerator TrafficCycle()
        {
            while (gameObject)
            {
                for (int i = 0; i < _trafficFlows.Count; i++)
                {
                    for (int j = 0; j < _trafficFlows.Count; j++)
                    {
                        if (j == i) continue;
                        StartCoroutine(SetTrafficSign(_trafficFlows[j], LightType.Red, 35));
                    }

                    yield return StartCoroutine(SetTrafficSign(_trafficFlows[i], LightType.Green, 30));
                    yield return StartCoroutine(SetTrafficSign(_trafficFlows[i], LightType.Yellow, 5));
                }
            }
        }

        private static IEnumerator SetTrafficSign(TrafficFlow trafficFlow, LightType lightType, int duration)
        {
            trafficFlow.SetLightType(lightType);
            yield return new WaitForSeconds(duration);
        }
    }
}