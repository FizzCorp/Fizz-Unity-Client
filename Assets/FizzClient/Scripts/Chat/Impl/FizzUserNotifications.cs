using System;

using Fizz.Common;

namespace Fizz.Chat.Impl
{
    public class FizzUserNotifications : IFizzUserNotifications
    {
        private IFizzAuthRestClient _client;

        public void Open(IFizzAuthRestClient client)
        {
            IfClosed(() =>
            {
                _client = client;
            });
        }

        public void Close()
        {
            IfOpened(() =>
            {
                _client = null;
            });
        }

        public void Subscribe(Action<FizzException> callback)
        {
            IfOpened(() =>
            {
                string path = FizzConfig.API_PATH_USER_NOTIFICATIONS;
                _client.Post(FizzConfig.API_BASE_URL, path, string.Empty, (response, ex) =>
                {
                    FizzUtils.DoCallback(ex, callback);
                });
            });
        }

        public void Unsubscribe(Action<FizzException> callback)
        {
            IfOpened(() =>
            {
                string path = FizzConfig.API_PATH_USER_NOTIFICATIONS;
                _client.Delete(FizzConfig.API_BASE_URL, path, string.Empty, (response, ex) =>
                {
                    FizzUtils.DoCallback(ex, callback);
                });
            });
        }

        private void IfOpened(Action callback)
        {
            if (_client != null)
            {
                FizzUtils.DoCallback(callback);
            }
            else
            {
                FizzLogger.W("Client should have been opened.");
            }
        }

        private void IfClosed(Action callback)
        {
            if (_client == null)
            {
                FizzUtils.DoCallback(callback);
            }
            else
            {
                FizzLogger.W("Client should have been closed.");
            }
        }
    }
}