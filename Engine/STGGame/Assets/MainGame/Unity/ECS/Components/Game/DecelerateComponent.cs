using Entitas;

namespace STGU3D
{
    [Game]
    public class DecelerateComponent : IComponent
    {
        public float speedRate = 0.5f;              //减速比
        public bool isDecelerating;                 //减速中
    }

}
