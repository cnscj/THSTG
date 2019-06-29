
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
        }
        private GameManager()
        {

        }
    }
}