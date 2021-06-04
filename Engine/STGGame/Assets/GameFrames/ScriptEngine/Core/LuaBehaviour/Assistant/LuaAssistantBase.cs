﻿using UnityEngine;
namespace SEGame
{
    public class LuaAssistantBase : MonoBehaviour
    {
        protected LuaBehaviour luaBehaviour;

        protected virtual void Awake()
        {
            luaBehaviour = GetComponent<LuaBehaviour>();
        }
    }
}