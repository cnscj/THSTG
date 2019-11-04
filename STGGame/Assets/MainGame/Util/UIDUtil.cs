using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace STGGame
{

    public static class UIDUtil
    {
        private static int m_eventUID = 10000;
        private static int m_u3dEventUID = -10000;


        public static int GetEventUID()
        {
            return ++m_eventUID;
        }

        public static int GetU3DEventUID()
        {
            return --m_u3dEventUID;
        }
    }


}
