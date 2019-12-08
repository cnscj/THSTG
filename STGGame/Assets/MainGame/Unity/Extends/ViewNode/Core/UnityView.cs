using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGGame
{
    public class UnityView : IView
    {
        public GameObject viewGO;           //与Unity关联的节点

        public UnityView()
        {
            viewGO = new GameObject("View");

        }
    }
}
