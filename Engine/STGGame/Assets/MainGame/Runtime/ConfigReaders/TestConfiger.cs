using STGU3D;
using XLibrary;

namespace STGRuntime
{
    public static class ResourceConfiger
    {
        private static CSVTable s_resTb;

        private static CSVTable _GetOriTable()
        {
            if (s_resTb == null)
            {
                s_resTb = ConfigerManager.GetInstance().LoadConfig("H_Test.csv");
            }
            return s_resTb;
        }

        public static string GetResSrc(string key)
        {
            var tb = _GetOriTable();
            //return tb[key]["姓名"];
            return string.Empty;

        }


    }
}

