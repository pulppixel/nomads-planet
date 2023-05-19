using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Game Object")]
    [Category("Game Objects/Game Object")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue)]
    [Description("The position and rotation of a Game Object")]

    [Serializable]
    public class GetLocationGameObject : PropertyTypeGetLocation
    {
        [SerializeField]
        private PropertyGetGameObject m_GameObject = GetGameObjectInstance.Create();
        
        [SerializeField] private bool m_Rotate = true;
        
        public override Location Get(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return new Location();

            Marker marker = gameObject.Get<Marker>();
            return marker != null 
                ? new Location(marker, Vector3.zero) 
                : new Location(gameObject.transform, Space.Self, Vector3.zero, this.m_Rotate, Quaternion.identity);
        }

        public static PropertyGetLocation Create => new PropertyGetLocation(
            new GetLocationGameObject()
        );

        public override string String => $"{this.m_GameObject}";
    }
}