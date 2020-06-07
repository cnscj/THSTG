using System.Collections;
using System.Collections.Generic;
using System.IO;
using ASGame;
using UnityEngine;
namespace STGU3D
{
    public static class AssetFileBook
    {
        public static readonly string AB_ASSET_ROOT = "/Users/cnscj/UnityWorkspace/THSTG/Game/Resource";
        public static readonly string ED_ASSET_ROOT = "assets/gameassets";
        public static readonly string EFFECT_ROOT = Path.Combine(ED_ASSET_ROOT, "effect");
        public static readonly string SPRITE_ROOT = Path.Combine(ED_ASSET_ROOT, "sprite");
        public static readonly string MODEL_ROOT = Path.Combine(ED_ASSET_ROOT, "model");
        public static readonly string FGUI_ROOT = Path.Combine(ED_ASSET_ROOT, "fgui");
        public static readonly string FGUI_ROOT_SRC = "Assets/GameAssets/FGUI";
        public static readonly string CONFIG_ROOT = Path.Combine(ED_ASSET_ROOT, "config");

        private static string s_abAssetRootWithPlatform;
        public static string GetAbAssetRoot()
        {
            return s_abAssetRootWithPlatform ?? Path.Combine(AB_ASSET_ROOT, AssetPathUtil.GetCurPlatformName()).ToLower();
        }

        public static string GetEffectPath(string key)
        {
            string abPath = Path.Combine(GetAbAssetRoot(), string.Format("{0}.ab", key));
            string assetName = Path.Combine(EFFECT_ROOT, string.Format("{0}.prefab",key));
            return string.Format("{0}|{1}", abPath, assetName);
        }

        public static string GetSpritePath(string key)
        {
            string abPath = Path.Combine(GetAbAssetRoot(), string.Format("{0}.ab", key));
            string assetName = Path.Combine(SPRITE_ROOT, string.Format("{0}.prefab", key));
            return string.Format("{0}|{1}", abPath, assetName);
        }

        public static string GetModelPath(string key)
        {
            string abPath = Path.Combine(GetAbAssetRoot(), string.Format("{0}.ab", key));
            string assetName = Path.Combine(MODEL_ROOT, string.Format("{0}.prefab", key));
            return string.Format("{0}|{1}", abPath, assetName);
        }

        public static string GetUIPath(string key)
        {
            string abPath = Path.Combine(GetAbAssetRoot(), string.Format("{0}.ab", key));
            return string.Format("{0}", abPath);
        }

        public static string GetConfigPath(string key)
        {
            string abPath = Path.Combine(GetAbAssetRoot(), string.Format("{0}.ab", key));
            string assetName = Path.Combine(CONFIG_ROOT, string.Format("{0}.csv", key));
            return string.Format("{0}|{1}", abPath, assetName);
        }

        public static string GetBundleMainfest()
        {
            string platform = Path.GetFileNameWithoutExtension(GetAbAssetRoot());
            string abPath = Path.Combine(GetAbAssetRoot(), string.Format("{0}.ab", platform));
            return string.Format("{0}", abPath);
        }
    }
}

