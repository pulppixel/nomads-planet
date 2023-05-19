using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public class PropertyGetGameObject : TPropertyGet<PropertyTypeGetGameObject, GameObject>
    {
        public PropertyGetGameObject() : base(new GetGameObjectInstance())
        { }

        public PropertyGetGameObject(PropertyTypeGetGameObject defaultType) : base(defaultType)
        { }

        public T Get<T>(Args args) where T : Component
        {
            return this.m_Property.Get<T>(args);
        }

        public T Get<T>(GameObject target) where T : Component
        {
            return this.m_Property.Get<T>(target);
        }
        
        public T Get<T>(Component component) where T : Component
        {
            return this.m_Property.Get<T>(component);
        }
        
        // EDITOR: --------------------------------------------------------------------------------

        /// <summary>
        /// EDITOR ONLY: This is used by editor scripts that require an optional scene reference,
        /// if the value is not dynamic, but constant. For example, the GetGameObjectInstance.
        /// </summary>
        public GameObject SceneReference => this.m_Property.SceneReference;
    }
}