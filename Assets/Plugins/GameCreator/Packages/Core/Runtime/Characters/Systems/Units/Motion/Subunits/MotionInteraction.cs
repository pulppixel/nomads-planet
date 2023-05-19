using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class MotionInteraction
    {
        private static readonly Color COLOR_GIZMOS = new Color(0f, 1f, 0f, 0.05f);
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] protected float m_Radius;
        [SerializeField] protected InteractionMode m_Mode;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public float Radius
        {
            get => this.m_Radius;
            set => this.m_Radius = value;
        }
        
        public InteractionMode Mode
        {
            get => this.m_Mode;
            set => this.m_Mode = value;
        }

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public MotionInteraction()
        {
            this.m_Radius = 2f;
            this.m_Mode = new InteractionMode();
        }

        // GIZMOS: --------------------------------------------------------------------------------
        
        public void DrawGizmos(Character character)
        {
            this.m_Mode.DrawGizmos(character);
            
            Gizmos.color = COLOR_GIZMOS;
            GizmosExtension.Octahedron(
                character.transform.position,
                Quaternion.identity,
                this.m_Radius
            );
        }
    }
}
