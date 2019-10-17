
using UnityEngine;
using Unity.Entities;
using XLibrary.Package;

namespace STGGame
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        private void Start()
        {
            EntityManager.GetInstance().CreatePlayer();
            //SceneManager.GetInstance().LoadLevelScene("200001");
        }
        private GameManager()
        {

        }
    }
}