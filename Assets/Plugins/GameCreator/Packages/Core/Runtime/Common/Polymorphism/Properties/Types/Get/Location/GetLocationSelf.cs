using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Self")]
    [Category("Self")]

    [Image(typeof(IconSelf), ColorTheme.Type.Yellow)]
    [Description("The position and/or rotation of the caller game object")]

    [Serializable]
    public class GetLocationSelf : PropertyTypeGetLocation
    {
        [SerializeField] private EnablerVector3 m_Position = new EnablerVector3();
        [SerializeField] private EnablerVector3 m_Rotation = new EnablerVector3();

        public override Location Get(Args args)
        {
            if (args.Self == null) return new Location();
            return new Location(
                this.m_Position.IsEnabled,
                this.m_Rotation.IsEnabled,
                this.m_Position.IsEnabled
                    ? args.Self.transform.TransformPoint(this.m_Position.Value) 
                    : Vector3.zero,
                this.m_Rotation.IsEnabled 
                    ? args.Self.transform.rotation * Quaternion.Euler(this.m_Rotation.Value)
                    : Quaternion.identity
            );
        }

        public static PropertyGetLocation Create => new PropertyGetLocation(
            new GetLocationSelf()
        );

        public override string String => "Self";
    }
}