
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
            for (int i = 0;i < 5; i++)
            {
                var entity = EntityManager.GetInstance().CreateMob();
                Transform trsns = entity.GetComponent<Transform>();
                trsns.position = new Vector3(10 * Mathf.Cos(i), 0, 10 * Mathf.Sin(i));

            }
            SceneManager.GetInstance().LoadLevelScene("200001");
        }
        private GameManager()
        {

        }
    }
}