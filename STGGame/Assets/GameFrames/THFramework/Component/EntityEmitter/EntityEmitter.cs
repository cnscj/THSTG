using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace THGame
{
    public class EntityEmitter : MonoBehaviour
    {
        public enum LaunchType
        {
            Line,           //直线
            Surround,       //包围
            Random,         //随机
            FixedPoint,     //定点
            Custom,         //自定义
        }
        public enum CreateOrderType
        {
            Orderly,        //有序
            Random,         //随机
            Fixed,          //固定
        }

        public class CreateParams
        {
            public int index;
            public GameObject prefab;
            public GameObject parent;
        }

        public class CreateResult
        {
            public GameObject entity;
        }

        public class CalculateParams
        {
            public int index;
            public EntityEmitter emitter;
        }

        public class CalculateResult
        {
            public Vector3 startPosition = Vector3.zero;
            public Vector3 startEulerAngles = Vector3.zero;
            public Vector3 startSpeed = Vector3.zero;
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
            }else
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
                    double lAngle = args.emitter.launchLineAngle * Math.PI / 180;
                    result.startSpeed.x = args.emitter.launchSpeed * (float)Math.Cos(lAngle);
                    result.startSpeed.y = args.emitter.launchSpeed * (float)Math.Sin(lAngle);
                    args.emitter.launchLineAngle += args.emitter.launchLineRPT;
                    break;
                case LaunchType.Surround:
                    //以相对点为中心,360
                    int sr = args.emitter.launchSurroundRadius;
                    double srAngle = 2 * Math.PI / args.emitter.launchNum * args.index;
                    int srX = (int)(sr * Math.Cos(srAngle));
                    int srY = (int)(sr * Math.Sin(srAngle));
                    result.startPosition.x = srX + args.emitter.launchRelative.transform.localPosition.x;
                    result.startPosition.y = srY + args.emitter.launchRelative.transform.localPosition.y;

                    result.startSpeed.x = args.emitter.launchSpeed * (float)Math.Cos(srAngle);
                    result.startSpeed.y = args.emitter.launchSpeed * (float)Math.Sin(srAngle);
                    break;
                case LaunchType.Random:
                    //随机位置
                    int minR = args.emitter.launchRandomMinRadius;
                    int maxR = args.emitter.launchRandomMaxRadius;
                    System.Random rd = new System.Random();
                    int r = rd.Next(minR, maxR+1);
                    double angle = rd.Next(0, 361) * Math.PI / 180;
                    int rX = (int)(r * Math.Cos(angle));
                    int rY = (int)(r * Math.Sin(angle));
                    result.startPosition.x = rX + args.emitter.launchRelative.transform.localPosition.x;
                    result.startPosition.y = rY + args.emitter.launchRelative.transform.localPosition.y;

                    result.startSpeed.x = args.emitter.launchSpeed * (float)Math.Cos(angle);
                    result.startSpeed.y = args.emitter.launchSpeed * (float)Math.Sin(angle);
                    break;
                case LaunchType.FixedPoint:
                    //固定位置
                    if (args.emitter.launchFixedPointPoints.Length > 0)
                    {
                        var ponit = args.emitter.launchFixedPointPoints[args.index % args.emitter.launchFixedPointPoints.Length];
                        result.startPosition.x = ponit.x;
                        result.startPosition.y = ponit.y;
                    }
                    else
                    {
                        result.startPosition = args.emitter.launchRelative.transform.localPosition;
                    }
                    break;
            }

            return result;
        };
        //发射函数
        public static readonly Action<LaunchParams> defaultOnLaunch = (args) =>
        {
            Transform trans = args.createResult.entity.GetComponent<Transform>();
            EntityController ctrl = args.createResult.entity.GetComponent<EntityController>();
            if (ctrl == null) ctrl = args.createResult.entity.AddComponent<EntityController>();

            trans.localPosition = args.calculateResult.startPosition;
            trans.localEulerAngles = args.calculateResult.startEulerAngles;

            ctrl.speed = args.calculateResult.startSpeed;
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
        public float launchSpeed = 1;
        public bool launchAutoDestroy = false;
        public bool launchForceLaunch = false;
        public LaunchType launchType = LaunchType.Surround;

        //排序值
        public bool launchOrderOrderlyByTimes = false;
        public int launchOrderFixedIndex = 0;

        //发射类型:直线
        public float launchLineAngle;         //发射角度(水平
        public float launchLineRPT;           //每发射一次转动角度

        //发射类型:周围
        public int launchSurroundRadius;      //轨道半径

        //发射类型:离散
        public int launchRandomMinRadius;     //最小半径
        public int launchRandomMaxRadius = 10;     //最大半径

        //发射类型:固定点
        public Vector2[] launchFixedPointPoints;     //固定点位置 FIXME:没法K帧

        //发射类型:自定义
        public int launchCustomType;         //自定义类型
        public object launchCustomData0;       //自定义数据
        public float launchCustomData1;      //自定义数据1
        public float launchCustomData2;      //自定义数据2
        public int launchCustomData3;       //自定义数据3
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
                    createParams.index = index;
                    createParams.parent = launchParent;
                    createParams.prefab = launchEntities[index];

                    calculateParams.emitter = this;
                    calculateParams.index = i;

                    launchParams.createResult = s_createFunc(createParams);
                    launchParams.calculateResult = s_calculateFunc(calculateParams);
                    s_launchFunc(launchParams);

                }
            }
        }
       
    }

}
