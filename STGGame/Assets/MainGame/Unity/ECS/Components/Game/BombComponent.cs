using Entitas;

namespace STGU3D
{
    [Game]
    public class BombComponent : IComponent
    {
        public float cdTime = 5.0f;             //冷却时间
        public int times = 3;                   //拥有bomb次数
        public float nextBombTime = 0f;         //下次可bomb时间

        public bool isBombing = false;             //触发
    }
}
