using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace STGGame
{
    public interface IView 
    {
        Vector3 Position { get; set; }
        Vector3 Rotation { get; set; }
        Vector3 Scale { get; set; }

        object Execute(int operate, object data = null);

        IView Create(GameEntity entity);
        void Clear();
    }

}
