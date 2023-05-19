using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Target")]
    [Category("Target")]
    
    [Image(typeof(IconTarget), ColorTheme.Type.Yellow)]
    [Description("Returns the position of the targeted object")]

    [Serializable] [HideLabelsInEditor]
    public class GetPositionTarget : PropertyTypeGetPosition
    {
        public override Vector3 Get(Args args)
        {
            return args.Target != null ? args.Target.transform.position : default;
        }
        
        public static PropertyGetPosition Create() => new PropertyGetPosition(
            new GetPositionTarget()
        );

        public override string String => "Target";
    }
}