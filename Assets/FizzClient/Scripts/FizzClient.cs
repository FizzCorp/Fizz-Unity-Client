using Fizz.Chat;
using Fizz.Chat.Impl;
using Fizz.Common;
using Fizz.Ingestion;
using Fizz.Ingestion.Impl;
using Fizz.Moderation;
using Fizz.Moderation.Impl;
using Fizz.Threading;
using System;

namespace Fizz
{
    public enum FizzClientState
    {
        Closed,
        Opened
    }

    [Flags]
    public enum FizzServices
    {
        Chat = 1 << 0,
        Analytics = 1 << 1,
        Moderation = 1 << 2,
        All = Chat | Analytics | Moderation
    }

    public interface IFizzClient
    {
        void Open(string userId, string locale, FizzServices services, Action<FizzException> callback);
        void Close(Action<FizzException> callback);
        void Update();

        IFizzChatClient Chat { get; }
        IFizzIngestionClient Ingestion { get; }
        IFizzModerationClient Moderation { get; }
        FizzClientState State { get; }
        string Version { get; }
    }

    public class FizzClient : IFizzClient
    {
        readonly FizzChatClient _chat;
        readonly IFizzRestClient _restClient;
        readonly IFizzAuthRestClient _authClient;
        readonly IFizzSessionProvider _sessionClient;
        readonly FizzIngestionClient _ingestionClient;
        readonly FizzModerationClient _moderationClient;
        readonly FizzActionDispatcher _dispatcher = new FizzActionDispatcher();

        public FizzClient(string appId, string appSecret)
        {
            if (string.IsNullOrEmpty(appId))
            {
                throw FizzException.ERROR_INVALID_APP_ID;
            }

            if (string.IsNullOrEmpty (appSecret))
            {
                throw FizzException.ERROR_INVALID_APP_SECRET;
            }

            _chat = new FizzChatClient(appId, _dispatcher);
            _restClient = new FizzRestClient(_dispatcher);
            _sessionClient = new FizzIdSecretSessionProvider(appId, appSecret, _restClient);
            _authClient = new FizzAuthRestClient(_restClient);
            _ingestionClient = new FizzIngestionClient(new FizzInMemoryEventLog(), _dispatcher);
            _moderationClient = new FizzModerationClient ();
        }

        public FizzClient(string appId, IFizzSessionProvider sessionClient)
        {
            if (string.IsNullOrEmpty(appId))
            {
                throw FizzException.ERROR_INVALID_APP_ID;
            }

            _sessionClient = sessionClient;
            _chat = new FizzChatClient(appId, _dispatcher);
            _restClient = new FizzRestClient(_dispatcher);
            _authClient = new FizzAuthRestClient(_restClient);
            _ingestionClient = new FizzIngestionClient(new FizzInMemoryEventLog(), _dispatcher);
            _moderationClient = new FizzModerationClient ();
        }

        public void Open(string userId, string locale, FizzServices services, Action<FizzException> callback)
        {
            try
            {
                if (State == FizzClientState.Opened)
                    return;

                FizzSessionRepository sessionRepo = new FizzSessionRepository(userId, locale, _sessionClient);
                _authClient.Open(sessionRepo, ex =>
                {
                    if (ex == null)
                    {
                        if (services.HasFlag(FizzServices.Chat))
                        {
                            _chat.Open(userId, _authClient, sessionRepo);
                        }
                        if (services.HasFlag(FizzServices.Analytics))
                        {
                            _ingestionClient.Open(userId, sessionRepo.Session._serverTS, _authClient);
                        }
                        if (services.HasFlag(FizzServices.Moderation))
                        {
                            _moderationClient.Open (_authClient);
                        }

                        State = FizzClientState.Opened;
                        FizzUtils.DoCallback(null, callback);
                    }
                    else
                    {
                        FizzUtils.DoCallback(ex, callback);
                    }
                });
            }
            catch (FizzException ex)
            {
                FizzUtils.DoCallback(ex, callback);
            }
        }

        public void Close(Action<FizzException> callback)
        {
            try
            {
                if (State == FizzClientState.Closed)
                    return;

                Close(() => { FizzUtils.DoCallback(null, callback); });
            }
            catch (FizzException ex)
            {
                FizzUtils.DoCallback(ex, callback);
            }
        }

        public void Update()
        {
            _dispatcher.Process();
        }

        public IFizzChatClient Chat
        {
            get
            {
                return _chat;
            }
        }

        public IFizzIngestionClient Ingestion
        {
            get
            {
                return _ingestionClient;
            }
        }

        public IFizzModerationClient Moderation
        {
            get
            {
                return _moderationClient;
            }
        }

        public FizzClientState State { get; private set; }

        public string Version { get { return "v1.5.0"; } }

        private void Close(Action callback)
        {
            _ingestionClient.Close(() =>
            {
                _authClient.Close();
                _chat.Close(callback);
                _moderationClient.Close ();
                State = FizzClientState.Closed;
            });
        }
    }
}
