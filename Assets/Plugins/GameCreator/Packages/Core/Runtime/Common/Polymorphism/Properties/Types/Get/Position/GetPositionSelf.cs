using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Self")]
    [Category("Self")]
    
    [Image(typeof(IconSelf), ColorTheme.Type.Yellow)]
    [Description("Returns the position of the caller")]

    [Serializable] [HideLabelsInEditor]
    public class GetPositionSelf : PropertyTypeGetPosition
    {
        public override Vector3 Get(Args args)
        {
            return args.Self != null ? args.Self.transform.position : default;
        }
        
        public static PropertyGetPosition Create() => new PropertyGetPosition(
            new GetPositionSelf()
        );

        public override string String => "Self";
    }
}