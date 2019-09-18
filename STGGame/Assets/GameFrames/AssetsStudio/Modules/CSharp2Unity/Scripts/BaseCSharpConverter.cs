using System.IO;
using System.Text;

namespace ASGame
{
    public abstract class BaseCSharpConverter
    {
        private object m_object;

        public abstract string OnConvert(object obj);

        ////////
        public BaseCSharpConverter(object obj)
        {
            m_object = obj;
        }

        public string Convert()
        {
            return OnConvert(m_object);
        }

        public void Write2File(string content, string savePath, Encoding encoding)
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

        public void Export(object obj, string savePath, Encoding encoding = null)
        {
            encoding = encoding != null ? encoding : Encoding.GetEncoding("utf-8");
            string content = OnConvert(obj);
            Write2File(content, savePath, encoding);
        }
    }
}
