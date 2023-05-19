using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(RunInstructionsList))]
    public class RunInstructionsListDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty instructions = property.FindPropertyRelative("m_Instructions");
            return new PropertyField(instructions);
        }
    }
}