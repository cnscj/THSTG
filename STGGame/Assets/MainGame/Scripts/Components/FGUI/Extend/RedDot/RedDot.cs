using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGGame.UI
{
    public class RedDot : FComponent
    {
        private RedDotParams m_redDotParams;
        public void SetKeys(RedDotParams redDotParams)
        {
            m_redDotParams = redDotParams;
        }

        public void SetKeys(string key1, string key2 = null, string key3 = null, string key4 = null)
        {
            SetKeys(new RedDotParams(key1, key2, key3, key4));
        }

        public void Update()
        {

        }

        public void Show(bool visible)
        {

        }
    }

}
