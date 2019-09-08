using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace THEditor
{
    public class Excel2Lua : BaseExcelConverter
    {
        public Excel2Lua(string excelFile) : base(excelFile)
        {

        }

        public override string OnConvert(DataSet dataSet)
        {
            //判断Excel文件中是否存在数据表
            if (dataSet.Tables.Count < 1)
                return null;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("local datas = {");
            stringBuilder.Append("\r\n");

            //读取数据表
            foreach (DataTable mSheet in dataSet.Tables)
            {
                //判断数据表内是否存在数据
                if (mSheet.Rows.Count < 1)
                    continue;

                //读取数据表行数和列数
                int rowCount = mSheet.Rows.Count;
                int colCount = mSheet.Columns.Count;

                //准备一个列表存储整个表的数据
                List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

                //读取数据
                for (int i = 1; i < rowCount; i++)
                {
                    //准备一个字典存储每一行的数据
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    for (int j = 0; j < colCount; j++)
                    {
                        //读取第1行数据作为表头字段
                        string field = mSheet.Rows[0][j].ToString();
                        //Key-Value对应
                        row[field] = mSheet.Rows[i][j];
                    }
                    //添加到表数据中
                    table.Add(row);
                }
                stringBuilder.Append(string.Format("\t\"{0}\" = ", mSheet.TableName));
                stringBuilder.Append("{\r\n");
                foreach (Dictionary<string, object> dic in table)
                {
                    stringBuilder.Append("\t\t{\r\n");
                    foreach (string key in dic.Keys)
                    {
                        if (dic[key].GetType().Name == "String")
                            stringBuilder.Append(string.Format("\t\t\t\"{0}\" = \"{1}\",\r\n", key, dic[key]));
                        else
                            stringBuilder.Append(string.Format("\t\t\t\"{0}\" = {1},\r\n", key, dic[key]));
                    }
                    stringBuilder.Append("\t\t},\r\n");
                }
                stringBuilder.Append("\t}\r\n");
            }

            stringBuilder.Append("}\r\n");
            stringBuilder.Append("return datas");

            //写入文件
            return stringBuilder.ToString();
        }
    }

}
