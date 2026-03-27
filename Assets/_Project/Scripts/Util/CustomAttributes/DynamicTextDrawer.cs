namespace _Project.Scripts.Util.CustomAttributes
{
    using UnityEngine;

    public class DynamicTextAreaAttribute : PropertyAttribute
    {
        public readonly float minLines;

        public DynamicTextAreaAttribute(float minLines = 2f)
        {
            this.minLines = minLines;
        }
    }
}
