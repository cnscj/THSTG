using STGU3D;
using THGame;
using Unity.Entities;
using UnityEngine;
using XLibrary;

namespace STGService
{
    public static class EntitySystem
    {
        public static string heroName = "Player";


        public static void InitAwake()
        {
            //实体发射器扩展
            EntityEmitter.OnCreate((args) =>
            {
                //TODO:对象池以及根据实体类型分配父类
                return EntityEmitter.defaultOnCreate(args);
            });

            EntityEmitter.OnCalculate((args) =>
            {
                if (args.emitter.launchType == EntityEmitter.LaunchType.Custom)
                {
                    EntityEmitter.CalculateResult result = new EntityEmitter.CalculateResult();



                    return result;
                }
                else
                {
                    return EntityEmitter.defaultOnCalculate(args);
                }

            });

            EntityEmitter.OnLaunch((args) =>
            {
                var entity = args.createResult.entity;
                var transformCom = entity.GetComponent<Transform>();
                var moveCom = entity.GetComponent<MoveComponent>();

                if (transformCom != null)
                {
                    transformCom.localPosition = args.calculateResult.startPosition;
                    transformCom.localEulerAngles = args.calculateResult.startEulerAngles;
                }

                if (moveCom != null)
                {
                    moveCom.speed = args.calculateResult.startSpeed;
                }
            });
        }

        public static void InitStart()
        {
            var name = CSVUtil.SafeGetValue("", EntityConfiger.GetRoleInfo("10100001"), "name");
            Debug.Log(name);
        }

        private static STGU3D.EntityManager GetManager()
        {
            return STGU3D.EntityManager.GetInstance();
        }

        public static GameObject AddRole(ERoleType roleType)
        {
            GameObject entity = CreateBaseRole();
            //初始化各个组件
            var renderer = entity.GetComponent<RendererComponent>();



            return entity;
        }
        ////
        public static GameObject AddEntity(string entityCode)
        {
            //是否为实体code
            //取得实体类型


            return null;
        }
        ////
        public static GameObject CreateBaseRole()
        {
            GameObject role = new GameObject();
            AddCommonComponents(role);

            return role;
        }

        public static GameObject CreateBaseMob()
        {
            GameObject mob = new GameObject();
            AddCommonComponents(mob);

            return mob;
        }

        public static GameObject CreateBaseBoss()
        {
            GameObject boss = new GameObject();
            AddCommonComponents(boss);

            return boss;
        }

        public static GameObject CreateBaseBullet()
        {
            GameObject bullet = new GameObject();
            AddCommonComponents(bullet);

            return bullet;
        }

        //实体共有组件
        private static void AddCommonComponents(GameObject entity)
        {
            if (entity)
            {
                entity.AddComponent<GameObjectEntity>();    //必要组件
                entity.AddComponent<MoveComponent>();       //移动组件
                entity.AddComponent<RendererComponent>();   //渲染组件

            }
        }
    }

}
