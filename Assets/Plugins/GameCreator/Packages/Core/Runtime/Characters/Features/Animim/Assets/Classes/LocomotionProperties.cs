using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace GameCreator.Runtime.Characters
{
    [Serializable]
    public class LocomotionProperties
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] private EnablerFloat m_Speed = new EnablerFloat(4f);
        [SerializeField] private EnablerFloat m_Rotation = new EnablerFloat(1800f);
        
        [SerializeField] private EnablerFloat m_Mass = new EnablerFloat(80f);
        [SerializeField] private EnablerFloat m_Height = new EnablerFloat(2f);
        [SerializeField] private EnablerFloat m_Radius = new EnablerFloat(0.2f);
        
        [SerializeField] private EnablerFloat m_GravityUpwards = new EnablerFloat(-9.81f);
        [SerializeField] private EnablerFloat m_GravityDownwards = new EnablerFloat(-9.81f);
        [SerializeField] private EnablerFloat m_TerminalVelocity = new EnablerFloat(-53f);

        [SerializeField] private EnablerBool m_UseAcceleration = new EnablerBool(true);
        [SerializeField] private EnablerFloat m_Acceleration = new EnablerFloat(10f);
        [SerializeField] private EnablerFloat m_Deceleration = new EnablerFloat(4f);
        
        [SerializeField] private EnablerBool m_CanJump = new EnablerBool(true);
        [SerializeField] private EnablerInt m_AirJumps = new EnablerInt(0);
        [SerializeField] private EnablerFloat m_JumpForce = new EnablerFloat(5f);
        [SerializeField] private EnablerFloat m_JumpCooldown = new EnablerFloat(0.5f);
        
        [SerializeField] private EnablerInt m_DashInSuccession = new EnablerInt(0);
        [SerializeField] private EnablerBool m_DashInAir = new EnablerBool(false);
        [SerializeField] private EnablerFloat m_DashCooldown = new EnablerFloat(1f);
        
        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Setup(Character character)
        {
            if (character == null) return;
            
            if (this.m_Speed.IsEnabled) character.Motion.LinearSpeed = this.m_Speed.Value;
            if (this.m_Rotation.IsEnabled) character.Motion.AngularSpeed = this.m_Rotation.Value;
            
            if (this.m_Mass.IsEnabled) character.Motion.Mass = this.m_Mass.Value;
            if (this.m_Height.IsEnabled) character.Motion.Height = this.m_Height.Value;
            if (this.m_Radius.IsEnabled) character.Motion.Radius = this.m_Radius.Value;
            
            if (this.m_GravityUpwards.IsEnabled) character.Motion.GravityUpwards = this.m_GravityUpwards.Value;
            if (this.m_GravityDownwards.IsEnabled) character.Motion.GravityDownwards = this.m_GravityDownwards.Value;
            if (this.m_TerminalVelocity.IsEnabled) character.Motion.TerminalVelocity = this.m_TerminalVelocity.Value;

            if (this.m_UseAcceleration.IsEnabled) character.Motion.UseAcceleration = this.m_UseAcceleration.Value;
            if (this.m_Acceleration.IsEnabled) character.Motion.Acceleration = this.m_Acceleration.Value;
            if (this.m_Deceleration.IsEnabled) character.Motion.Deceleration = this.m_Deceleration.Value;
            
            if (this.m_CanJump.IsEnabled) character.Motion.CanJump = this.m_CanJump.Value;
            if (this.m_AirJumps.IsEnabled) character.Motion.AirJumps = this.m_AirJumps.Value;
            if (this.m_JumpForce.IsEnabled) character.Motion.JumpForce = this.m_JumpForce.Value;
            if (this.m_JumpCooldown.IsEnabled) character.Motion.JumpCooldown = this.m_JumpCooldown.Value;
            
            if (this.m_DashInSuccession.IsEnabled) character.Motion.DashInSuccession = this.m_DashInSuccession.Value;
            if (this.m_DashInAir.IsEnabled) character.Motion.DashInAir = this.m_DashInAir.Value;
            if (this.m_DashCooldown.IsEnabled) character.Motion.DashCooldown = this.m_DashCooldown.Value;
        }
    }
}