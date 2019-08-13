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
            //判断Excel文件中是否存在数据表
            if (dataSet.Tables.Count < 1)
                return null;

            //默认读取第一个数据表
            DataTable mSheet = dataSet.Tables[0];

            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1)
                return null;

            //读取数据表行数和列数
            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            //创建一个StringBuilder存储数据
            StringBuilder stringBuilder = new StringBuilder();

            //读取数据
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < colCount; j++)
                {
                    //使用","分割每一个数值
                    stringBuilder.Append(mSheet.Rows[i][j] + ",");
                }
                //使用换行符分割每一行
                stringBuilder.Append("\r\n");
            }

            return stringBuilder.ToString();
        }
    }

}
