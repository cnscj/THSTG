
using System.Collections.Generic;
using UnityEngine;
using System;
using XLibrary.Package;
using THGame;
using Entitas;

namespace STGU3D
{
    public class EntityManager : MonoSingleton<EntityManager>
    {
        public GameObject stageRoot;
        public GameObject mapRoot;
        public GameObject heroRoot;
        public GameObject bossRoot;
        public GameObject mobRoot;

        private Systems __systems;

        private void Awake()
        {
            stageRoot = new GameObject("StageRoot");
            stageRoot.transform.SetParent(gameObject.transform, true);
            {

                mapRoot = new GameObject("MapRoot");
                mapRoot.transform.SetParent(stageRoot.transform, true);

                heroRoot = new GameObject("HeroRoot");
                heroRoot.layer = 10;
                heroRoot.transform.SetParent(stageRoot.transform, true);

                bossRoot = new GameObject("BossRoot");
                bossRoot.transform.SetParent(stageRoot.transform, true);

                mobRoot = new GameObject("MobRoot");
                mobRoot.transform.SetParent(stageRoot.transform, true);
            }
        }

        private void Start()
        {
            // 获取Entitas的上下文对象，类似一个单例管理器
            var contexts = Contexts.sharedInstance;

            // 获取所需的System组
            __systems = new Feature("Systems")
            .Add(new GameFeature(contexts))
            .Add(new InputFeature(contexts));

            // 初始化System
            __systems.Initialize();

        }

        private void Update()
        {
            __systems.Execute();
        }

    }
}
