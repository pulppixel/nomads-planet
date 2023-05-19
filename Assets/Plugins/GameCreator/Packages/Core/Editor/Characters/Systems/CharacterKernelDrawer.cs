using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Editor.Common;

namespace GameCreator.Editor.Characters
{
    [CustomPropertyDrawer(typeof(CharacterKernel), true)]
    public class CharacterKernelDrawer : PropertyDrawer
    {
        private const string PATH_STYLES = EditorPaths.CHARACTERS + "StyleSheets/";

        private const string PLAYER = "m_Player";
        private const string MOTION = "m_Motion";
        private const string DRIVER = "m_Driver";
        private const string FACING = "m_Facing";
        private const string ANIMIM = "m_Animim";

        // CREATE PROPERTY: -----------------------------------------------------------------------

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new VisualElement();
            VisualElement body = new VisualElement();

            root.Add(body);
            
            root.AddToClassList("gc-character-kernel-root");
            body.AddToClassList("gc-character-kernel-body");

            string customUSS = PathUtils.Combine(PATH_STYLES, "Kernel");
            StyleSheet[] styleSheets = StyleSheetUtils.Load(customUSS);
            foreach (StyleSheet sheet in styleSheets) root.styleSheets.Add(sheet);
            
            this.BuildBody(property, body);

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Character character = property.serializedObject.targetObject as Character;
                if (character != null)
                {
                    character.Kernel.EventChangePlayer += () => this.BuildBody(property, body);
                    character.Kernel.EventChangeMotion += () => this.BuildBody(property, body);
                    character.Kernel.EventChangeFacing += () => this.BuildBody(property, body);
                    character.Kernel.EventChangeDriver += () => this.BuildBody(property, body);
                    character.Kernel.EventChangeAnimim += () => this.BuildBody(property, body);
                }
            }

