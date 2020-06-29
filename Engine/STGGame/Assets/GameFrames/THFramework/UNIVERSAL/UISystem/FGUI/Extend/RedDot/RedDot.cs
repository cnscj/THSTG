using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THGame.UI
{
    public class RedDot : FWidget
    {
        private RedDotParams m_redDotParams;
        public RedDot(string packageName, string componentName):base(packageName, componentName)
        {

        }
        public void SetKeys(RedDotParams redDotParams)
        {
            //反注册
            RedDotManager.GetInstance();
            //注册

            m_redDotParams = redDotParams;
        }

        public RedDotParams GetKeys()
        {
            return m_redDotParams;
        }

        private void OnCall()
        {

        }

    }

}
