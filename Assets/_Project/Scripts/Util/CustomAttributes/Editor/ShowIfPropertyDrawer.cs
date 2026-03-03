using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace _Project.Scripts.Util.CustomAttributes.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!ShouldShow(property))
                return;

            // If field has [Range], draw slider manually (because Unity won't stack drawers)
            var range = fieldInfo.GetCustomAttribute<RangeAttribute>(true);
            if (range != null)
            {
                DrawRange(position, property, label, range);
                return;
            }

            // Default draw (will include other non-drawer attributes like Header/Tooltip)
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldShow(property)
                ? EditorGUI.GetPropertyHeight(property, label, true)
                : 0;
        }
        
        private void DrawRange(Rect position, SerializedProperty property, GUIContent label, RangeAttribute range)
        {
            if (property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = EditorGUI.Slider(position, label, property.floatValue, range.min, range.max);
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.IntSlider(position, label, property.intValue, (int)range.min, (int)range.max);
            }
            else
            {
                // Range on unsupported type – fallback
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        bool ShouldShow(SerializedProperty property)
        {
            var cond = (ShowIfAttribute)attribute;

            // 1) Fast path: serialized bool field
            var boolProp = property.serializedObject.FindProperty(cond.BoolFieldName);
            if (boolProp != null && boolProp.propertyType == SerializedPropertyType.Boolean)
                return cond.Invert ? !boolProp.boolValue : boolProp.boolValue;

            // 2) Reflection fallback: field OR property OR method on the owning object
            object owner = GetOwnerObject(property);
            if (owner != null && TryGetBoolMember(owner, cond.BoolFieldName, out bool result))
                return cond.Invert ? !result : result;

            Debug.LogWarning($"ShowIf: Could not find bool member '{cond.BoolFieldName}' (field/property/method).");
            return true; // fail open
        }

        static bool TryGetBoolMember(object obj, string name, out bool value)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var t = obj.GetType();

            // field
            var f = t.GetField(name, flags);
            if (f != null && f.FieldType == typeof(bool))
            {
                value = (bool)f.GetValue(obj);
                return true;
            }

            // property
            var p = t.GetProperty(name, flags);
            if (p != null && p.PropertyType == typeof(bool) && p.GetIndexParameters().Length == 0)
            {
                value = (bool)p.GetValue(obj);
                return true;
            }

            // method bool Name()
            var m = t.GetMethod(name, flags, null, Type.EmptyTypes, null);
            if (m != null && m.ReturnType == typeof(bool))
            {
                value = (bool)m.Invoke(obj, null);
                return true;
            }

            value = default;
            return false;
        }

// Resolves the object instance that *owns* the drawn property (handles nested objects + arrays)
        static object GetOwnerObject(SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;
            if (obj == null)
                return null;

            string path = property.propertyPath.Replace(".Array.data[", "[");
            string[] elements = path.Split('.');

            for (int i = 0; i < elements.Length - 1; i++)
            {
                obj = GetValue(obj, elements[i]);
                if (obj == null)
                    return null;
            }

            return obj;
        }

        static object GetValue(object obj, string element)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            int bracket = element.IndexOf('[');
            if (bracket >= 0)
            {
                string fieldName = element.Substring(0, bracket);
                int index = int.Parse(element.Substring(bracket + 1, element.Length - bracket - 2));

                var field = obj.GetType().GetField(fieldName, flags);
                if (field == null)
                    return null;

                if (field.GetValue(obj) is System.Collections.IList list && index >= 0 && index < list.Count)
                    return list[index];

                return null;
            }
            else
            {
                var field = obj.GetType().GetField(element, flags);
                return field?.GetValue(obj);
            }
        }
    }
}