            return root;
        }

        private void BuildBody(SerializedProperty property, VisualElement body)
        {
            body.Clear();
            
            if (property == null) return;
            
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();

            SerializedProperty propertyPlayer = property.FindPropertyRelative(PLAYER);
            SerializedProperty propertyMotion = property.FindPropertyRelative(MOTION);
            SerializedProperty propertyDriver = property.FindPropertyRelative(DRIVER);
            SerializedProperty propertyFacing = property.FindPropertyRelative(FACING);
            SerializedProperty propertyAnimim = property.FindPropertyRelative(ANIMIM);

            PropertyField fieldPlayer = new PropertyField(propertyPlayer);
            PropertyField fieldMotion = new PropertyField(propertyMotion);
            PropertyField fieldDriver = new PropertyField(propertyDriver);
            PropertyField fieldFacing = new PropertyField(propertyFacing);
            PropertyField fieldAnimim = new PropertyField(propertyAnimim);

            body.Add(fieldPlayer);
            body.Add(fieldMotion);
            body.Add(fieldDriver);
            body.Add(fieldFacing);
            body.Add(fieldAnimim);

            fieldPlayer.Bind(property.serializedObject);
            fieldMotion.Bind(property.serializedObject);
            fieldDriver.Bind(property.serializedObject);
            fieldFacing.Bind(property.serializedObject);
            fieldAnimim.Bind(property.serializedObject);

            VisualElement errors = new VisualElement();
            body.Add(errors);
            
            this.CheckForceUnits(errors, property);

            fieldPlayer.RegisterValueChangeCallback(_ => this.CheckForceUnits(errors, property));
            fieldMotion.RegisterValueChangeCallback(_ => this.CheckForceUnits(errors, property));
            fieldDriver.RegisterValueChangeCallback(_ => this.CheckForceUnits(errors, property));
            fieldFacing.RegisterValueChangeCallback(_ => this.CheckForceUnits(errors, property));
            fieldAnimim.RegisterValueChangeCallback(_ => this.CheckForceUnits(errors, property));
        }
        
        // VERIFY UNITS: --------------------------------------------------------------------------
        
        private void CheckForceUnits(VisualElement content, SerializedProperty propertyKernel)
        {
            content.Clear();

            ICharacterKernel kernel = propertyKernel.GetValue<ICharacterKernel>();
            if (kernel == null)
            {
                content.Add(new ErrorMessage("Kernel cannot be null"));
                return;
            }
            
            if (kernel.Player == null)
            {
                content.Add(new ErrorMessage("Player unit cannot be null"));
                return;
            }
            
            if (kernel.Motion == null)
            {
                content.Add(new ErrorMessage("Motion unit cannot be null"));
                return;
            }
            
            if (kernel.Driver == null)
            {
                content.Add(new ErrorMessage("Driver unit cannot be null"));
                return;
            }
            
            if (kernel.Facing == null)
            {
                content.Add(new ErrorMessage("Rotation unit cannot be null"));
                return;
            }
            
            if (kernel.Animim == null)
            {
                content.Add(new ErrorMessage("Animation unit cannot be null"));
                return;
            }
            
            IUnitCommon player = propertyKernel.FindPropertyRelative(PLAYER).GetValue<IUnitCommon>();
            IUnitCommon motion = propertyKernel.FindPropertyRelative(MOTION).GetValue<IUnitCommon>();
            IUnitCommon driver = propertyKernel.FindPropertyRelative(DRIVER).GetValue<IUnitCommon>();
            IUnitCommon facing = propertyKernel.FindPropertyRelative(FACING).GetValue<IUnitCommon>();
            IUnitCommon animim = propertyKernel.FindPropertyRelative(ANIMIM).GetValue<IUnitCommon>();
            
            this.CheckForceUnit(content, player, kernel);
            this.CheckForceUnit(content, motion, kernel);
            this.CheckForceUnit(content, driver, kernel);
            this.CheckForceUnit(content, facing, kernel);
            this.CheckForceUnit(content, animim, kernel);
        }

        private void CheckForceUnit(VisualElement body, IUnitCommon unit, ICharacterKernel kernel)
        {
            if (unit.ForcePlayer != null && unit.ForcePlayer != kernel.Player.GetType())
            {
                string unitName = TypeUtils.GetTitleFromType(unit.GetType());
                string forceName = TypeUtils.GetTitleFromType(unit.ForcePlayer);
                body.Add(new ErrorMessage($"{unitName} requires Player of type {forceName}"));
            }
            
            if (unit.ForceMotion != null && unit.ForceMotion != kernel.Motion.GetType())
            {
                string unitName = TypeUtils.GetTitleFromType(unit.GetType());
                string forceName = TypeUtils.GetTitleFromType(unit.ForceMotion);
                body.Add(new ErrorMessage($"{unitName} requires Motion of type {forceName}"));
            }
            
            if (unit.ForceDriver != null && unit.ForceDriver != kernel.Driver.GetType())
            {
                string unitName = TypeUtils.GetTitleFromType(unit.GetType());
                string forceName = TypeUtils.GetTitleFromType(unit.ForceDriver);
                body.Add(new ErrorMessage($"{unitName} requires Driver of type {forceName}"));
            }
            
            if (unit.ForceFacing != null && unit.ForceFacing != kernel.Facing.GetType())
            {
                string unitName = TypeUtils.GetTitleFromType(unit.GetType());
                string forceName = TypeUtils.GetTitleFromType(unit.ForceFacing);
                body.Add(new ErrorMessage($"{unitName} requires Rotation of type {forceName}"));
            }
            
            if (unit.ForceAnimim != null && unit.ForceAnimim != kernel.Animim.GetType())
            {
                string unitName = TypeUtils.GetTitleFromType(unit.GetType());
                string forceName = TypeUtils.GetTitleFromType(unit.ForceAnimim);
                body.Add(new ErrorMessage($"{unitName} requires Animation of type {forceName}"));
            }
        }
    }
}