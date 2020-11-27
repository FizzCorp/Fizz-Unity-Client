using System;
using System.Collections.Generic;

using Fizz.Common;
using Fizz.Common.Json;

namespace Fizz.Chat.Impl 
{
    public class FizzGroups: IFizzGroups 
    {
        private static readonly FizzException ERROR_INVALID_RESPONSE_FORMAT = new FizzException (FizzError.ERROR_REQUEST_FAILED, "invalid_response_format");
        private static readonly FizzException ERROR_INVALID_MESSAGE_QUERY_COUNT = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_query_count");

        private IFizzAuthRestClient _restClient;

        public void Open(IFizzAuthRestClient client) 
        {
            IfClosed(() => 
            {
                _restClient = client;
            });
        }

        public void Close() 
        {
            IfOpened(() => 
            {
                _restClient = null;
            });
        }

        public void FetchGroup(string groupId, Action<IFizzGroup, FizzException> callback) 
        {
            IfOpened(() => 
            {
                string path = string.Format(FizzConfig.API_PATH_GROUP, groupId);
                _restClient.Get(FizzConfig.API_BASE_URL, path, (response, ex) => {
                    if (ex != null) {
                        FizzUtils.DoCallback<IFizzGroup> (null, ex, callback);
                    }
                    else {
                        FizzUtils.DoCallback<IFizzGroup>(new FizzJsonGroup(response), null, callback);
                    }
                });
            });
        }

        public void GetGroupMembers(string groupId, Action<IList<IFizzGroupMember>, FizzException> callback) 
        {
            IfOpened(() => 
            {
                string path = string.Format(FizzConfig.API_PATH_GROUP_MEMBERS, groupId);
                _restClient.Get(FizzConfig.API_BASE_URL, path, (response, ex) => {
                    if (ex != null) {
                        FizzUtils.DoCallback<IList<IFizzGroupMember>>(null, ex, callback);
                    }
                    else {
                        JSONArray membersArr = JSONNode.Parse (response).AsArray;
                        IList<IFizzGroupMember> members = new List<IFizzGroupMember> ();
                        foreach (JSONNode member in membersArr.Childs)
                        {
                            members.Add (new FizzJsonGroupMember(member));
                        }

                        FizzUtils.DoCallback<IList<IFizzGroupMember>> (members, null, callback);
                    }
                });
            });
        }

        public void PublishMessage(
            string groupId,
            string nick, 
            string body, 
            Dictionary<string, string> data, 
            bool translate, 
            bool filter, 
            bool persist, 
            Action<FizzException> callback) 
        {
            IfOpened(() => 
            {
                string path = string.Format(FizzConfig.API_PATH_GROUP_MESSAGES, groupId);
                JSONClass json = new JSONClass();
                json[FizzJsonChannelMessage.KEY_NICK] = nick;
                json[FizzJsonChannelMessage.KEY_BODY] = body;
                json[FizzJsonChannelMessage.KEY_PERSIST].AsBool = persist;
                json[FizzJsonChannelMessage.KEY_FILTER].AsBool = filter;
                json[FizzJsonChannelMessage.KEY_TRANSLATE].AsBool = translate;

                string dataStr = string.Empty;
                if (data != null)
                {
                    JSONClass dataJson = new JSONClass();
                    foreach (KeyValuePair<string, string> pair in data)
                    {
                        dataJson.Add(pair.Key, new JSONData(pair.Value));
                    }

                    dataStr = dataJson.ToString();
                }

                json[FizzJsonChannelMessage.KEY_DATA] = dataStr;

                _restClient.Post (FizzConfig.API_BASE_URL, path, json.ToString (), (response, ex) =>
                {
                    FizzUtils.DoCallback (ex, callback);
                });
            });
        }

        public void QueryLatest(
            string groupId, 
            int count, 
            Action<IList<FizzChannelMessage>, FizzException> callback) 
        {
            QueryLatest(groupId, count, -1, callback);
        }

        public void QueryLatest(
            string groupId, 
            int count, 
            long beforeId, 
            Action<IList<FizzChannelMessage>, FizzException> callback) 
        {
            IfOpened (() =>
            {
                if (count < 0)
                {
                    FizzUtils.DoCallback<IList<FizzChannelMessage>> (null, ERROR_INVALID_MESSAGE_QUERY_COUNT, callback);
                    return;
                }
                if (count == 0)
                {
                    FizzUtils.DoCallback<IList<FizzChannelMessage>> (new List<FizzChannelMessage> (), null, callback);
                    return;
                }

                string path = string.Format (FizzConfig.API_PATH_GROUP_MESSAGES, groupId) + "?count=" + count;
                if (beforeId > 0)
                {
                    path += "&before_id=" + beforeId;
                }
                _restClient.Get (FizzConfig.API_BASE_URL, path, (response, ex) =>
                {
                    if (ex != null)
                    {
                        FizzUtils.DoCallback<IList<FizzChannelMessage>> (null, ex, callback);
                    }
                    else
                    {
                        try
                        {
                            JSONArray messagesArr = JSONNode.Parse (response).AsArray;
                            IList<FizzChannelMessage> messages = new List<FizzChannelMessage> ();
                            foreach (JSONNode message in messagesArr.Childs)
                            {
                                messages.Add (new FizzJsonChannelMessage (message));
                            }
                            FizzUtils.DoCallback<IList<FizzChannelMessage>> (messages, null, callback);
                        }
                        catch
                        {
                            FizzUtils.DoCallback<IList<FizzChannelMessage>> (null, ERROR_INVALID_RESPONSE_FORMAT, callback);
                        }
                    }
                });
            });
        }

        private void IfOpened (Action callback)
        {
            if (_restClient != null)
            {
                FizzUtils.DoCallback (callback);
            }
            else
            {
                FizzLogger.W ("Client should have been opened.");
            }
        }

        private void IfClosed (Action callback)
        {
            if (_restClient == null)
            {
                FizzUtils.DoCallback (callback);
            }
            else
            {
                FizzLogger.W ("Client should have been closed.");
            }
        } 
    }
}