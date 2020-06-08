
// ------------------------------ //
// Product Name : CSV_Read&Write
// Company Name : MOESTONE
// Author  Name : Eazey Wang
// Create  Data : 2017/12/16
// ------------------------------ //

using UnityEngine;

namespace XLibrary
{
    public class CSVValue
    {
        private string m_content;

        public CSVValue(string content) { m_content = string.Format("{0}", content); }
        public CSVValue(int content) { m_content = string.Format("{0}", content); }
        public CSVValue(bool content) { m_content = string.Format("{0}", content); }
        public CSVValue(float content) { m_content = string.Format("{0}", content); }
        public CSVValue(double content) { m_content = string.Format("{0}", content); }
        public CSVValue(Vector3 content) { m_content = string.Format("{0},{1},{2}", content.x,content.y,content.z); }

        public override string ToString() { return m_content; }
        public int ToInt() { return int.Parse(m_content); }
        public bool ToBool() { return bool.Parse(m_content); }
        public float ToFloat() { return float.Parse(m_content); }
        public double ToDouble() { return double.Parse(m_content); }
        public Vector3 ToVector3() {
            string []xyz = m_content.Split(',');
            Vector3 vec3 = new Vector3(
                xyz.Length > 0 ? float.Parse(xyz[0]) : 0,
                xyz.Length > 1 ? float.Parse(xyz[1]) : 0,
                xyz.Length > 2 ? float.Parse(xyz[2]) : 0);
            return vec3;
        }

        //隐式转换
        public static implicit operator CSVValue(string v)
        {
            return new CSVValue(v);
        }

        public static implicit operator string(CSVValue v)
        {
            return v.ToString();
        }

        public static implicit operator CSVValue(int v)
        {
            return new CSVValue(v);
        }
    }
}
