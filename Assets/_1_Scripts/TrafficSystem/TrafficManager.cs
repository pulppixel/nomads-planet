using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NomadsPlanet
{
    public class TrafficManager : MonoBehaviour
    {
        [Flags]
        internal enum TrafficType
        {
            Left = 1,
            Right = 2,
            Forward = 4,
        }
        
        
    }
}