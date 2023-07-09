using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NomadsPlanet.Utils;
using LightType = NomadsPlanet.Utils.LightType;

namespace NomadsPlanet
{
    public class TrafficController : MonoBehaviour
    {
        private const int SIGN_DURATION = 30;
        
        private List<TrafficFlow> _trafficFlows;

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

        // 하나의 신호가 초록불일 때, 나머지는 빨간불 유지
        // 초록 -> 노랑 -> 빨강
        private IEnumerator TrafficCycle()
        {
            while (gameObject)
            {
                // todo: 대충 시간 은근 달라지게끔 해두기
                int duration = Random.Range(SIGN_DURATION, SIGN_DURATION + 5);
                for (int i = 0; i < _trafficFlows.Count; i++)
                {
                    for (int j = 0; j < _trafficFlows.Count; j++)
                    {
                        if (j == i) continue;
                        StartCoroutine(SetTrafficSign(_trafficFlows[j], LightType.Red, duration));
                    }

                    yield return StartCoroutine(SetTrafficSign(_trafficFlows[i], LightType.Green, duration - 5));
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