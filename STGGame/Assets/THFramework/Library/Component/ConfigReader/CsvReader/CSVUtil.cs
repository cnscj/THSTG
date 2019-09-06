

namespace THGame
{
    public static class CSVUtil
    {

		public static string Encode(CSVTable table)
		{
            return table.GetContent();

        }
		public static CSVTable Decode(string content)
		{
            return new CSVTable(content);
        }
    }
}

