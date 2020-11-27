using System;

using Fizz.Common;
using Fizz.Common.Json;

namespace Fizz.Chat.Impl 
{
    public class FizzUsers: IFizzUsers 
    {
        private static readonly FizzException ERROR_INVALID_RESPONSE_FORMAT = new FizzException(FizzError.ERROR_REQUEST_FAILED, "invalid_response_format");

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

        public void GetUser(string userId, Action<IFizzUser, FizzException> callback)
        {
            IfOpened(() =>
            {
                string path = string.Format(FizzConfig.API_PATH_USER, userId);
                _client.Get(FizzConfig.API_BASE_URL, path, (response, ex) =>
                {
                    if (ex != null)
                    {
                        FizzUtils.DoCallback<IFizzUser>(null, ex, callback);
                    }
                    else
                    {
                        try
                        {
                            JSONNode userResponse = JSONNode.Parse(response);
                            FizzJsonUser user = new FizzJsonUser(userId, userResponse);
                            FizzUtils.DoCallback<IFizzUser>(user, null, callback);
                        }
                        catch
                        {
                            FizzUtils.DoCallback<IFizzUser>(null, ERROR_INVALID_RESPONSE_FORMAT, callback);
                        }
                    }
                });
            });
        }

        public void Subscribe(string userId, Action<FizzException> callback)
        {
            IfOpened(() =>
            {
                string path = string.Format(FizzConfig.API_PATH_USER_SUBCRIBERS, userId);
                _client.Post(FizzConfig.API_BASE_URL, path, string.Empty, (response, ex) =>
                {
                    FizzUtils.DoCallback(ex, callback);
                });
            });
        }

        public void Unsubscribe(string userId, Action<FizzException> callback)
        {
            IfOpened(() =>
            {
                string path = string.Format(FizzConfig.API_PATH_USER_SUBCRIBERS, userId);
                _client.Delete(FizzConfig.API_BASE_URL, path, string.Empty, (response, ex) =>
                {
                    FizzUtils.DoCallback(ex, callback);
                });
            });
        }

        public IFizzFetchUserGroupsQuery BuildFetchUserGroupsQuery(string userId)
        {
            return new FizzFetchUserGroupsQuery(userId, _client);
        }

        public void UpdateGroup(
            string userId,
            string groupId,
            FizzGroupMemberState? state,
            long? lastReadMessageId,
            Action<FizzException> callback
        )
        {
            IfOpened(() =>
            {
                string path = string.Format(FizzConfig.API_PATH_USER_GROUP, userId, groupId);
                JSONClass body = new JSONClass();
                if (state != null)
                {
                    body["state"] = Value(state.Value);
                }
                if (lastReadMessageId != null)
                {
                    body["last_read_message_id"].AsDouble = lastReadMessageId.Value;
                }

                _client.Post(FizzConfig.API_BASE_URL, path, body.ToString(), (response, ex) =>
                {
                    FizzUtils.DoCallback(ex, callback);
                });
            });
        }

        public void RemoveGroup(string userId, string groupId, Action<FizzException> callback)
        {
            IfOpened(() =>
            {
                string path = string.Format(FizzConfig.API_PATH_USER_GROUP, userId, groupId);

                _client.Delete(FizzConfig.API_BASE_URL, path, string.Empty, (response, ex) =>
                {
                    FizzUtils.DoCallback(ex, callback);
                });
            });
        }

        private string Value(FizzGroupMemberState state)
        {
            switch (state)
            {
                case FizzGroupMemberState.Joined:
                    return "joined";

                case FizzGroupMemberState.Pending:
                    return "pending";

                default:
                    return "";
            }
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