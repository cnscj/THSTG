
using System.IO;
using UnityEngine;
using XLibrary.Package;

namespace STGU3D
{
    //场景管理器
    public class SceneManager : MonoSingleton<SceneManager>
    {
        private string m_curLevel = "";
        public void AddScene(string name)
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name);
           
        }

        public void RemoveScene(string name)
        {

        }

        public void LoadLevelScene(string uid)
        {
           
        }
    }
}
