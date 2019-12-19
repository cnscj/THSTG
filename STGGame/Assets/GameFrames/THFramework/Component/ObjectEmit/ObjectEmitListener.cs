
using UnityEngine;

namespace THGame
{
    public class ObjectEmitListener
    {
        //实例创建
        public virtual ObjectEmitCreateResult OnCreate(ObjectEmitCreateParams args)
        {
            ObjectEmitCreateResult result = new ObjectEmitCreateResult();
            if (args.parent != null)
            {
                var parent = args.parent as GameObject;
                var prefab = args.prefab as GameObject;
                result.entity = Object.Instantiate(prefab, parent.transform, false);
            }
            else
            {
                var prefab = args.prefab as GameObject;
                result.entity = Object.Instantiate(prefab);
            }
            return result;
        }

        //计算
        public virtual ObjectEmitCalculateResult OnCalculate(ObjectEmitCalculateParams args)
        {
            ObjectEmitCalculateResult result = new ObjectEmitCalculateResult();

            switch (args.emitter.launchType)
            {
                //TODO:存在一次发射不同方向的可能
                case EObjectEmitLaunchType.Line:
                    float lAngle = args.emitter.launchLineAngle * Mathf.Deg2Rad;
                    result.startMoveSpeed.x = args.emitter.launchMoveSpeed * (float)Mathf.Cos(lAngle);
                    result.startMoveSpeed.y = args.emitter.launchMoveSpeed * (float)Mathf.Sin(lAngle);
                    args.emitter.launchLineAngle += args.emitter.launchLineRPT;
 
                    result.startEulerAngles.z = (lAngle * Mathf.Rad2Deg) - 90;
                    result.startAngleSpeed.z = args.emitter.launchAngleSpeed;
                    
                    break;
                case EObjectEmitLaunchType.Sector:
                    float sectorAnglePerTimes = args.emitter.launchSectorSpreadAngle / args.emitter.launchNum;
                    float sectorAngle = (sectorAnglePerTimes * args.index + args.emitter.launchSectorStartAngle) * Mathf.Deg2Rad;
                    float sectorX = (args.emitter.launchSectorRadius * Mathf.Cos(sectorAngle));
                    float sectorY = (args.emitter.launchSectorRadius * Mathf.Sin(sectorAngle));
                    result.startPosition.x = sectorX;
                    result.startPosition.y = sectorY;

                    result.startMoveSpeed.x = args.emitter.launchMoveSpeed * (float)Mathf.Cos(sectorAngle);
                    result.startMoveSpeed.y = args.emitter.launchMoveSpeed * (float)Mathf.Sin(sectorAngle);

                    result.startAngleSpeed.z = args.emitter.launchAngleSpeed;
                    result.startEulerAngles.z = (sectorAngle * Mathf.Rad2Deg) - 90;

                    break;
                case EObjectEmitLaunchType.Random:
                    //随机位置
                    float minR = args.emitter.launchRandomMinRadius;
                    float maxR = args.emitter.launchRandomMaxRadius;
                    System.Random rd = new System.Random();
                    float r = rd.Next((int)(minR * 100), (int)((maxR * 100) + 1)) / 100f;
                    float angle = rd.Next(0, 361) * Mathf.Deg2Rad;
                    float rX = r * Mathf.Cos(angle);
                    float rY = r * Mathf.Sin(angle);
                    result.startPosition.x = rX;
                    result.startPosition.y = rY;

                    result.startMoveSpeed.x = args.emitter.launchMoveSpeed * (float)Mathf.Cos(angle);
                    result.startMoveSpeed.y = args.emitter.launchMoveSpeed * (float)Mathf.Sin(angle);

                    result.startAngleSpeed.z = args.emitter.launchAngleSpeed;
                    result.startEulerAngles.z = (angle * Mathf.Rad2Deg) - 90;
                    
                    break;
                case EObjectEmitLaunchType.FixedPoint:
                    //固定位置
                    if (args.emitter.launchFixedPointPoints.Length > 0)
                    {
                        var ponit = args.emitter.launchFixedPointPoints[(args.index + args.times) % args.emitter.launchFixedPointPoints.Length];
                        result.startPosition.x = ponit.x;
                        result.startPosition.y = ponit.y;
                        result.startPosition.z = ponit.z;

                    }

                    break;
                case EObjectEmitLaunchType.Custom:
                    if (args.emitter.launchCustomCallback != null)
                    {
                        result = args.emitter.launchCustomCallback.Calculate(args);
                    }
                    break;
             }

            return result;
        }

        //发射
        public virtual void OnLaunch(ObjectEmitLaunchParams args)
        {
            var entity = args.createResult.entity as GameObject;
            Transform trans = entity.GetComponent<Transform>();
            ObjectEmitController ctrl = entity.GetComponent<ObjectEmitController>();
            if (ctrl == null) ctrl = entity.AddComponent<ObjectEmitController>();

            trans.localPosition = args.calculateResult.startPosition;
            trans.localEulerAngles = args.calculateResult.startEulerAngles;

            ctrl.moveSpeed = args.calculateResult.startMoveSpeed;
            ctrl.rotationSpeed = args.calculateResult.startAngleSpeed;
        }
    }
}

