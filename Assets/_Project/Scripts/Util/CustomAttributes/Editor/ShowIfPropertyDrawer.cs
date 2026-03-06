using System;
using System.Collections;
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

            var range = fieldInfo.GetCustomAttribute<RangeAttribute>(true);
            if (range != null)
            {
                DrawRange(position, property, label, range);
                return;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldShow(property)
                ? EditorGUI.GetPropertyHeight(property, label, true)
                : 0f;
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
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        private bool ShouldShow(SerializedProperty property)
        {
            var cond = (ShowIfAttribute)attribute;

            // Fast path: try relative lookup first, then absolute lookup.
            var boolProp = FindRelativeProperty(property, cond.BoolFieldName) 
                           ?? property.serializedObject.FindProperty(cond.BoolFieldName);

            if (boolProp != null && boolProp.propertyType == SerializedPropertyType.Boolean)
                return cond.Invert ? !boolProp.boolValue : boolProp.boolValue;

            // Reflection fallback on the actual owner object.
            object owner = GetOwnerObject(property);
            if (owner != null && TryGetBoolMember(owner, cond.BoolFieldName, out bool result))
                return cond.Invert ? !result : result;

            Debug.LogWarning(
                $"ShowIf: Could not find bool member '{cond.BoolFieldName}' on '{owner?.GetType().FullName ?? "null"}'.");
            return true; // fail open
        }

        private static SerializedProperty FindRelativeProperty(SerializedProperty property, string memberName)
        {
            if (property == null || string.IsNullOrEmpty(memberName))
                return null;

            string path = property.propertyPath;
            int lastDot = path.LastIndexOf('.');
            if (lastDot < 0)
                return null;

            string parentPath = path.Substring(0, lastDot);
            return property.serializedObject.FindProperty($"{parentPath}.{memberName}");
        }

        private static bool TryGetBoolMember(object obj, string name, out bool value)
        {
            value = default;

            if (obj == null || string.IsNullOrEmpty(name))
                return false;

            Type type = obj.GetType();

            while (type != null)
            {
                const BindingFlags flags =
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly;

                // field
                FieldInfo f = type.GetField(name, flags);
                if (f != null && f.FieldType == typeof(bool))
                {
                    value = (bool)f.GetValue(obj);
                    return true;
                }

                // property
                PropertyInfo p = type.GetProperty(name, flags);
                if (p != null &&
                    p.PropertyType == typeof(bool) &&
                    p.GetIndexParameters().Length == 0)
                {
                    value = (bool)p.GetValue(obj);
                    return true;
                }

                // method bool Name()
                MethodInfo m = type.GetMethod(name, flags, null, Type.EmptyTypes, null);
                if (m != null && m.ReturnType == typeof(bool))
                {
                    value = (bool)m.Invoke(obj, null);
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        // Resolves the object instance that owns the drawn property (handles nested objects + arrays + base members)
        private static object GetOwnerObject(SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;
            if (obj == null)
                return null;

            string path = property.propertyPath.Replace(".Array.data[", "[");
            string[] elements = path.Split('.');

            // Walk to parent of the final member being drawn
            for (int i = 0; i < elements.Length - 1; i++)
            {
                obj = GetValue(obj, elements[i]);
                if (obj == null)
                    return null;
            }

            return obj;
        }

        private static object GetValue(object obj, string element)
        {
            if (obj == null || string.IsNullOrEmpty(element))
                return null;

            int bracketIndex = element.IndexOf('[');
            if (bracketIndex >= 0)
            {
                string memberName = element.Substring(0, bracketIndex);
                int index = int.Parse(element.Substring(bracketIndex + 1, element.Length - bracketIndex - 2));

                object collection = GetMemberValue(obj, memberName);
                if (collection is IList list)
                {
                    if (index >= 0 && index < list.Count)
                        return list[index];
                }
                else if (collection is IEnumerable enumerable)
                {
                    int i = 0;
                    foreach (object item in enumerable)
                    {
                        if (i == index)
                            return item;
                        i++;
                    }
                }

                return null;
            }

            return GetMemberValue(obj, element);
        }

        private static object GetMemberValue(object obj, string memberName)
        {
            if (obj == null || string.IsNullOrEmpty(memberName))
                return null;

            Type type = obj.GetType();

            while (type != null)
            {
                const BindingFlags flags =
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.DeclaredOnly;

                FieldInfo field = type.GetField(memberName, flags);
                if (field != null)
                    return field.GetValue(obj);

                PropertyInfo prop = type.GetProperty(memberName, flags);
                if (prop != null && prop.GetIndexParameters().Length == 0)
                    return prop.GetValue(obj);

                type = type.BaseType;
            }

            return null;
        }
    }
}
