using System;
using System.Collections;
using System.Collections.Generic;
using NomadsPlanet.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using LightType = NomadsPlanet.Utils.LightType;

namespace NomadsPlanet
{
    // 초록불 60초
    // 노랑불 5초
    // 빨간불 55초
    public class TrafficController : MonoBehaviour
    {
        [ShowInInspector]
        private List<TrafficFlow> _trafficFlows;

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
        }

        private IEnumerator TrafficFlower()
        {
            yield return new WaitForSeconds(1);
        }
    }
}