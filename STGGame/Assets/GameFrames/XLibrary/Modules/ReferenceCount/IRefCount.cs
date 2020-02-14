
namespace XLibGame
{
    public interface IRefCount
    {
        int RefCount();
        void Retain();
        void Release();
    }
}
