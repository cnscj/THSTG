using Entitas;

namespace STGU3D
{
    //消弹器
    [Game]
    public class EliminateComponent : IComponent
    {
        public float radius = 5;            //消弹半径
        public float maxHoldTime = 5.0f;    //维持时间最大值

        public float holdTime = 5.0f;       //当前剩余时间
    }

}
