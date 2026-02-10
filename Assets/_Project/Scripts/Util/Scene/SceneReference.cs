using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Util.Scene
{
    [Serializable]
    public class SceneReference
    {
        [SerializeField] private int buildIndex;

        public int BuildIndex
        {
            get => buildIndex;
            set => buildIndex = value;
        }
        

        public string SceneName
        {
            get
            {
                string path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
                return Path.GetFileNameWithoutExtension(path);
            }
        }
    }
}
