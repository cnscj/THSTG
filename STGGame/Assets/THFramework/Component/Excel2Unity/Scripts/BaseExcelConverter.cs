﻿using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Excel;
using UnityEngine;

namespace THEditor
{
    public abstract class BaseExcelConverter
    {
        /// <summary>
        /// 表格数据集合
        /// </summary>
        private DataSet m_ResultSet;


        public abstract string Convert(DataSet dataSet);

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

        public void Export(string savePath, Encoding encoding = null)
        {
            Export(m_ResultSet, savePath, encoding);
        }

        public void Export(DataSet dataSet, string savePath, Encoding encoding = null)
        {
            encoding = encoding != null ? encoding : Encoding.GetEncoding("utf-8");
            string content = Convert(dataSet);
            Write2File(content, savePath, encoding);
        }

    }

}
