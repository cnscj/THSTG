﻿using UnityEngine;
using System.Collections;

namespace THGame.EUI
{
    /// <summary>
    /// 资源加载回调
    /// </summary>
    public interface IEventListener
    {
        int EventPriority();

        bool HandleEvent(int key, object param1, object param2);
    }

}
