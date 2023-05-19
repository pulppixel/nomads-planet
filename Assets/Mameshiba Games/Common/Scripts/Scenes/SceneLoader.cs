using UnityEngine;
using UnityEngine.SceneManagement;

namespace MameshibaGames.Common.Scenes
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField]
        private string sceneName;
        
        public void Load()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}