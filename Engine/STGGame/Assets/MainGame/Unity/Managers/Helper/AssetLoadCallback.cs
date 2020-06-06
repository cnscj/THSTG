using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLibGame;

namespace STGU3D
{
    public class AssetLoadCallback<T> : Callback<T, int>
    {
        public new static AssetLoadCallback<T> GetOrNew() { return Callback<T, int>.GetOrNew() as AssetLoadCallback<T>; }
    }

    public class AssetLoadCallback<T1,T2> : Callback<T1, T2, int>
    {
        public new static AssetLoadCallback<T1, T2> GetOrNew() { return Callback<T1, T2, int>.GetOrNew() as AssetLoadCallback<T1, T2>; }
    }
}
