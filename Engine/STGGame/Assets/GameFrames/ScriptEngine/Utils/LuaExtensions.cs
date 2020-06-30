using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using DG.Tweening;

namespace UnityEngine
{
    public static class GameObjectExtensions
    {
        public static GameObject FindChildGameObject(this GameObject _this, string name)
        {
            GameObject ret = null;
            Transform transform = _this.transform.Find(name);
            if (transform != null && transform.gameObject != null)
            {
                ret = transform.gameObject;
            }

            return ret;
        }

        public static void SetParentGameObject(this GameObject _this, GameObject parent, bool worldPositionStays = false)
        {
            if (parent == null)
            {
                _this.transform.SetParent(null);
            }
            else
            {
                _this.transform.SetParent(parent.transform, worldPositionStays);
            }
        }

        public static void SetLayerName(this GameObject _this, string layerName, bool changeChildren)
        {
            var layer = LayerMask.NameToLayer(layerName);
            _this.layer = layer;
            if (!changeChildren) return;
            var tran = _this.transform;
            for (var i = 0; i < tran.childCount; i++)
            {
                SetLayerName(tran.GetChild(i).gameObject, layerName, changeChildren);
            }
        }

        public static string GetLayerName(this GameObject _this)
        {
            return LayerMask.LayerToName(_this.layer);
        }

        public static bool IsNull(this UnityEngine.Object unityObject)
        {
            return unityObject == null;
        }
    }

    public static class ButtonExtensions
    {
        public static void SetClickFunc(this UI.Button _this, Events.UnityAction callback)
        {
            _this.onClick.AddListener(callback);
        }
    }
}