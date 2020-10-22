using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace ASGame
{
    //TODO:
    public class Excel2CSharp : BaseExcelConverter
    {
        public static string CONTENT_TEMPLATE = @"
            public class $sheetName$
            {
                $sheetTypes$
            };
        ";

        public Excel2CSharp(string excelFile) : base(excelFile)
        {

        }

        public override string OnConvert(DataSet dataSet)
        {
            return "";

            
        }
    }

}
