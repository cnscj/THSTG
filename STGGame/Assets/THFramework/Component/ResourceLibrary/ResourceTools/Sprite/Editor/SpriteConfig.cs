﻿using System.Collections;
using System.Collections.Generic;
using THGame;
using UnityEditor;
using UnityEngine;

namespace THEditor
{
    public class SpriteConfig : BaseResourceConfig<SpriteConfig>
    {
        static string assetPath = ChangeAssetPath(string.Format("Assets/Resources/THSpriteConfig.asset"));

        [SerializeField] public Shader defaultShader;
        [SerializeField] public float defaultFrameRate = 12.0f;
        [SerializeField] public List<string> defaultStateList = new List<string>()
        {
            "idle","stand",
        };
        [SerializeField] public List<string> loopStateList = new List<string>()
        {
            "idle","stand",
        };
        Dictionary<string, bool> defaultStateMap = new Dictionary<string, bool>();
        Dictionary<string, bool> loopStateMap = new Dictionary<string, bool>();


        private void OnEnable()
        {
            defaultShader = defaultShader ? defaultShader : Shader.Find("Sprites/Default");
            foreach (var state in defaultStateList)
            {
                if (!defaultStateMap.ContainsKey(state))
                {
                    defaultStateMap.Add(state, true);
                }
            }

            foreach (var state in loopStateList)
            {
                if (!loopStateMap.ContainsKey(state))
                {
                    loopStateMap.Add(state, true);
                }
            }

        }

        public bool IsDefaultState(string stateName)
        {
            if (defaultStateMap.ContainsKey(stateName))
            {
                return true;
            }
            return false;
        }

        public bool IsNeedLoop(string stateName)
        {
            if (loopStateMap.ContainsKey(stateName))
            {
                return true;
            }
            return false;
        }

    }
}