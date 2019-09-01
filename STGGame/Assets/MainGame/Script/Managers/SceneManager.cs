
using System.IO;
using THGame.Package;
using UnityEngine;

namespace STGGame
{
    //场景管理器
    public class SceneManager : MonoSingleton<SceneManager>
    {
        public void AddScene(string name)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
           
        }

        public void RemoveScene(string name)
        {

        }

        public void LoadLevelScene(string uid)
        {
            var sceneName = AssetManager.GetInstance().LoadLevel(uid);
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
    }
}
