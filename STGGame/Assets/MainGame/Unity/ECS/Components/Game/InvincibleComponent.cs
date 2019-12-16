
using Entitas;

namespace STGU3D
{
    public class InvincibleComponent : IComponent
    {
        public float time = 0f;             //保护剩余时间

        public bool isInvincible;
    }

}
