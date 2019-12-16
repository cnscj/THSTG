using Entitas;
using STGGame;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class ViewComponent : IComponent
    {
        public IView view;          //抽象交互
        public bool isEditor;       //是否为编辑器节点-开启后Entity受View影响
    }

}
