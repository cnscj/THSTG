﻿using STGService.MVC;

namespace STGService
{
    public static class ModuleMgr
    {
        public static void InitAwake()
        {
            MVCManager.GetInstance().AddController<TestController>();
            MVCManager.GetInstance().AddController<MainUIController>();
            MVCManager.GetInstance().AddController<StageController>();
        }
    }

}