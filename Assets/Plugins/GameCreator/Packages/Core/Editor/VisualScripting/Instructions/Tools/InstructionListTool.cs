using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Editor.VisualScripting
{
    public class InstructionListTool : TPolymorphicListTool
    {
        private const string NAME_BUTTON_ADD = "GC-Instruction-List-Foot-Add";
        
        private const string CLASS_INSTRUCTION_RUNNING = "gc-list-item-head-running";

        private static readonly IIcon ICON_PASTE = new IconPaste(ColorTheme.Type.TextNormal);
        private static readonly IIcon ICON_PLAY = new IconPlay(ColorTheme.Type.TextNormal);

        // MEMBERS: -------------------------------------------------------------------------------

        protected Button m_ButtonAdd;
        protected Button m_ButtonPaste;
        protected Button m_ButtonPlay;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string ElementNameHead => "GC-Instruction-List-Head";
        protected override string ElementNameBody => "GC-Instruction-List-Body";
        protected override string ElementNameFoot => "GC-Instruction-List-Foot";

        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.VISUAL_SCRIPTING + "Instructions/StyleSheets/Instructions-List"
        };

        public override bool AllowReordering => true;
        public override bool AllowDuplicating => true;
        public override bool AllowDeleting  => true;
        public override bool AllowContextMenu => true;
        public override bool AllowCopyPaste => true;
        public override bool AllowInsertion => true;
        public override bool AllowBreakpoint => true;
        public override bool AllowDisable => true;
        public override bool AllowDocumentation => true;
        
        private BaseActions Actions => this.SerializedObject?.targetObject as BaseActions;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public InstructionListTool(SerializedProperty property)
            : base(property, InstructionListDrawer.NAME_INSTRUCTIONS)
        {
            this.SerializedObject.Update();

            this.OnChangePlayMode(EditorApplication.isPlaying
                ? PlayModeStateChange.EnteredPlayMode
                : PlayModeStateChange.ExitingPlayMode
            );
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            return new InstructionItemTool(this, index);
        }

        protected override void SetupHead()
        { }

        protected override void SetupFoot()
        {
            base.SetupFoot();
            
            this.m_ButtonAdd = new TypeSelectorElementInstruction(this.PropertyList, this)
            {
                name = NAME_BUTTON_ADD
            };
            
            this.m_ButtonPaste = new Button(() =>
            {
                if (!CopyPasteUtils.CanSoftPaste(typeof(Instruction))) return;
                
                int pasteIndex = this.PropertyList.arraySize;
                this.InsertItem(pasteIndex, CopyPasteUtils.SourceObjectCopy);
            })
            {
                name = "GC-Instruction-List-Foot-Button"
            };
            
            this.m_ButtonPaste.Add(new Image
            {
                image = ICON_PASTE.Texture
            });
            
            this.m_ButtonPlay = new Button(this.RunInstructions)
            {
                name = "GC-Instruction-List-Foot-Button"
            };
            
            this.m_ButtonPlay.Add(new Image
            {
                image = ICON_PLAY.Texture
            });
            
            this.m_Foot.Add(this.m_ButtonAdd);
            this.m_Foot.Add(this.m_ButtonPaste);
            this.m_Foot.Add(this.m_ButtonPlay);
            
            this.m_ButtonPlay.style.display = this.Actions != null
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }

        private void OnChangePlayMode(PlayModeStateChange state)
        {
            this.m_ButtonPlay?.SetEnabled(state == PlayModeStateChange.EnteredPlayMode);
            
            BaseActions actions = this.Actions;
            if (actions == null) return;

            actions.EventInstructionRun -= this.OnRunInstruction;
            actions.EventInstructionEndRunning -= this.OnEndRunning;
            
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                if (actions.IsRunning)
                {
                    this.OnRunInstruction(actions.RunningIndex);
                }
                
                actions.EventInstructionRun += this.OnRunInstruction;
                actions.EventInstructionEndRunning += this.OnEndRunning;
            }
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RunInstructions()
        {
            BaseActions actions = this.Actions;
            if (actions == null) return;
            
            actions.Invoke(actions.gameObject);
        }
        
        private void OnRunInstruction(int index)
        {
            foreach (VisualElement child in this.m_Body.Children())
            {
                child.RemoveFromClassList(CLASS_INSTRUCTION_RUNNING);
            }
            
            if (this.m_Body.childCount <= index) return;
            this.m_Body[index].AddToClassList(CLASS_INSTRUCTION_RUNNING);
        }
        
        private void OnEndRunning()
        {
            foreach (VisualElement child in this.m_Body.Children())
            {
                child.RemoveFromClassList(CLASS_INSTRUCTION_RUNNING);
            }
        }
    }
}