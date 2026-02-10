using UnityEngine;

namespace _Project.Scripts.Core.ApplicationQuit
{
    public class QuitGame : MonoBehaviour
    {
        public void Quit()
        {
            Debug.Log("Exiting Game...");
            Application.Quit();
        }
    }
}
