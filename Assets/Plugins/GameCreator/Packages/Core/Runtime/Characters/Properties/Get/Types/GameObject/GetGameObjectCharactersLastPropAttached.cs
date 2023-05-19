using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("Last Prop Attached")]
    [Category("Characters/Last Prop Attached")]
    
    [Image(typeof(IconTennis), ColorTheme.Type.Yellow, typeof(OverlayPlus))]
    [Description("Reference to the latest Prop instance attached to a Character")]

    [Serializable] [HideLabelsInEditor]
    public class GetGameObjectCharactersLastPropAttached : PropertyTypeGetGameObject
    {
        public override GameObject Get(Args args)
        {
            return Props.LastPropAttached;
        }

        public override GameObject Get(GameObject gameObject)
        {
            return Props.LastPropAttached;
        }

        public override string String => "Last Prop Attached";
        
        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectCharactersLastPropAttached()
        );
    }
}