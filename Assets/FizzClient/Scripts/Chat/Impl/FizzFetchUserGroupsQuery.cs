using System;
using System.Collections.Generic;

using Fizz.Common;
using Fizz.Common.Json;

namespace Fizz.Chat.Impl 
{
    public class FizzFetchUserGroupsQuery: IFizzFetchUserGroupsQuery 
    {
        private string _next;
        private readonly string _userId;
        private readonly IFizzAuthRestClient _restClient;

        public FizzFetchUserGroupsQuery(string userId, IFizzAuthRestClient restClient) 
        {
            _userId = userId;
            _restClient = restClient;
            HasNext = true;
        }

        public bool HasNext { get; private set; }

        public void Next(Action<IList<IFizzUserGroup>, FizzException> callback) 
        {
            string path = string.Format(FizzConfig.API_PATH_USER_GROUPS, _userId) + "?page_size=100";
            if (_next != null && _next != string.Empty) 
            {
                path += "&page=" + _next;
            }

            _restClient.Get(FizzConfig.API_BASE_URL, path, (response, ex) => 
            {
                if (ex != null) 
                {
                    FizzUtils.DoCallback<IList<IFizzUserGroup>>(null, ex, callback);
                }
                else 
                {
                    try 
                    {
                        FizzUtils.DoCallback<IList<IFizzUserGroup>>(ParseResponse(response), null, callback);
                    }
                    catch 
                    {
                        FizzUtils.DoCallback<IList<IFizzUserGroup>>(null, FizzRestClient.ERROR_INVALID_RESPONSE_FORMAT, callback);
                    }
                }
            });
        }

        private IList<IFizzUserGroup> ParseResponse(string response) 
        {
            FizzLogger.D("query parse links: " + response);

            JSONClass json = JSONNode.Parse(response).AsObject;
            if (!json.HasKey("links") || !json.HasKey("groups")) 
            {
                throw FizzRestClient.ERROR_INVALID_RESPONSE_FORMAT;
            }

            JSONClass links = json["links"].AsObject;
            ParseLinks(links);

            JSONArray groupsElement = json["groups"].AsArray;

            return ParseGroups(groupsElement);
        }

        private IList<IFizzUserGroup> ParseGroups(JSONArray groupsArr) 
        {
            IList<IFizzUserGroup> groups = new List<IFizzUserGroup> ();

            foreach (JSONNode group in groupsArr.Childs)
            {
                groups.Add(new FizzJsonUserGroup(group));
            }

            return groups;
        }

        private void ParseLinks(JSONClass links) 
        {
            if (links.HasKey("next")) 
            {
                _next = links["next"];    
            }
            else 
            {
                _next = null;
            }

            HasNext = _next != null;
        }
    }
}