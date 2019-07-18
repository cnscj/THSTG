using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace THGame
{
    public class EntityEmitter : MonoBehaviour
    {
        public enum EDiffusionType
        {
            Line,           //直线
            Surround,       //包围
            Random,         //随机
        }
        public enum ECreateOrderType
        {
            Orderly,        //有序
            Random,         //随机
            Fixed,          //固定
        }
        public GameObject[] launchEntities;
        public ECreateOrderType launchOrderType;
        public float launchFreq = 1;
        public int launchTimes = -1;
        public int launchNum = 1;
        public float launchSpeed = 1;
        public bool launchAutoDestroy = true;
        public bool launchForceLaunch = false;
        public EDiffusionType launchType = EDiffusionType.Surround;
        public Func<GameObject, GameObject> createFunc;               //实例化函数(有的可能是从对象池取得)

        //排序值
        public int launchOrderFixedIndex = 0;

        //发射类型:直线
        public int launchLineAngle;           //发射角度
        public float launchLineDuration;      //持续时间

        //发射类型:周围
        public int launchSurroundMinRadius;   //最小半径
        public int launchSurroundMaxRadius;   //最大半径

        //发射类型:离散
        public int launchRandomMinRadius;     //最小半径
        public int launchRandomMaxRadius;     //最大半径

        //
        private int m_curTimes;
        private float m_nextTime;
        private bool m_isStop;

        void Awake()
        {
            m_isStop = false;
            m_curTimes = launchTimes;
            m_nextTime = Time.time;
            if (createFunc == null)
            {
                createFunc = (prefab) =>
                {
                    return Instantiate(prefab);
                };
            }
        }

        void Start()
        {
            
        }
        void Update()
        {
            if (launchForceLaunch)
            {
                Launch();
                launchForceLaunch = false;

                m_curTimes--;
                m_nextTime = Time.time + launchFreq;
            }

            if (m_curTimes == 0)
            {
                m_isStop = true;
                if(launchAutoDestroy)
                {
                    Destroy(this);
                    return;
                }
            }

            if (m_isStop)
                return ;

            if (Time.time >= m_nextTime)
            {
                launchForceLaunch = true;
            }
        }
        void Launch()
        {
            switch(launchType)
            {
                case EDiffusionType.Line:
                    LaunchLine();
                    break;
                case EDiffusionType.Surround:
                    LaunchLine();
                    break;
                case EDiffusionType.Random:
                    LaunchLine();
                    break;
            }
        }

        void LaunchLine()
        {

        }

        void LaunchSurround()
        {

        }

        void LaunchRandom()
        {

        }

        GameObject[] CreateGameObjects(int num = 1)
        {
            if (launchEntities.Length > 0)
            {
                //随机函数,从中取一个
                List<GameObject> gameObjects = new List<GameObject>(num);
                if (launchOrderType == ECreateOrderType.Orderly)
                {
                    System.Random rd = new System.Random();
                    for(int i = 0; i < num; i++)
                    {
                        int index = rd.Next(launchEntities.Length) - 1;
                        var go = createFunc(launchEntities[index]);
                        gameObjects.Add(go);
                    }
                    
                }
                else if (launchOrderType == ECreateOrderType.Random)
                {
                    for (int i = 0; i < num; i++)
                    {
                        int index = i % num;
                        var go = createFunc(launchEntities[index]);
                        gameObjects.Add(go);
                    }
                }
                else if (launchOrderType == ECreateOrderType.Fixed)
                {
                    for (int i = 0; i < num; i++)
                    {
                        int index = launchOrderFixedIndex;
                        var go = createFunc(launchEntities[index]);
                        gameObjects.Add(go);
                    }
                }

                return gameObjects.ToArray();
            }
            return null;
        }
    }

}
