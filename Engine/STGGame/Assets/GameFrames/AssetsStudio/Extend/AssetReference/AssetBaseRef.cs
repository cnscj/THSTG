using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASGame
{
    public abstract class AssetBaseRef<T> where T : UnityEngine.Object
    {
        [SerializeField, SetProperty("Asset")] private T _asset;
        [HideInInspector] [SerializeField] private string _path;

        public T Asset
        {
            get
            {
                return _asset;
            }
            set
            {
                _asset = value;
                Connect(value);
            }
        }

        public string Path { get { return _path; } set { _path = value; } }

        public void Connect(T asset)
        {
            _asset = asset;
#if UNITY_EDITOR
            if (asset != null)
            {
                _path = AssetDatabase.GetAssetPath(asset);
            }
#endif
        }

        public void Disconnect()
        {
            _asset = null;
        }

        public virtual T GetAsset(Action<T> callback = null)
        {
#if !UNITY_EDITOR
            return _asset;
#else
            if (_asset == null)
            {
                if (!string.IsNullOrEmpty(_path))
                {
                    //TODO:
                    if (callback != null) callback(default);
                    else return default;   
                }
            }
            else
            {
                //TODO:
                if (callback != null) callback(_asset);
                else return _asset;
            }
            callback?.Invoke(default);
            return default;
#endif
        }
    }
}

