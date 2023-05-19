using System;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Characters
{
    [Title("None")]
    [Category("None")]
    
    [Image(typeof(IconEmpty), ColorTheme.Type.TextNormal)]
    [Description("Do not use any kind of axonometric processing")]
    
    [Serializable]
    public class AxonometryNone : TAxonometry
    { }
}