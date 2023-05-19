using GameCreator.Runtime.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [CustomPropertyDrawer(typeof(RunConditionsList))]
    public class RunConditionsListDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty conditions = property.FindPropertyRelative("m_Conditions");
            return new PropertyField(conditions);
        }
    }
}