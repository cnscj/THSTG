using System;

namespace XLibrary.Package
{
    public interface IObserver
    {
        void Notified(Subject subject, Object date);
    }
    
}
