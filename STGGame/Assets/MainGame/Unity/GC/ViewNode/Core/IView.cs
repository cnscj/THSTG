using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public interface IView 
    {
        object GetObject();
        void SetObject(object obj);

        void SetPosition(in float x, in float y, in float z);
        void GetPosition(out float x, out float y, out float z);

        void SetRotation(in float x, in float y, in float z);
        void GetRotation(out float x, out float y, out float z);

        object Execute(int operate, object data = null);

        void Create(GameEntity entity);
        void Clear();
    }

}
