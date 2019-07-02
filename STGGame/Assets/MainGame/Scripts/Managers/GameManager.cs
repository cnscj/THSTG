
using UnityEngine;
using Unity.Entities;
using THGame;
namespace STGGame
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        private void Start()
        {
            EntityManager.GetInstance().CreatePlayer();
            for (int i = 0;i < 1000; i++)
            {
                var entity = EntityManager.GetInstance().CreateMob();
                Transform trsns = entity.GetComponent<Transform>();
                trsns.position = new Vector3(10 * Mathf.Cos(i), 0, 10 * Mathf.Sin(i));

            }
        }
        private GameManager()
        {

        }
    }
}