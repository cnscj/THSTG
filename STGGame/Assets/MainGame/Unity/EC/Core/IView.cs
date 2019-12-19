using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public interface IView 
    {
        void SetPosition(float x, float y, float z);
        void GetPosition(ref float x, ref float y, ref float z);

        void SetRotation(float x, float y, float z);
        void GetRotation(ref float x, ref float y, ref float z);

        object Execute(int operate, object data = null);

        void Create(GameEntity entity);
        void Clear();
    }

}
