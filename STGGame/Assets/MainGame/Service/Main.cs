using System.Collections;
using System.Collections.Generic;
using STGU3D;
using UnityEngine;

namespace STGService
{
    public static class Main 
    {
        //所有业务逻辑初始化
        public static void InitAwake()
        {
            AssetSystem.InitAwake();
            ModuleSystem.InitAwake();
            UISystem.InitAwake();
            EntitySystem.InitAwake();
        }

        //所有业务逻辑的入口
        public static void InitStart()
        {
            UISystem.InitStart();
            EntitySystem.InitStart();

            EntityManager.GetInstance().CreateHero();
            //SceneManager.GetInstance().LoadLevelScene("200001");

        }

        public static void Update()
        {

        }
    }

}
