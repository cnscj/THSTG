using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGU3D
{
    public class ViewBaseClass
    {
        public UnityView unityView { get; protected set; }
        public virtual ViewBaseClass Bind(UnityView u3dView)
        {
            Clear();

            unityView = u3dView;
            return this;
        }

        public virtual void Clear()
        {

        }
    }
}