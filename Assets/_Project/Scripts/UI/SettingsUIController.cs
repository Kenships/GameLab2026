using System;
using Knot.Localization;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class SettingsUIController : MonoBehaviour
    {
        [SerializeField] Button[] languageButtons;
        [SerializeField] Button[] hapticsButtons;
        
        [SerializeField] private IntVariable language;
        [SerializeField] private FloatVariable hapticStrength;

        private void Start()
        {
            languageButtons[language.Value].onClick.Invoke();
            languageButtons[language.Value].Select();
            
            switch (hapticStrength.Value)
            {
                case > 0.5f:
                    hapticsButtons[2].onClick.Invoke();
                    break;
                case > 0f:
                    hapticsButtons[1].onClick.Invoke();
                    break;
                default:
                    hapticsButtons[0].onClick.Invoke();
                    break;
            }
        }

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
