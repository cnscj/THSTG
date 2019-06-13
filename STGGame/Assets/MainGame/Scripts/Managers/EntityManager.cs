
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using THGame;
namespace STGGame
{
    public class EntityManager : SingletonBehaviour<StageEntityManager>
    {
        public GameObject PlayerTmpl;
        public GameObject MobTmpl;

        [SerializeField] public List<GameObject> players = new List<GameObject>();
        [SerializeField] public List<GameObject> mobs = new List<GameObject>();

        private EntityManager()
        {

        }
    }
}