using Knot.Localization;
using Obvious.Soap;
using UnityEngine;

namespace _Project.Scripts.Core
{
    public class LanguageInitializer : MonoBehaviour
    {
        [SerializeField] private IntVariable languageIndex;
        private void Awake()
        {
            KnotLocalization.Manager.LoadLanguage(KnotLocalization.Manager.Languages[languageIndex.Value]);
        }
    }
}
