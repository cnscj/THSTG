
using UnityEngine;

namespace THGame
{
    public abstract class ObjectCallback : MonoBehaviour
    {
        public abstract void OnLaunch(ObjectEmitter.LaunchParams launchParams);
    }
}

