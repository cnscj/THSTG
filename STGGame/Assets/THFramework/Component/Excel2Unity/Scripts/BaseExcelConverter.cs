using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Excel;

/*
 *  Excel解析说明
 *  表一:
 *   第一行为注释         (可以没有)
 *   第二行为字段名       (这行没有则不导出)
 *   第三行为类型         (不支持类型不导出)
 *   第四行保留          (可以没有)
 *   第五行保留          (可以没有)
 *  表二(如果有):
 *   一些配置
 */
namespace THEditor
{
    public abstract class BaseExcelConverter
    {
        /// <summary>
        /// 表格数据集合
        /// </summary>
        private DataSet m_ResultSet;


        public abstract string OnConvert(DataSet dataSet);

        ////////
        public BaseExcelConverter()
        {
        }
        public BaseExcelConverter(string excelFile)
        {
            FileStream mStream = File.Open(excelFile, FileMode.Open, FileAccess.Read);
            IExcelDataReader mExcelReader = ExcelReaderFactory.CreateOpenXmlReader(mStream);
            m_ResultSet = mExcelReader.AsDataSet();
        }


        public void Write2File(string content,string savePath, Encoding encoding)
        {
            //写入文件
            using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
                {
                    textWriter.Write(content);
                }
            }
            
        }

        public string Convert()
        {
            return OnConvert(m_ResultSet);
        }

        public void Export(string savePath, Encoding encoding = null)
        {
            Export(m_ResultSet, savePath, encoding);
        }

        public void Export(DataSet dataSet, string savePath, Encoding encoding = null)
        {
            encoding = encoding != null ? encoding : Encoding.GetEncoding("utf-8");
            string content = OnConvert(dataSet);
            Write2File(content, savePath, encoding);
        }

        public Excel2UnityData Parse(DataSet dataSet)
        {
            //无效参数
            if (dataSet == null)
                return null;

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

            Excel2UnityData sheetData = new Excel2UnityData();
       
            for (int j = 0; j < colCount; j++)
            {
                Excel2UnityHeadData coldata = new Excel2UnityHeadData();
                
                for (int i = 0; i < 5; i++)
                {
                    if (i == 0)
                    {
                        coldata.note = string.Format("{0}", mSheet.Rows[i][j]);
                    }
                    else if (i == 1)
                    {
                        coldata.field = string.Format("{0}", mSheet.Rows[i][j]);
                    }
                    else if (i == 2)
                    {
                        coldata.type = string.Format("{0}", mSheet.Rows[i][j]);
                    }
                    else if (i == 3)
                    {
                        coldata.client = string.Format("{0}", mSheet.Rows[i][j]);
                    }
                    else if (i == 4)
                    {
                        coldata.server = string.Format("{0}", mSheet.Rows[i][j]);
                    }
                }
                sheetData.headData.Add(coldata);
            }

            sheetData.valTable = mSheet.Copy();
            //倒序删,防止移动报错
            sheetData.valTable.Rows.RemoveAt(4);
            sheetData.valTable.Rows.RemoveAt(3);
            sheetData.valTable.Rows.RemoveAt(2);
            sheetData.valTable.Rows.RemoveAt(0);

            return sheetData;
        }
    }

}
