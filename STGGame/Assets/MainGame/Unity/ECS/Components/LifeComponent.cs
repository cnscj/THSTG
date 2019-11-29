using System.Collections.Generic;
using Entitas;

namespace STGU3D
{
    [Game]
    public class LifeComponent : IComponent
    {
        public int maxLife = 3;                         //最大生命次数

        public int life = 1;                            //当前生命次数
        public int lastLife = 1;                        //上次生命值
    }
}
