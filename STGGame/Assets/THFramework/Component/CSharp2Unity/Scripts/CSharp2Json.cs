
namespace THGame
{
    public class CSharp2Json : BaseCSharpConverter
    {
        public CSharp2Json(object obj) : base(obj)
        {

        }

        public override string OnConvert(object obj)
        {
            return UnityEngine.JsonUtility.ToJson(obj, true);
        }
    }
}
