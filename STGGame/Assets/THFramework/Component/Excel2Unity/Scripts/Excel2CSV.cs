using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace THEditor
{
    public class Excel2CSV : BaseExcelConverter
    {
        public Excel2CSV(string excelFile) : base(excelFile)
        {

        }

        public override string Convert(DataSet dataSet)
        {

            var sheetData = Parse(dataSet);

            //读取数据表行数和列数
            int rowCount = sheetData.valTable.Rows.Count;
            int colCount = sheetData.valTable.Columns.Count;

            //创建一个StringBuilder存储数据
            StringBuilder stringBuilder = new StringBuilder();

            //读取数据
            for (int i = 0; i < rowCount; i++)  //从第二行开始,第一行为注释
            {
                for (int j = 0; j < colCount; j++)
                {
                    if (null == sheetData.headData[j].type || "" == sheetData.headData[j].type)
                        continue;
                    
                    //使用","分割每一个数值
                    stringBuilder.Append(sheetData.valTable.Rows[i][j] + ",");
                }
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
                //使用换行符分割每一行
                stringBuilder.Append("\r\n");
            }

            return stringBuilder.ToString();
        }
    }

}
