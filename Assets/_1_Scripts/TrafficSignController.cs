using System;
using System.Collections;
using System.Collections.Generic;
using NomadsPlanet.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Sirenix.Serialization;
using UnityEngine.Serialization;

namespace NomadsPlanet
{
    public class TrafficSignController : MonoBehaviour
    {
        [ShowInInspector] private TrafficSign _trafficSign;
        [Required] public GameObject[] trafficSigns;

        // 3: green
        // 4: yellow
        // 5: red
        [ShowInInspector]
        public void SetTrafficSign(TrafficSign seTrafficSign)
        {
            _trafficSign = seTrafficSign;

            for (int i = 0; i < 3; i++)
            {
                trafficSigns[i].SetActive(i == (int)seTrafficSign);
            }
        }
    }
}