using System;
using UnityEditor;

namespace ASGame
{
    public class EditorLoader : BaseLoader
    {
        public override void LoadAtPath<T>(string path, Action<AssetLoadResult<T>> result)
        {
            T obj = AssetDatabase.LoadAssetAtPath<T>(path);
            result?.Invoke(new AssetLoadResult<T>()
            {
                asset = obj,
                isDone = true,
                status = LoadStatus.LOAD_SUCCESS
            });
        }

        public override void Unload(string path)
        {
            
        }
    }
}

