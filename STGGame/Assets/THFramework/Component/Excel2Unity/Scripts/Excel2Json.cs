using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace THEditor
{
    public class Excel2Json : BaseExcelConverter
    {
        public Excel2Json(string excelFile) : base(excelFile)
        {

        }
        public override string Convert(DataSet dataSet)
        {
            var sheetData = Parse(dataSet);

            //读取数据表行数和列数
            int rowCount = sheetData.valTable.Rows.Count;
            int colCount = sheetData.valTable.Columns.Count;

            //准备一个列表存储整个表的数据
            List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

            //读取数据
            for (int i = 1; i < rowCount; i++)
            {
                //准备一个字典存储每一行的数据
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int j = 0; j < colCount; j++)
                {
                    if (null == sheetData.headData[j].type || "" == sheetData.headData[j].type)
                        continue;

                    //读取第1行数据作为表头字段
                    string field = sheetData.headData[j].field;
                    //Key-Value对应
                    if (sheetData.headData[j].type == "int")
                        row[field] = int.Parse(sheetData.valTable.Rows[i][j].ToString());
                    else if (sheetData.headData[j].type == "string")
                        row[field] = sheetData.valTable.Rows[i][j].ToString();
                    else
                        row[field] = sheetData.valTable.Rows[i][j];
                }

                //添加到表数据中
                table.Add(row);
            }

            //生成Json字符串
            string json = JsonConvert.SerializeObject(table, Newtonsoft.Json.Formatting.Indented);

            return json;
        }
    }

}
