using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Common
{
    [Obsolete(
        "The wrapper class PropertyTool is obsolete and will be removed soon. " +
        "Please upgrade your Game Creator projects to their latest version to avoid any " +
        "conflicts in the future"
    )]
    
    public class PropertyTool : VisualElement
    {
        // TODO:
        // [2022-10-30]: Unity does not keep a consistent Label width between versions.
        // Seems they have settled with 120px in future versions, but while the LTS version 
        // is the recommended one, we'll use 150px as a fallback.
        // 
        // [2023-03-10]: Now that Unity has changed how UIToolkit fields behave making them more
        // similar to IMGUI, there is no need for this wrapper class and thus is marked for
        // deprecation. Remove once all modules are upgraded to use PropertyField instead.
        
        #if UNITY_2021
        private const float LABEL_WIDTH = 150f;
        #else
        private const float LABEL_WIDTH = 120f;
        #endif

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly string m_Text;

        // PROPERTIES: ----------------------------------------------------------------------------

        public SerializedProperty Property { get; }
        public PropertyField PropertyField { get; }

        // EVENTS: --------------------------------------------------------------------------------

        public event Action<SerializedPropertyChangeEvent> EventChange;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public PropertyTool(SerializedProperty property)
        {
            this.Property = property;
            this.PropertyField = new PropertyField(property);
            
            this.m_Text = property.displayName;
            
            this.Add(this.PropertyField);
            this.Bind(property.serializedObject);
            
            this.PropertyField.RegisterValueChangeCallback(this.OnChangeValue);
            this.PropertyField.RegisterCallback<GeometryChangedEvent>(this.OnChaneGeometry);
            
            this.RegisterCallback<DetachFromPanelEvent>(_ =>
            {
                this.EventChange = null;
            });
        }

        public PropertyTool(SerializedProperty property, string text) : this(property)
        {
            this.m_Text = text;
            this.PropertyField.label = text;
            this.RegisterCallback<GeometryChangedEvent>(this.OnCompleteDraw);
        }

        // CALLBACK METHODS: ----------------------------------------------------------------------

        private void OnChangeValue(SerializedPropertyChangeEvent eventChange)
        {
            EventChange?.Invoke(eventChange);
        }
        
        private void OnChaneGeometry(GeometryChangedEvent geometryEvent)
        {
            this.RefreshSize();
        }
        
        private void OnCompleteDraw(GeometryChangedEvent geometryEvent)
        {
            this.UnregisterCallback<GeometryChangedEvent>(this.OnCompleteDraw);
            this.RefreshLabel();
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void RefreshLabel()
        {
            Label label = FirstLabel();
            if (label != null) label.text = this.m_Text; 
        }
        
        private void RefreshSize()
        {
            if (string.IsNullOrEmpty(this.m_Text)) return;
            
            Label label = FirstLabel();
            if (label != null) label.style.width = LABEL_WIDTH;
        }

        private Label FirstLabel()
        {
            return this.PropertyField.Q<Label>(className: "unity-property-field__label");
        }
    }
}