using System;

namespace Fizz.Chat 
{
    public interface IFizzUserNotifications
    {
        void Subscribe(Action<FizzException> callback);
        void Unsubscribe(Action<FizzException> callback);
    }
}
