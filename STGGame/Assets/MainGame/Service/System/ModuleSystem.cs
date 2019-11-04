using System.Collections.Generic;
using STGService.MVC;
using STGU3D;
using XLibGame;

namespace STGService
{
    public static class ModuleSystem
    {
        public static void InitAwake()
        {
            MVCManager.GetInstance().AddController<MainUIController>();
            MVCManager.GetInstance().AddController<StageController>();
            MVCManager.GetInstance().AddController<TestController>();

        }
    }

}
