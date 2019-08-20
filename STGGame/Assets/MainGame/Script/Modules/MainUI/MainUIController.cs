using System.Collections;
using System.Collections.Generic;
using THGame;
using THGame.Package.MVC;
using UnityEngine;

namespace STGGame
{
    public class MainUIController : Controller
    {
        protected override void OnOpen()
        {
            DispatcherManager.GetInstance().Dispatch(EventType.TEST_EVENT);
        }
    }

}
