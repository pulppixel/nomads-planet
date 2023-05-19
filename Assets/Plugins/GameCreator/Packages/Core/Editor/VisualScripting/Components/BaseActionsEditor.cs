using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace GameCreator.Editor.VisualScripting
{
    public abstract class BaseActionsEditor : UnityEditor.Editor
    {
        protected static readonly StyleLength DEFAULT_MARGIN_TOP = new StyleLength(5);

        protected void CreateInstructionsGUI(VisualElement container)
        {
            SerializedProperty instructions = this.serializedObject.FindProperty("m_Instructions");
            container.Add(new PropertyField(instructions));
        }
    }
}