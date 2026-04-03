using System;
using _Project.Scripts.Core.Modules.Base_Class;
using _Project.Scripts.Core.Player;
using _Project.Scripts.Util.CustomAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.UI
{
    public class ModuleSectionUIComponent : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI moduleName;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private Image moduleImage;
        [SerializeField] private GameObject button;
        [SerializeField] private GameObject player1Visual;
        [SerializeField] private GameObject player2Visual;
        
        [Header("Select Settings")]
        [SerializeField] private GameObject player1ConfirmVisual;
        [SerializeField] private GameObject player2ConfirmVisual;
        
        [Header("Debug")]
        [SerializeField, ReadOnly] private Module selectedModule;

        private void Awake()
        {
            player1Visual.SetActive(false);
            player2Visual.SetActive(false);
            button.SetActive(false);
            player1ConfirmVisual.SetActive(false);
            player2ConfirmVisual.SetActive(false);
        }

        public void Initialize(Module module)
        {
            selectedModule = module;
            moduleName.text = module.name;
            description.text = module.description;
            moduleImage.sprite = module.moduleSprite;
            Color c = moduleImage.color;
            c.a = 1;
            moduleImage.color = c;
        }

        public void Confirm(PlayerData.PlayerID id)
        {
            switch (id)
            {
                case PlayerData.PlayerID.Player1:
                    player1ConfirmVisual.SetActive(true);
                    break;
                case PlayerData.PlayerID.Player2:
                    player2ConfirmVisual.SetActive(true);
                    break;
            }
        }

        public void Select(PlayerData.PlayerID playerID)
        {
            switch (playerID)
            {
                case PlayerData.PlayerID.Player1:
                    player1ConfirmVisual.SetActive(false);
                    player1Visual.SetActive(true);
                    break;
                case PlayerData.PlayerID.Player2:
                    player2ConfirmVisual.SetActive(false);
                    player2Visual.SetActive(true);
                    break;
            }
            
            button?.SetActive(true);
        }

        public void Unselect(PlayerData.PlayerID playerID)
        {
            switch (playerID)
            {
                case PlayerData.PlayerID.Player1:
                    player1ConfirmVisual.SetActive(false);
                    player1Visual.SetActive(false);
                    break;
                case PlayerData.PlayerID.Player2:
                    player2ConfirmVisual.SetActive(false);
                    player2Visual.SetActive(false);
                    break;
            }
            
            button?.SetActive(player1Visual.activeInHierarchy || player2Visual.activeInHierarchy);
        }

        public Module GetSelectedModule()
        {
            return selectedModule;
        }
    }
}
