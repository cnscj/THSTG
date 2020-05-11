

namespace XLibrary
{
    public static class CSVUtil
    {

		public static string Encode(CSVTable table)
		{
            return table.GetContent();

        }
		public static CSVTable Decode(string content)
		{
            return new CSVTable(content);
        }



        //安全取得值
        public static CSVValue SafeGetValue(CSVValue val, CSVObject obj, string key2)
        {
            if (obj != null)
            {
                if (obj.HadKey(key2))
                {
                    return obj[key2];
                }
            }

            return val;
        }

        public static CSVValue SafeGetValue(CSVValue val, CSVTable table, string key1, string key2)
        {
            if (table != null)
            {
                return SafeGetValue(val, table[key1], key2);
            }

            return val;
        }

        public static void SafeSetValue(string val, CSVObject obj, string key2)
        {
            if (obj != null)
            {
                if (obj.HadKey(key2))
                {
                    obj[key2] = val;
                }
            }
        }

        public static void SafeSetValue(string val, CSVTable table, string key1, string key2)
        {
            if (table != null)
            {
                SafeSetValue(val, table[key1], key2);
            }
        }

        public static void ForceSetValue(string val, CSVTable table, string key1, string key2)
        {
            if (table != null)
            {
                var obj = table[key1];
                if (obj != null)
                {
                    table[key1] = new CSVObject(key1, null, null);
                }
                table[key1][key2] = val;
            }
        }
    }
}

