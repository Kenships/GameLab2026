using UnityEngine;

namespace _Project.Scripts.Util.CustomAttributes
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public string BoolFieldName;
        public bool Invert;

        public ShowIfAttribute(string boolFieldName, bool invert = false)
        {
            BoolFieldName = boolFieldName;
            Invert = invert;
        }
    }
}
