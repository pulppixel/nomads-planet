using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Cursor")]
    [Image(typeof(IconCursor), ColorTheme.Type.Yellow)]
    
    [Category("Cursor")]
    [Description("Changes the cursor image when hovering the Hotspot")]

    [Serializable]
    public class SpotCursor : Spot
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] protected Texture2D m_Texture;
        [SerializeField] protected Vector2 m_Origin = Vector2.zero;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "Change Cursor to {0}",
            this.m_Texture != null ? this.m_Texture.name : "(none)"
        );
        
        [field: NonSerialized] private bool IsPointerHovering { get; set; }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnPointerEnter(Hotspot hotspot)
        {
            base.OnPointerEnter(hotspot);
            
            this.IsPointerHovering = true;
            this.RefreshCursor(hotspot.IsActive && this.IsPointerHovering);
        }

        public override void OnPointerExit(Hotspot hotspot)
        {
            base.OnPointerExit(hotspot);
            
            this.IsPointerHovering = false;
            this.RefreshCursor(hotspot.IsActive && this.IsPointerHovering);
        }

        public override void OnDisable(Hotspot hotspot)
        {
            base.OnDisable(hotspot);

            if (hotspot.IsActive && this.IsPointerHovering) this.RefreshCursor(false);
            this.IsPointerHovering = false;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RefreshCursor(bool customCursor)
        {
            switch (customCursor)
            {
                case true:
                    Cursor.SetCursor(this.m_Texture, this.m_Origin, CursorMode.Auto);
                    break;
                
                case false:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
            }
        }
    }
}