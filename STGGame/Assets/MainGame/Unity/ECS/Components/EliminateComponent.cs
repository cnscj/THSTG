using Entitas;

namespace STGU3D
{
    //消弹器
    [Game]
    public class EliminateComponent : IComponent
    {
        public float radius = 5;            //消弹半径
        public float maxHoldTime = 5.0f;    //维持时间最大值

        public float consumeRate = 1f;      //消耗比率
        public float chargeRate = 1f;       //充能比率
        public float holdTime = 5.0f;       //当前剩余时间

        public float cdTime = 3f;           //冷却时间
        public float nextChargeTime;        //下次充能时间

        public bool isEliminating;          //消除
    }

}
