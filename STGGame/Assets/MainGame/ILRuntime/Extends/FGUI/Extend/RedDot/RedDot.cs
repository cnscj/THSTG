using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGService.UI
{
    public class RedDot : FComponent
    {
        private RedDotParams m_redDotParams;
        public void SetKeys(RedDotParams redDotParams)
        {
            if (redDotParams == null)
            {
                //反注册
            }
            else
            {
                //注册
            }
            m_redDotParams = redDotParams;
        }

        public RedDotParams GetKeys()
        {
            return m_redDotParams;
        }

    }

}
