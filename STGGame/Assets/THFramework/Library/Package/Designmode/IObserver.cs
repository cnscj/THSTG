using System;

namespace THGame.Package
{
    public interface IObserver
    {
        void Notified(Subject subject, Object date);
    }
    
}
