using Entitas;

namespace STGU3D
{
    [Game, Input, UI]
    public class DestroyedComponent : IComponent
    {
        public int what;                //移除原因                
        public bool isDestroyed;        //是否移除
    }
}
