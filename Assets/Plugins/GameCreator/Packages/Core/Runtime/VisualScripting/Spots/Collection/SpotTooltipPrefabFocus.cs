using System;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Activate Object on Focus")]
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue, typeof(OverlayDot))]
    
    [Category("Tooltips/Activate Object on Focus")]
    [Description(
        "Creates or Activates a prefab game object when the Interactive object is focused " +
        "and deactivates it when its unfocused"
    )]

    [Serializable]
    public class SpotTooltipPrefabFocus : SpotTooltipPrefab
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private IInteractive m_Interactive;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "Show {0} on Focus",
            this.m_Prefab != null ? this.m_Prefab.name : "(none)"
        );

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnAwake(Hotspot hotspot)
        {
            base.OnAwake(hotspot);
            this.m_Interactive = InteractionTracker.Require(hotspot.gameObject);
        }

        protected override bool EnableInstance(Hotspot hotspot)
        {
            bool isActive = base.EnableInstance(hotspot);
            
            Character character = hotspot.Target.Get<Character>();
            bool hasFocus = character != null && 
                            character.Interaction.Target?.Instance == hotspot.gameObject;

            return isActive && hasFocus && !this.m_Interactive.IsInteracting;
        }
    }
}