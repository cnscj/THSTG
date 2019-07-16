using UnityEngine;
using System.Collections;
using THGame.Package;
using THGame;

namespace STGGame
{
    public class DispatcherManager : MonoBehaviour
    {
        private static Dispatcher s_dispatcher = new Dispatcher();
        public static Dispatcher GetInstance()
        {
            return s_dispatcher;
        }
    }
}
