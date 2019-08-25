
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
            var bundle = AssetManager.GetInstance().LoadLevel(uid);
            if(bundle.isStreamedSceneAssetBundle)
            {
                var scenePaths = bundle.GetAllScenePaths();
                string sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
            }
        }
    }
}
