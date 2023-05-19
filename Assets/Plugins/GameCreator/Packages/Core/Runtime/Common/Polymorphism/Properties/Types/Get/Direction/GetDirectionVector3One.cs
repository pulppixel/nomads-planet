using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Vector One")]
    [Category("Vectors/Vector One")]
    
    [Image(typeof(IconVector3), ColorTheme.Type.Green)]
    [Description("A Vector3 that has a unit in all components")]

    [Serializable] [HideLabelsInEditor]
    public class GetDirectionVector3One : PropertyTypeGetDirection
    {
        public override Vector3 Get(Args args) => Vector3.one;
        public override Vector3 Get(GameObject gameObject) => Vector3.one;

        public static PropertyGetDirection Create() => new PropertyGetDirection(
            new GetDirectionVector3One()
        );

        public override string String => "(1,1,1)";
    }
}