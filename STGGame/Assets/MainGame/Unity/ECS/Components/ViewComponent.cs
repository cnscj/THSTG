using Entitas;
using STGGame;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class ViewComponent : IComponent
    {
        public IView view;  //抽象交互
    }

}
