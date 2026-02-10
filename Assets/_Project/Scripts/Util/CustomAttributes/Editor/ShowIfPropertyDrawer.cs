using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Util.CustomAttributes.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldShow(property)
                ? EditorGUI.GetPropertyHeight(property, label, true)
                : 0;
        }

        bool ShouldShow(SerializedProperty property)
        {
            ShowIfAttribute cond = (ShowIfAttribute)attribute;

            SerializedProperty boolProp =
                property.serializedObject.FindProperty(cond.BoolFieldName);

            if (boolProp == null || boolProp.propertyType != SerializedPropertyType.Boolean)
            {
                Debug.LogWarning($"ShowIf: Could not find bool field '{cond.BoolFieldName}'");
                return true;
            }

            return cond.Invert ? !boolProp.boolValue : boolProp.boolValue;
        }
    }
}
