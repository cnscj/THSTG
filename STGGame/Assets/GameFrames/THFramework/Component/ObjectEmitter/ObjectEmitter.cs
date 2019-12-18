using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace THGame
{
    public class ObjectEmitter : MonoBehaviour
    {
        public enum LaunchType
        {
            Line,           //直线
            Surround,       //包围
            Sector,         //扇形    //TODO:
            Random,         //随机
            FixedPoint,     //定点
            Custom,         //自定义
        }
        public enum CreateOrderType
        {
            Orderly,        //顺序
            Random,         //随机
            Fixed,          //固定
        }

        public class CreateParams
        {
            public GameObject prefab;
            public GameObject parent;
        }

        public class CreateResult
        {
            public GameObject entity;
        }

        public class CalculateParams
        {
            public int times;
            public int index;
            public ObjectEmitter emitter;
        }

        public class CalculateResult
        {
            public Vector3 startPosition = Vector3.zero;
            public Vector3 startEulerAngles = Vector3.zero;

            public Vector3 startMoveSpeed = Vector3.zero;
            public Vector3 startAngleSpeed = Vector3.zero;
        }

        public class LaunchParams
        {
            public CreateResult createResult;
            public CalculateResult calculateResult;
        }

        //各种回调
        //默认实例化函数
        public static readonly Func<CreateParams, CreateResult> defaultOnCreate = (args) =>
        {
            CreateResult result = new CreateResult();
            if (args.parent != null)
            {
                result.entity = Instantiate(args.prefab, args.parent.transform, false);
            }
            else
            {
                result.entity = Instantiate(args.prefab);
            }
            return result;

        };
        //计算函数
        public static readonly Func<CalculateParams, CalculateResult> defaultOnCalculate = (args) =>
        {
            CalculateResult result = new CalculateResult();

            switch (args.emitter.launchType)
            {
                case LaunchType.Line:
                    result.startPosition = args.emitter.launchRelative.transform.localPosition;
                    

                    float lAngle = args.emitter.launchLineAngle * Mathf.Deg2Rad;
                    result.startMoveSpeed.x = args.emitter.launchMoveSpeed * (float)Mathf.Cos(lAngle);
                    result.startMoveSpeed.y = args.emitter.launchMoveSpeed * (float)Mathf.Sin(lAngle);
                    args.emitter.launchLineAngle += args.emitter.launchLineRPT;
                    if (!args.emitter.launchFixAngle)
                    {
                        result.startEulerAngles.z = (lAngle * Mathf.Rad2Deg) - 90;
                        result.startAngleSpeed.z = args.emitter.launchAngleSpeed;
                    }
                    break;
                case LaunchType.Surround:
                    //以相对点为中心,360
                    float sr = args.emitter.launchSurroundRadius;
                    float srAngle = 2 * Mathf.PI / args.emitter.launchNum * args.index;
                    float srX = (sr * Mathf.Cos(srAngle));
                    float srY = (sr * Mathf.Sin(srAngle));
                    result.startPosition.x = srX + args.emitter.launchRelative.transform.localPosition.x;
                    result.startPosition.y = srY + args.emitter.launchRelative.transform.localPosition.y;
                    
                    result.startMoveSpeed.x = args.emitter.launchMoveSpeed * (float)Mathf.Cos(srAngle);
                    result.startMoveSpeed.y = args.emitter.launchMoveSpeed * (float)Mathf.Sin(srAngle);
                    if (!args.emitter.launchFixAngle)
                    {
                        result.startAngleSpeed.z = args.emitter.launchAngleSpeed;
                        result.startEulerAngles.z = (srAngle * Mathf.Rad2Deg) - 90;
                    }
                    break;
                    //TODO:
                case LaunchType.Sector:
                    break;
                case LaunchType.Random:
                    //随机位置
                    float minR = args.emitter.launchRandomMinRadius;
                    float maxR = args.emitter.launchRandomMaxRadius;
                    System.Random rd = new System.Random();
                    float r = rd.Next((int)(minR*100), (int)((maxR * 100) + 1)) / 100f;
                    float angle = rd.Next(0, 361) * Mathf.Deg2Rad;
                    float rX = r * Mathf.Cos(angle);
                    float rY = r * Mathf.Sin(angle);
                    result.startPosition.x = rX + args.emitter.launchRelative.transform.localPosition.x;
                    result.startPosition.y = rY + args.emitter.launchRelative.transform.localPosition.y;
                    
                    result.startMoveSpeed.x = args.emitter.launchMoveSpeed * (float)Mathf.Cos(angle);
                    result.startMoveSpeed.y = args.emitter.launchMoveSpeed * (float)Mathf.Sin(angle);
                    if (!args.emitter.launchFixAngle)
                    {
                        result.startAngleSpeed.z = args.emitter.launchAngleSpeed;
                        result.startEulerAngles.z = (angle * Mathf.Rad2Deg) - 90;
                    }
                    break;
                case LaunchType.FixedPoint:
                    //固定位置
                    if (args.emitter.launchFixedPointPoints.Length > 0)
                    {
                        var ponit = args.emitter.launchFixedPointPoints[(args.index + args.times) % args.emitter.launchFixedPointPoints.Length];
                        result.startPosition.x = ponit.x;
                        result.startPosition.y = ponit.y;
                        result.startPosition.z = ponit.z;

                    }
                    
                    break;
            }

            return result;
        };
        //发射函数
        public static readonly Action<LaunchParams> defaultOnLaunch = (args) =>
        {
            Transform trans = args.createResult.entity.GetComponent<Transform>();
            ObjectController ctrl = args.createResult.entity.GetComponent<ObjectController>();
            if (ctrl == null) ctrl = args.createResult.entity.AddComponent<ObjectController>();

            trans.localPosition = args.calculateResult.startPosition;
            trans.localEulerAngles = args.calculateResult.startEulerAngles;

            ctrl.moveSpeed = args.calculateResult.startMoveSpeed;
            ctrl.rotationSpeed = args.calculateResult.startAngleSpeed;
        };

        private static Func<CreateParams, CreateResult> s_createFunc = defaultOnCreate;
        private static Func<CalculateParams, CalculateResult> s_calculateFunc = defaultOnCalculate;
        private static Action<LaunchParams> s_launchFunc = defaultOnLaunch;
        
        public GameObject[] launchEntities;
        public GameObject launchParent;
        public GameObject launchRelative;
        public CreateOrderType launchOrderType;
        public float launchFreq = 0.5f;
        public int launchTimes = -1;
        public int launchNum = 1;
        public float launchMoveSpeed = 1;
        public int launchAngleSpeed = 0;
        public bool launchFixAngle = false;
        public bool launchAutoDestroy = false;
        public bool launchForceLaunch = false;
        
        public LaunchType launchType = LaunchType.Surround;

        //排序值
        public bool launchOrderOrderlyByTimes = false;
        public int launchOrderFixedIndex = 0;

        //发射类型:直线
        public float launchLineAngle;               //发射角度(水平
        public float launchLineRPT;                 //每发射一次转动角度

        //发射类型:周围
        public float launchSurroundRadius;           //轨道半径

        //扇形
        public float launchSectorRadius;            //轨道半径
        public float launchSectorStartAngle;        //起始角
        public float launchSectorSpreadAngle;       //张角

        //发射类型:离散
        public float launchRandomMinRadius;          //最小半径
        public float launchRandomMaxRadius = 10;     //最大半径

        //发射类型:固定点
        public Vector3[] launchFixedPointPoints;     //固定点位置


        //发射类型:自定义
        public int launchCustomType;            //自定义类型
        public int launchCustomData1;           //自定义数据1
        public float launchCustomData2;         //自定义数据2
        public string launchCustomData3;        //自定义数据3
        public ObjectCallback launchCustomCallback;

        //
        private float m_nextTime;

        public static void OnCreate(Func<CreateParams, CreateResult> func)
        {
            s_createFunc = func;

        }

        public static void OnCalculate(Func<CalculateParams, CalculateResult> func)
        {
            s_calculateFunc = func;

        }

        public static void OnLaunch(Action<LaunchParams> func)
        {
            s_launchFunc = func;

        }

        void Awake()
        {
            launchRelative = launchRelative != null ? launchRelative : gameObject;
        }

        void Start()
        {
            m_nextTime = Time.time + launchFreq;
            
        }
        void Update()
        {
            if (launchTimes == 0)    //负数表示无穷
            {
                if(launchAutoDestroy)
                {
                    Destroy(this);
                }
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
                List<GameObject> gameObjects = new List<GameObject>(launchNum);
                CreateParams createParams = new CreateParams();
                CalculateParams calculateParams = new CalculateParams();
                LaunchParams launchParams = new LaunchParams();

                int index = 0;
                bool isNeedIndex = true;
                for (int i = 0; i < launchNum; i++)
                {
                    switch (launchOrderType)
                    {
                        case CreateOrderType.Orderly:
                            index = launchOrderOrderlyByTimes ? Math.Abs(launchTimes % launchEntities.Length) : (i % launchEntities.Length);
                            break;
                        case CreateOrderType.Random:
                            if (!isNeedIndex) break;
                            if (launchOrderOrderlyByTimes) isNeedIndex = false;
                            System.Random rd = new System.Random();
                            index = rd.Next(0, launchEntities.Length);//随机函数,从中取一个
                            break;
                        case CreateOrderType.Fixed:
                            index = launchOrderFixedIndex;
                            break;
                    }

                    createParams.parent = launchParent;
                    createParams.prefab = launchEntities[index];

                    calculateParams.emitter = this;
                    calculateParams.index = i;
                    calculateParams.times = Mathf.Abs(launchTimes);

                    launchParams.createResult = s_createFunc(createParams);
                    launchParams.calculateResult = s_calculateFunc(calculateParams);
                    s_launchFunc(launchParams);

                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!enabled) return;

            switch (launchType)
            {
                case LaunchType.Line:
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(transform.localPosition, new Vector3(Mathf.Cos(launchLineAngle * Mathf.Deg2Rad) + transform.localPosition.x, Mathf.Sin(launchLineAngle * Mathf.Deg2Rad) + transform.localPosition.y));
                    break;
                case LaunchType.Surround:
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(transform.localPosition, launchSurroundRadius);
                    break;
                case LaunchType.Random:
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(transform.localPosition, launchRandomMinRadius);
                    Gizmos.DrawWireSphere(transform.localPosition, launchRandomMaxRadius);
                    break;
                case LaunchType.FixedPoint:
                    if (launchFixedPointPoints != null && launchFixedPointPoints.Length > 0)
                    {
                        Gizmos.color = Color.blue;
                        foreach (var position in launchFixedPointPoints)
                        {
                            Gizmos.DrawWireSphere(position, .20f);
                        }
                    }
                    break;
            }
        }

    }

}
