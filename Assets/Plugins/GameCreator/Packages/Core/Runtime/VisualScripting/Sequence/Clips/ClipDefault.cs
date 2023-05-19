using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Serializable]
    public class ClipDefault : Clip
    {
        public const string NAME_INSTRUCTIONS = nameof(m_Instructions);
        
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private InstructionList m_Instructions = new InstructionList();
        
        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private GameObject m_TemplateInstructions;
        
        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ClipDefault() : base(default)
        { }
        
        public ClipDefault(float time) : base(time)
        { }

        public ClipDefault(InstructionList instructions, float time) : base(time)
        {
            this.m_Instructions = instructions;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override void OnStart(ITrack track, Args args)
        {
            base.OnStart(track, args);
            this.Run(args);
        }

        // METHODS: -------------------------------------------------------------------------------
        
        private void Run(Args args)
        {
            if (this.m_TemplateInstructions == null)
            {
                this.m_TemplateInstructions = RunInstructionsList.CreateTemplate(
                    this.m_Instructions
                );
            }
            
            _ = RunInstructionsList.Run(
                args.Clone, this.m_TemplateInstructions, 
                new RunnerConfig
                {
                    Name = "On Clip Run",
                    Location = new RunnerLocationPosition(
                        args.Self != null ? args.Self.transform.position : Vector3.zero, 
                        args.Self != null ? args.Self.transform.rotation : Quaternion.identity
                    )
                }
            );
        }
    }
}