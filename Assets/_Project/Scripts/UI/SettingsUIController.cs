using Knot.Localization;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class SettingsUIController : MonoBehaviour
    {
        [SerializeField] private IntVariable language;
        [SerializeField] private FloatVariable hapticStrength;

        public void SetLanguage(int languageIndex)
        {
            language.Value = languageIndex;
            KnotLocalization.Manager.LoadLanguage(KnotLocalization.Manager.Languages[languageIndex]);
        }

        public void SetHapticStrength(float strength)
        {
            hapticStrength.Value = strength;
        }
    }
}
