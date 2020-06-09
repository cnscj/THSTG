
/*
 * 二次处理尽量少生成文件,如果已经存在,务必在原基础上改
 * 尽量确保GUID不变,引用不变
 */
namespace ASEditor
{
    public abstract class AssetCustomProcesser : AssetBaseProcesser
    {
        protected AssetCustomProcesserInfo _processerInfo;
        public AssetCustomProcesser(string name = null) : base(name)
        {
            
        }
        public void Init()
        {
            _processerInfo = OnInit();
            _progresersName = _progresersName ?? _processerInfo.name;
        }
        public int GetPriority()
        {
            return _processerInfo.priority;
        }
        public abstract AssetCustomProcesserInfo OnInit();
    }
}
