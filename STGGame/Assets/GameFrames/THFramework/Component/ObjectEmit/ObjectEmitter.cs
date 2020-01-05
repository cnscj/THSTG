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
        public float launchLineDistance = 0f;               //发射起始距离
        public float launchLineAngle;                       //发射角度(水平
        public float launchLineRPT;                         //每发射一次转动角度

        //扇形
        public float launchSectorRadius;                    //轨道半径
        public float launchSectorStartAngle;                //起始角
        public float launchSectorSpreadAngle = 360f;        //张角

        //发射类型:离散
        public float launchRandomMinRadius;                 //最小半径
        public float launchRandomMaxRadius = 10f;           //最大半径

        //发射类型:固定点
        public Vector3[] launchFixedPointPoints;            //固定点位置


        //发射类型:自定义
        public ObjectEmitCustom launchCustomCallback;

        //
        private float m_nextTime;
        private System.Random m_rd = new System.Random();
        private ObjectEmitCreateParams m_objectEmitCreateParams = new ObjectEmitCreateParams();
        private ObjectEmitCalculateParams m_objectEmitCalculateParams = new ObjectEmitCalculateParams();
        private ObjectEmitLaunchParams m_objectEmitLaunchParams = new ObjectEmitLaunchParams();
        private ObjectEmitCalculateResult m_objectEmitCalculateResult = new ObjectEmitCalculateResult();

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
            //m_nextTime = Time.time + launchFreq;
            
        }

        void OnEnable()
        {
            if (enabled)
            {
                Launch();
                m_nextTime = Time.time + launchFreq;
            }
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

                    m_nextTime = Time.time + launchFreq;
                }
            }
        }
        void Launch()
        {
            if (launchEntities.Length > 0)
            {
                int index = 0;
                bool isNeedIndex = true;
                for (int i = 0; i < launchNum; i++)
                {
                    switch (launchOrderType)
                    {
                        case EObjectEmitCreateOrderType.Orderly:
                            {
                                index = launchOrderOrderlyByTimes ? Math.Abs(launchTimes % launchEntities.Length) : (i % launchEntities.Length);
                            }
                            break;
                        case EObjectEmitCreateOrderType.Random:
                            {
                                if (!isNeedIndex) break;
                                if (launchOrderOrderlyByTimes) isNeedIndex = false;
                                index = m_rd.Next(0, launchEntities.Length);//随机函数,从中取一个
                            }
                            break;
                        case EObjectEmitCreateOrderType.Fixed:
                            {
                                index = launchOrderFixedIndex;
                            }
                            break;
                    }

                    ///
                    m_objectEmitCreateParams.parent = launchParent;
                    m_objectEmitCreateParams.prefab = launchEntities[index];

                    m_objectEmitCalculateParams.emitter = this;
                    m_objectEmitCalculateParams.index = i;
                    m_objectEmitCalculateParams.times = Mathf.Abs(launchTimes);

                    m_objectEmitCalculateResult.startPosition = Vector3.zero;
                    m_objectEmitCalculateResult.startEulerAngles = Vector3.zero;
                    m_objectEmitCalculateResult.startAngleSpeed = Vector3.zero;
                    m_objectEmitCalculateResult.startMoveSpeed = Vector3.zero;

                    ///
                    m_objectEmitLaunchParams.createResult = launchListener.OnCreate(m_objectEmitCreateParams);
                    m_objectEmitLaunchParams.calculateResult = OnCalculate(m_objectEmitCalculateParams, ref m_objectEmitCalculateResult);

                    //发射前的校验
                    if (launchRelative != null)
                    {
                        m_objectEmitLaunchParams.calculateResult.startPosition += launchRelative.transform.position;

                    }

                    if (launchFixAngle)
                    {
                        m_objectEmitLaunchParams.calculateResult.startEulerAngles.z = 0;
                        m_objectEmitLaunchParams.calculateResult.startAngleSpeed.z = 0;
                    }

                    launchListener.OnLaunch(m_objectEmitLaunchParams);
                }
            }
            launchTimes--;
        }

        private ObjectEmitCalculateResult OnCalculate(ObjectEmitCalculateParams args, ref ObjectEmitCalculateResult result)
        {
            switch (args.emitter.launchType)
            {
                //存在一次发射不同方向的可能
                case EObjectEmitLaunchType.Line:
                    {
                        float lAngle = args.emitter.launchLineAngle * Mathf.Deg2Rad;
                        result.startMoveSpeed.x = args.emitter.launchMoveSpeed * Mathf.Cos(lAngle);
                        result.startMoveSpeed.y = args.emitter.launchMoveSpeed * Mathf.Sin(lAngle);
                        args.emitter.launchLineAngle += args.emitter.launchLineRPT;

                        result.startPosition.x = args.emitter.launchLineDistance * Mathf.Cos(lAngle);
                        result.startPosition.y = args.emitter.launchLineDistance * Mathf.Sin(lAngle);

                        result.startEulerAngles.z = (lAngle * Mathf.Rad2Deg) - 90;
                        result.startAngleSpeed.z = args.emitter.launchAngleSpeed;
                    }
                    break;
                case EObjectEmitLaunchType.Sector:
                    {
                        float sectorAnglePerTimes = args.emitter.launchSectorSpreadAngle / args.emitter.launchNum;
                        float sectorAngle = (sectorAnglePerTimes * args.index + args.emitter.launchSectorStartAngle) * Mathf.Deg2Rad;
             
                        result.startPosition.x = args.emitter.launchSectorRadius * Mathf.Cos(sectorAngle);
                        result.startPosition.y = args.emitter.launchSectorRadius * Mathf.Sin(sectorAngle);

                        result.startMoveSpeed.x = args.emitter.launchMoveSpeed * Mathf.Cos(sectorAngle);
                        result.startMoveSpeed.y = args.emitter.launchMoveSpeed * Mathf.Sin(sectorAngle);

                        result.startAngleSpeed.z = args.emitter.launchAngleSpeed;
                        result.startEulerAngles.z = (sectorAngle * Mathf.Rad2Deg) - 90;
                    }
                    break;
                case EObjectEmitLaunchType.Random:
                    {
                        //随机位置
                        float minR = args.emitter.launchRandomMinRadius;
                        float maxR = args.emitter.launchRandomMaxRadius;
                        float r = m_rd.Next((int)(minR * 100), (int)((maxR * 100) + 1)) / 100f;
                        float angle = m_rd.Next(0, 361) * Mathf.Deg2Rad;
                        float rX = r * Mathf.Cos(angle);
                        float rY = r * Mathf.Sin(angle);
                        result.startPosition.x = rX;
                        result.startPosition.y = rY;

                        result.startMoveSpeed.x = args.emitter.launchMoveSpeed * (float)Mathf.Cos(angle);
                        result.startMoveSpeed.y = args.emitter.launchMoveSpeed * (float)Mathf.Sin(angle);

                        result.startAngleSpeed.z = args.emitter.launchAngleSpeed;
                        result.startEulerAngles.z = (angle * Mathf.Rad2Deg) - 90;
                    }
                    break;
                case EObjectEmitLaunchType.FixedPoint:
                    {
                        //固定位置
                        if (args.emitter.launchFixedPointPoints.Length > 0)
                        {
                            var ponit = args.emitter.launchFixedPointPoints[(args.index + args.times) % args.emitter.launchFixedPointPoints.Length];
                            result.startPosition.x = ponit.x;
                            result.startPosition.y = ponit.y;
                            result.startPosition.z = ponit.z;
                        }
                    }
                    break;
                case EObjectEmitLaunchType.Custom:
                    {
                        if (args.emitter.launchCustomCallback != null)
                        {
                            result = args.emitter.launchCustomCallback.Calculate(args);
                        }
                    }
                    break;
            }

            return result;
        }

        private void OnDrawGizmos()
        {
            if (!enabled) return;

            switch (launchType)
            {
                case EObjectEmitLaunchType.Line:
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(transform.position, new Vector3(launchLineDistance * Mathf.Cos(launchLineAngle * Mathf.Deg2Rad) + transform.position.x, launchLineDistance * Mathf.Sin(launchLineAngle * Mathf.Deg2Rad) + transform.position.y));
                    }
                    break;
                case EObjectEmitLaunchType.Sector:
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireSphere(transform.position, launchSectorRadius);
                        var sectorRadius = Mathf.Max(0.1f, launchSectorRadius);
                        Gizmos.DrawLine(transform.position, new Vector3(sectorRadius * Mathf.Cos(launchSectorStartAngle * Mathf.Deg2Rad) + transform.position.x, sectorRadius * Mathf.Sin(launchSectorStartAngle * Mathf.Deg2Rad) + transform.position.y));
                        Gizmos.DrawLine(transform.position, new Vector3(sectorRadius * Mathf.Cos((launchSectorStartAngle + launchSectorSpreadAngle) * Mathf.Deg2Rad) + transform.position.x, sectorRadius * Mathf.Sin((launchSectorStartAngle + launchSectorSpreadAngle) * Mathf.Deg2Rad) + transform.position.y));
                    }
                    break;
                case EObjectEmitLaunchType.Random:
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireSphere(transform.position, launchRandomMinRadius);
                        Gizmos.DrawWireSphere(transform.position, launchRandomMaxRadius);
                    }
                    break;
                case EObjectEmitLaunchType.FixedPoint:
                    {
                        if (launchFixedPointPoints != null && launchFixedPointPoints.Length > 0)
                        {
                            Gizmos.color = Color.blue;
                            foreach (var position in launchFixedPointPoints)
                            {
                                Gizmos.DrawWireSphere(position, .20f);
                            }
                        }
                    }
                    break;
                case EObjectEmitLaunchType.Custom:
                    {
                        if (launchCustomCallback != null)
                        {
                            launchCustomCallback.DrawGizmos(this);
                        }
                    }
                    break;
            }
        }

    }

}
