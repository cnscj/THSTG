﻿using THGame.Package;
using THGame.Package.MVC;
using UnityEngine;

namespace STGGame
{
	public class ModuleManager : MonoSingleton<ModuleManager>
    {
        private void Awake()
        {
            MVCManager.GetInstance().AddController<MainUIController>(ModuleType.MAIN_UI);
            MVCManager.GetInstance().AddController<StageController>(ModuleType.STAGE);
        }

    }


}