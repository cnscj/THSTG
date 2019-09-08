using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace THEditor
{
    public class Excel2List : BaseExcelConverter
    {
        public Excel2List(string excelFile) : base(excelFile)
        {

        }

        public override string OnConvert(DataSet dataSet)
        {
			return "";
        }

		/// <summary>
		/// 转换为实体类列表
		/// </summary>
		public List<T> Convert<T>(DataSet dataSet)
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

			//准备一个列表以保存全部数据
			List<T> list = new List<T>();

			//读取数据
			for (int i = 1; i < rowCount; i++)
			{
				//创建实例
				Type t = typeof(T);
				ConstructorInfo ct = t.GetConstructor(System.Type.EmptyTypes);
				T target = (T)ct.Invoke(null);
				for (int j = 0; j < colCount; j++)
				{
					//读取第1行数据作为表头字段
					string field = mSheet.Rows[0][j].ToString();
					object value = mSheet.Rows[i][j];
					//设置属性值
					SetTargetProperty(target, field, value);
				}

				//添加至列表
				list.Add(target);
			}

			return list;
		}

		/// <summary>
		/// 设置目标实例的属性
		/// </summary>
		private void SetTargetProperty(object target, string propertyName, object propertyValue)
		{
			//获取类型
			Type mType = target.GetType();
			//获取属性集合
			PropertyInfo[] mPropertys = mType.GetProperties();
			foreach (PropertyInfo property in mPropertys)
			{
				if (property.Name == propertyName)
				{
					property.SetValue(target, System.Convert.ChangeType(propertyValue, property.PropertyType), null);
				}
			}
		}
	}

}
