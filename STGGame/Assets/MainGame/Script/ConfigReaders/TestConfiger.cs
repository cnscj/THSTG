using XLibrary;

namespace STGGame
{
    public static class ResourceConfiger
    {
        private static CSVTable s_resTb;

        private static CSVTable _GetOriTable()
        {
            if (s_resTb == null)
            {
                s_resTb = CSVUtil.Decode(AssetManager.GetInstance().LoadConfig("H_Test.csv"));
            }
            return s_resTb;
        }

        public static string GetResSrc(string key)
        {
            var tb = _GetOriTable();
            return tb[key]["姓名"];

        }


    }
}

