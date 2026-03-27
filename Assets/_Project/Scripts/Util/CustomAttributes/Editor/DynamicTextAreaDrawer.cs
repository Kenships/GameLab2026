namespace _Project.Scripts.Util.CustomAttributes.Editor
{
    using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DynamicTextAreaAttribute))]
public class DynamicTextAreaDrawer : PropertyDrawer
{
    private const float VerticalPadding = 4f;
    private const float WidthPadding = 2f;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        var attr = (DynamicTextAreaAttribute)attribute;

        float labelHeight = EditorGUIUtility.singleLineHeight;
        float minTextHeight = EditorGUIUtility.singleLineHeight * attr.minLines;

        float inspectorWidth = EditorGUIUtility.currentViewWidth;
        float textWidth = Mathf.Max(50f, inspectorWidth - EditorGUIUtility.labelWidth - 35f - WidthPadding);

        string text = string.IsNullOrEmpty(property.stringValue) ? " " : property.stringValue;
        float textHeight = EditorStyles.textArea.CalcHeight(new GUIContent(text), textWidth);

        return labelHeight + VerticalPadding + Mathf.Max(minTextHeight, textHeight);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "Use [DynamicTextArea] with string fields only.");
            return;
        }

        var attr = (DynamicTextAreaAttribute)attribute;

        EditorGUI.BeginProperty(position, label, property);

        Rect labelRect = new Rect(
            position.x,
            position.y,
            position.width,
            EditorGUIUtility.singleLineHeight
        );

        EditorGUI.PrefixLabel(labelRect, label);

        float textY = position.y + EditorGUIUtility.singleLineHeight + VerticalPadding;
        float minTextHeight = EditorGUIUtility.singleLineHeight * attr.minLines;

        Rect textRect = new Rect(
            position.x,
            textY,
            position.width,
            Mathf.Max(minTextHeight, position.height - EditorGUIUtility.singleLineHeight - VerticalPadding)
        );

        property.stringValue = EditorGUI.TextArea(textRect, property.stringValue);

        EditorGUI.EndProperty();
    }
}
}
