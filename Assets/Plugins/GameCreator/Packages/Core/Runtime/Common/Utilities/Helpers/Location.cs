using System;
using GameCreator.Runtime.Characters;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Serializable]
    public struct Location
    {
        public static readonly Location NONE = new Location
        {
            m_HasPosition = false,
            m_HasRotation = false
        };
        
        // ENUMS: ---------------------------------------------------------------------------------
        
        public enum Type
        {
            Position,
            Transform,
            Marker   
        }

        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private Vector3 m_Position;
        [SerializeField] private Quaternion m_Rotation;
        
        [SerializeField] private bool m_HasPosition;
        [SerializeField] private bool m_HasRotation;
        
        // PROPERTIES: ----------------------------------------------------------------------------
        
        [field: NonSerialized] private Type LocationType { get; }
        
        [field: NonSerialized] private Transform Transform { get; }
        [field: NonSerialized] private Marker Marker { get; }

        [field: NonSerialized] private Space OffsetSpace { get; }
        [field: NonSerialized] private bool m_UseTransformRotation { get; } 
        
        [field: NonSerialized] private Vector3 OffsetPosition { get; }
        [field: NonSerialized] private Quaternion OffsetRotation { get; }
        
        public bool HasPosition => this.LocationType switch
        {
            Type.Transform when this.Transform != null => true,
            Type.Marker when this.Marker != null => true,
            _ => this.m_HasPosition
        };
        
        public bool HasRotation => this.LocationType switch
        {
            Type.Marker when this.Marker != null => true,
            _ => this.m_HasRotation
        };
        
        // CONSTRUCTORS CONSTANTS: ----------------------------------------------------------------

        public Location(bool hasPosition, bool hasRotation, Vector3 position, Quaternion rotation)
            : this()
        {
            this.m_HasPosition = hasPosition;
            this.m_HasRotation = hasRotation;
            
            this.m_Position = position;
            this.m_Rotation = rotation;

            this.LocationType = Type.Position;
            this.Transform = null;
            this.Marker = null;

            this.OffsetSpace = Space.Self;
            this.OffsetPosition = Vector3.zero;
            this.OffsetRotation = Quaternion.identity;
        }
        
        public Location(Vector3 position, Quaternion rotation)
            : this(true, true, position, rotation)
        { }
        
        public Location(Vector3 position) 
            : this(true, false, position, Quaternion.identity)
        { }
        
        public Location(Quaternion rotation) 
            : this(false, true, Vector3.zero, rotation)
        { }
        
        // CONSTRUCTORS MARKERS: ------------------------------------------------------------------

        public Location(Marker marker, Space offsetSpace, Vector3 offsetPosition, Quaternion offsetRotation) 
            : this()
        {
            if (marker == null) return;

            this.m_HasPosition = true;
            this.m_HasRotation = true;
            
            this.m_Position = offsetSpace switch
            {
                Space.World => marker.transform.position + offsetPosition,
                Space.Self => marker.transform.TransformPoint(offsetPosition),
                _ => throw new ArgumentOutOfRangeException(nameof(offsetSpace))
            };
            
            this.m_Rotation = marker.transform.rotation * offsetRotation;

            this.LocationType = Type.Marker;
            this.Transform = null;
            this.Marker = marker;
            
            this.OffsetSpace = offsetSpace;
            this.OffsetPosition = offsetPosition;
            this.OffsetRotation = offsetRotation;
        }
        
        public Location(Marker marker, Vector3 offsetLocalPosition) 
            : this(marker, Space.Self, offsetLocalPosition, Quaternion.identity)
        { }
        
        public Location(Marker marker, Quaternion offsetRotation) 
            : this(marker, Space.Self, Vector3.zero, offsetRotation)
        { }

        // CONSTRUCTORS TRANSFORMS: ---------------------------------------------------------------

        public Location(Transform transform, Space offsetSpace, Vector3 offsetPosition, 
            bool applyTransformRotation, Quaternion offsetRotation)
            : this()
        {
            if (transform == null) return;

            this.m_HasPosition = true;
            this.m_HasRotation = applyTransformRotation;
            
            this.m_Position = offsetSpace switch
            {
                Space.World => transform.position + offsetPosition,
                Space.Self => transform.TransformPoint(offsetPosition),
                _ => throw new ArgumentOutOfRangeException(nameof(offsetSpace))
            };

            this.m_Rotation = transform.rotation * offsetRotation;

            this.LocationType = Type.Transform;
            this.Transform = transform;
            this.Marker = null;
            
            this.OffsetSpace = offsetSpace;
            this.m_UseTransformRotation = applyTransformRotation;
            
            this.OffsetPosition = offsetPosition;
            this.OffsetRotation = offsetRotation;
        }
        
        // +------------------------------------------------------------------+
        // | TODO: Deprecate in 6 months from now [Creation date: 17/02/2023] |
        // +------------------------------------------------------------------+
        
        [Obsolete(
            "Creating a Location using a Game Object has been deprecated. " +
            "Please, consider using the method with the complete signature"
        )]
        
        public Location(GameObject gameObject, Vector3 offsetLocalSpace, bool applyTransformRotation) 
            : this(gameObject != null ? gameObject.transform : null, Space.World, offsetLocalSpace, 
                applyTransformRotation, Quaternion.identity)
        { }

        // +------------------------------------------------------------------+
        // | TODO: Deprecate in 6 months from now [Creation date: 17/02/2023] |
        // +------------------------------------------------------------------+
        
        [Obsolete(
            "Creating a Location using a Game Object has been deprecated. " +
            "Please, consider using the method with the complete signature"
        )]
        
        public Location(Transform transform, Vector3 offsetLocalSpace, bool applyTransformRotation) 
            : this(transform, Space.World, offsetLocalSpace, applyTransformRotation, Quaternion.identity)
        { }
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public Vector3 GetPosition(GameObject target)
        {
            switch (this.LocationType)
            {
                case Type.Transform:
                    if (this.Transform == null) return Vector3.zero;
                    Character character = this.Transform.Get<Character>();
                    Vector3 position = character != null ? character.Feet : this.Transform.position;
                    return this.OffsetSpace switch
                    {
                        Space.World => position + this.OffsetPosition,
                        Space.Self => position + this.Transform.TransformDirection(this.OffsetPosition),
                        _ => throw new ArgumentOutOfRangeException(nameof(this.OffsetSpace))
                    };
                
                case Type.Marker:
                    if (this.Marker == null) return Vector3.zero;
                    return this.Marker.GetPosition(target) + this.OffsetSpace switch 
                    {
                        Space.World => this.OffsetPosition,
                        Space.Self => this.Marker.transform.TransformDirection(this.OffsetPosition), 
                        _ => throw new ArgumentOutOfRangeException(nameof(this.OffsetSpace))
                    };
                
                case Type.Position:
                    return this.m_Position;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public Quaternion GetRotation(GameObject user)
        {
            return this.LocationType switch
            {
                Type.Transform when this.Transform != null => this.m_UseTransformRotation switch
                {
                    false => this.OffsetRotation,
                    true => this.Transform.rotation * this.OffsetRotation
                },
                Type.Marker when this.Marker != null => this.Marker.GetRotation(user) * this.OffsetRotation,
                _ => this.m_Rotation
            };
        }
    }
}