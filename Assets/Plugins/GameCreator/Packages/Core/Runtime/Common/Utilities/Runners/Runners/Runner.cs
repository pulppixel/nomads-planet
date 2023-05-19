using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public abstract class Runner : MonoBehaviour
    {
        protected const HideFlags TEMPLATE_FLAGS = HideFlags.HideInHierarchy;
        protected const HideFlags INSTANCE_FLAGS = HideFlags.None;
        
        /* Implement exposed member: m_Value */
    }
}