using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameCreator.Runtime.VisualScripting
{
    [Version(0, 0, 1)]
    
    [Title("Change Hotspot Radius")]
    [Description("Changes the radius of a Hotspot")]

    [Category("Logic/Change Hotspot Radius")]

    [Parameter(
        "Hotspot",
        "The Hotspot object that changes its radius"
    )]

    [Parameter(
        "Radius",
        "The new Hotspot radius"
    )]
    
    [Keywords("Execute", "Call", "Instruction", "Action")]
    [Image(typeof(IconHotspot), ColorTheme.Type.Yellow)]
    
    [Serializable]
    public class InstructionLogicHotspotRadius : Instruction
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private PropertyGetGameObject m_Hotspot = GetGameObjectHotspot.Create();
        [SerializeField] private PropertyGetDecimal m_Radius = GetDecimalDecimal.Create(5f);
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => $"Radius {this.m_Hotspot} = {this.m_Radius}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override Task Run(Args args)
        {
            Hotspot hotspot = this.m_Hotspot.Get<Hotspot>(args);
            if (hotspot != null) hotspot.Radius = (float) this.m_Radius.Get(args);

            return DefaultResult;
        }
    }
}