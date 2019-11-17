using Entitas;
using THGame;


namespace STGU3D
{
    //请直接使用组件BehaviourMapper
    [Input]
    public class InputComponent : IComponent
    {
        public int type;//类型
        public BehaviourMapper keymaps;
    }

}
