using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace THGame
{
    public class ObjectEmitter : MonoBehaviour
    {
        private static readonly ObjectEmitListener defaultLaunchListener = new ObjectEmitListener();

        private static ObjectEmitListener launchListener = defaultLaunchListener;
        public GameObject[] launchEntities;
        public GameObject launchParent;
        public GameObject launchRelative;
        public EObjectEmitCreateOrderType launchOrderType;
        public float launchFreq = 0.5f;
        public int launchTimes = -1;
        public int launchNum = 1;
        public float launchMoveSpeed = 1;
        public int launchAngleSpeed = 0;
        public bool launchFixAngle = false;
        
        public EObjectEmitLaunchType launchType = EObjectEmitLaunchType.Line;

        //排序值
        public bool launchOrderOrderlyByTimes = false;
        public int launchOrderFixedIndex = 0;

        //发射类型:直线
        public float launchLineAngle;               //发射角度(水平
        public float launchLineRPT;                 //每发射一次转动角度

        //扇形
        public float launchSectorRadius;                    //轨道半径
        public float launchSectorStartAngle;                //起始角
        public float launchSectorSpreadAngle = 360;         //张角

        //发射类型:离散
        public float launchRandomMinRadius;          //最小半径
        public float launchRandomMaxRadius = 10;     //最大半径

        //发射类型:固定点
        public Vector3[] launchFixedPointPoints;     //固定点位置


        //发射类型:自定义
        public ObjectEmitCustom launchCustomCallback;

        //
        private float m_nextTime;

        public static void ReplaceListener(ObjectEmitListener listener)
        {
            launchListener = (listener == null ? defaultLaunchListener : listener);
        }

        void Awake()
        {
            launchRelative = launchRelative != null ? launchRelative : gameObject;
        }

        void Start()
        {
            m_nextTime = Time.time + launchFreq;
            
        }

        void OnEnable()
        {
            m_nextTime = Time.time + launchFreq;
        }

        void Update()
        {
            if (launchTimes == 0)    //负数表示无穷
            {
                return;
            }
            else
            {
                if (Time.time >= m_nextTime)
                {
                    Launch();

                    launchTimes--;
                    m_nextTime = Time.time + launchFreq;
                }
            }
        }
        void Launch()
        {
            if (launchEntities.Length > 0)
            {
                ObjectEmitCreateParams objectEmitCreateParams = new ObjectEmitCreateParams();
                ObjectEmitCalculateParams objectEmitCalculateParams = new ObjectEmitCalculateParams();
                ObjectEmitLaunchParams objectEmitLaunchParams = new ObjectEmitLaunchParams();

                int index = 0;
                bool isNeedIndex = true;
                for (int i = 0; i < launchNum; i++)
                {
                    switch (launchOrderType)
                    {
                        case EObjectEmitCreateOrderType.Orderly:
                            index = launchOrderOrderlyByTimes ? Math.Abs(launchTimes % launchEntities.Length) : (i % launchEntities.Length);
                            break;
                        case EObjectEmitCreateOrderType.Random:
                            if (!isNeedIndex) break;
                            if (launchOrderOrderlyByTimes) isNeedIndex = false;
                            System.Random rd = new System.Random();
                            index = rd.Next(0, launchEntities.Length);//随机函数,从中取一个
                            break;
                        case EObjectEmitCreateOrderType.Fixed:
                            index = launchOrderFixedIndex;
                            break;
                    }

                    objectEmitCreateParams.parent = launchParent;
                    objectEmitCreateParams.prefab = launchEntities[index];

                    objectEmitCalculateParams.emitter = this;
                    objectEmitCalculateParams.index = i;
                    objectEmitCalculateParams.times = Mathf.Abs(launchTimes);

                    objectEmitLaunchParams.createResult = launchListener.OnCreate(objectEmitCreateParams);
                    objectEmitLaunchParams.calculateResult = launchListener.OnCalculate(objectEmitCalculateParams);

                    //发射前的校验
                    if (launchRelative != null)
                    {
                        objectEmitLaunchParams.calculateResult.startPosition += launchRelative.transform.position;

                    }

                    if (launchFixAngle)
                    {
                        objectEmitLaunchParams.calculateResult.startEulerAngles.z = 0;
                        objectEmitLaunchParams.calculateResult.startAngleSpeed.z = 0;
                    }

                    launchListener.OnLaunch(objectEmitLaunchParams);

                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!enabled) return;

            switch (launchType)
            {
                case EObjectEmitLaunchType.Line:
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(transform.localPosition, new Vector3(Mathf.Cos(launchLineAngle * Mathf.Deg2Rad) + transform.localPosition.x, Mathf.Sin(launchLineAngle * Mathf.Deg2Rad) + transform.localPosition.y));
                    break;
                case EObjectEmitLaunchType.Sector:
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(transform.localPosition, launchSectorRadius);
                    var sectorRadius = Mathf.Max(0.1f, launchSectorRadius);
                    Gizmos.DrawLine(transform.localPosition, new Vector3(sectorRadius * Mathf.Cos(launchSectorStartAngle * Mathf.Deg2Rad) + transform.localPosition.x, sectorRadius * Mathf.Sin(launchSectorStartAngle * Mathf.Deg2Rad) + transform.localPosition.y));
                    Gizmos.DrawLine(transform.localPosition, new Vector3(sectorRadius * Mathf.Cos((launchSectorStartAngle + launchSectorSpreadAngle) * Mathf.Deg2Rad) + transform.localPosition.x, sectorRadius * Mathf.Sin((launchSectorStartAngle + launchSectorSpreadAngle) * Mathf.Deg2Rad) + transform.localPosition.y));
                    break;
                case EObjectEmitLaunchType.Random:
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(transform.localPosition, launchRandomMinRadius);
                    Gizmos.DrawWireSphere(transform.localPosition, launchRandomMaxRadius);
                    break;
                case EObjectEmitLaunchType.FixedPoint:
                    if (launchFixedPointPoints != null && launchFixedPointPoints.Length > 0)
                    {
                        Gizmos.color = Color.blue;
                        foreach (var position in launchFixedPointPoints)
                        {
                            Gizmos.DrawWireSphere(position, .20f);
                        }
                    }
                    break;
                case EObjectEmitLaunchType.Custom:
                    if (launchCustomCallback != null)
                    {
                        launchCustomCallback.DrawGizmos(this);
                    }
                    break;
            }
        }

    }

}
