using System.Collections;
using System.Collections.Generic;
using THGame.Package;
using UnityEngine;

namespace THGame
{
	public class BaseResourceManager<T> : MonoSingleton<T> where T: MonoBehaviour
    {
        protected ResourceLoader m_loader;
        private void Awake()
        {
            m_loader = ResourceLoader.GetInstance();
        }
    }

}
