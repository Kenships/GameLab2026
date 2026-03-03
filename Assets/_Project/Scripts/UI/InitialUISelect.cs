using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Project.Scripts.UI
{
    public class InitialUISelect : MonoBehaviour
    {
        private void Awake()
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        private void Update()
        {
            if (!EventSystem.current.currentSelectedGameObject)
                EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
