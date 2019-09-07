
// ------------------------------ //
// Product Name : CSV_Read&Write
// Company Name : MOESTONE
// Author  Name : Eazey Wang
// Create  Data : 2017/12/16
// ------------------------------ //

using System;
using System.Collections;
using System.Collections.Generic;

namespace THGame
{
    public class CSVValue
    {
        private string m_content;

        public CSVValue(string content) { m_content = string.Format("{0}", content); }
        public CSVValue(int content) { m_content = string.Format("{0}", content); }
        public CSVValue(bool content) { m_content = string.Format("{0}", content); }
        public CSVValue(double content) { m_content = string.Format("{0}", content); }

        public override string ToString() { return m_content; }
        public int ToInt() { return int.Parse(m_content); }
        public bool ToBool() { return bool.Parse(m_content); }
        public double ToDouble() { return double.Parse(m_content); }

    }
}
