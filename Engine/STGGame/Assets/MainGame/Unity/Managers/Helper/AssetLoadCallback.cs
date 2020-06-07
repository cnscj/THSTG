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
}
