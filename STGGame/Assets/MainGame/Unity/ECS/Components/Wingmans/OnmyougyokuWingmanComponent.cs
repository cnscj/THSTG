using Entitas;

namespace STGU3D
{
    [Game]
    public class OnmyougyokuWingmanComponent : IComponent
    {
        //阴阳玉组件,默认有2个,随等级提升有多个,最大4个,弧形排列
        //射击模式:集火,散火
        //
        public GameEntity[] subWingmans;
        public bool isPointFire;

    }
}
