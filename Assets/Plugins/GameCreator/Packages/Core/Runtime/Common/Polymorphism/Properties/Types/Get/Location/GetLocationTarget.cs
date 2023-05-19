using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Target")]
    [Category("Target")]

    [Image(typeof(IconTarget), ColorTheme.Type.Yellow)]
    [Description("The position and/or rotation of the targeted game object")]

    [Serializable]
    public class GetLocationTarget : PropertyTypeGetLocation
    {
        [SerializeField] private EnablerVector3 m_Position = new EnablerVector3();
        [SerializeField] private EnablerVector3 m_Rotation = new EnablerVector3();

        public override Location Get(Args args)
        {
            if (args.Target == null) return new Location();
            return new Location(
                this.m_Position.IsEnabled,
                this.m_Rotation.IsEnabled,
                this.m_Position.IsEnabled
                    ? args.Target.transform.TransformPoint(this.m_Position.Value) 
                    : Vector3.zero,
                this.m_Rotation.IsEnabled 
                    ? args.Target.transform.rotation * Quaternion.Euler(this.m_Rotation.Value)
                    : Quaternion.identity
            );
        }

        public static PropertyGetLocation Create => new PropertyGetLocation(
            new GetLocationTarget()
        );

        public override string String => "Target";
    }
}