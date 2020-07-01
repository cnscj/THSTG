using System.Text;

namespace THGame.UI
{
    public class RedDotParams
    {
        public string[] keys;   //默认6个

        public RedDotParams(params string[] args)
        {
            keys = args;
        }

        public override string ToString()
        {
            if (keys != null && keys.Length > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < keys.Length; i++)
                {
                    if (!string.IsNullOrEmpty(keys[i]))
                    {
                        stringBuilder.Append(string.Format("{0}_", keys[i]));
                    }
                }
                stringBuilder = stringBuilder.Length > 0 ? stringBuilder.Remove(stringBuilder.Length - 1, 1) : stringBuilder;
                return stringBuilder.ToString();
            }
            return string.Empty;
        }
    }
}

