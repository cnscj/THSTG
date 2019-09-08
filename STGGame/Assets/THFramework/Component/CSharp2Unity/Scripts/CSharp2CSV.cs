using System.Collections.Generic;

namespace THGame
{
    public class CSharp2CSV : BaseCSharpConverter
    {
        public CSharp2CSV(object obj) : base(obj)
        {

        }

        public override string OnConvert(object obj)
        {
            //TODO:必须是一个或者二维数组才行
            //if (obj.GetType() == typeof(Dictionary<string, Dictionary<string, string>>))
            Dictionary<string,Dictionary<string, string>> table = (Dictionary<string, Dictionary<string, string>>)obj;
            return "";
        }
    }
}
