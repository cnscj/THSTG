using UnityEngine;


namespace THGame
{
    public enum EObjectEmitLaunchType
    {
        Line,           //直线
        Sector,         //扇形
        Random,         //随机
        FixedPoint,     //定点
        Custom,         //自定义
    }
    public enum EObjectEmitCreateOrderType
    {
        Orderly,        //顺序
        Random,         //随机
        Fixed,          //固定
    }

    public class ObjectEmitCreateParams
    {
        public GameObject prefab;
        public GameObject parent;
    }

    public class ObjectEmitCreateResult
    {
        public GameObject entity;
    }

    public class ObjectEmitCalculateParams
    {
        public int times;
        public int index;
        public ObjectEmitter emitter;
    }

    public class ObjectEmitCalculateResult
    {
        public Vector3 startPosition = Vector3.zero;
        public Vector3 startEulerAngles = Vector3.zero;

        public Vector3 startMoveSpeed = Vector3.zero;
        public Vector3 startAngleSpeed = Vector3.zero;
    }

    public class ObjectEmitLaunchParams
    {
        public ObjectEmitCreateResult createResult;
        public ObjectEmitCalculateResult calculateResult;
    }
}
