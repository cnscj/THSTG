
namespace XLibGame
{
    public interface IRefCount
    {
        int RefCount { get; }
        void Retain();
        void Release();
    }
}
