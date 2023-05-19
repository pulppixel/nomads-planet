using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Characters;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Look on Focus")]
    [Image(typeof(IconEye), ColorTheme.Type.Green, typeof(OverlayDot))]
    
    [Category("Look/Look on Focus")]
    [Description(
        "Makes the Character look at the center of the Hotspot when it's an interactive and " +
        "is focused"
    )]

    [Serializable]
    public class SpotLookFocus : SpotLook
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private IInteractive m_Interactive;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => "Character Look on Focus";

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
