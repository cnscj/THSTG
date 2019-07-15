using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGGame
{

    public static class UIDUtil
    {
        private static int m_eventUID = 10000;


        public static int GetEventUID()
        {
            return ++m_eventUID;
        }
    }


}
