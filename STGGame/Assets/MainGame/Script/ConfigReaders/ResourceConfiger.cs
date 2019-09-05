using System;
using THGame;
using THGame.Package;

namespace STGGame
{
    public static class ResourceConfiger
    {
        private static CSVTable s_resTb;

        private static CSVTable _GetOriTable()
        {
            if (s_resTb == null)
            {
                s_resTb = new CSVTable(AssetManager.GetInstance().LoadConfig("G_Resource.csv"));
            }
            return s_resTb;
        }

        public static string GetResSrc(string key)
        {
            var tb = _GetOriTable();
            return tb[key]["src"];
        }


    }
}

