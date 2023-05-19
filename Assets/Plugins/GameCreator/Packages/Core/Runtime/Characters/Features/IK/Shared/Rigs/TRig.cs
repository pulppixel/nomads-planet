using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Characters.IK
{
    [Title("IK Rig")]
    
    [Serializable]
    public abstract class TRig : TPolymorphicItem<TRig>
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private bool m_IsActive = true;

        // PROPERTIES: ----------------------------------------------------------------------------

        [field: NonSerialized] protected Args Args { get; private set; }

        [field: NonSerialized] public Character Character { get; private set; }
        [field: NonSerialized] public Animator Animator { get; private set; }

        public bool IsActive
        {
            get
            {
                if (this.DisableOnBusy && this.Character.Busy.IsBusy) return false;
                return m_IsActive && this.IsEnabled && !this.Character.IsDead;
            }
            set => m_IsActive = value;
        }

        // ABSTRACT PROPERTIES: -------------------------------------------------------------------
        
        public abstract override string Title { get; }
        public abstract string Name { get; }
        
        public abstract bool RequiresHuman { get; }
        public abstract bool DisableOnBusy { get; }

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public bool OnStartup(Character character)
        {
            this.Args = new Args(character);

            this.Character = character;
            this.Animator = this.Character.Animim.Animator;
            
            return this.DoStartup(character);
        }

        public bool OnEnable(Character character)
        {
            return this.DoEnable(character);
        }

        public bool OnDisable(Character character)
        {
            return this.DoDisable(character);
        }
        
        public abstract bool OnUpdate(Character character);

        public void OnDrawGizmos(Character character)
        {
            this.DoDrawGizmos(character);
        }
        
        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual bool DoStartup(Character character)
        {
            return false;
        }

        protected virtual bool DoEnable(Character character)
        {
            return false;
        }

        protected virtual bool DoDisable(Character character)
        {
            return false;
        }

        protected virtual bool DoUpdate(Character character)
        {
            return false;
        }
        
        protected virtual void DoDrawGizmos(Character character)
        { }
    }
}