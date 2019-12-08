﻿using Entitas;
using STGGame;
using UnityEngine;

namespace STGU3D
{
    [Game]
    public class ViewComponent : IComponent
    {
        public IView view;  
        public GameObject viewGO;           //与Unity关联的节点
        public Renderer renderer;
        public Animator animator;
        public Collider collider;
    }

}
