using Entitas;
namespace STGU3D
{
    [Game]
    public class HealthComponent : IComponent
    {
        public float maxNearDeathTime = 1f;             //最大频死时间
        public int maxBlood = 100;                      //最大生命值

        public int blood = 100;                         //当前生命值
        public float trueDeathTime = -1f;               //真实死亡时间
        public bool isTrueDied;                         //是否已经死亡

        public int prevBlood = 100;                     //上一次血量
    }

}
