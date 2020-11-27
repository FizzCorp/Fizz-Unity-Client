using Fizz.Common.Json;

namespace Fizz.Chat.Impl
{
    public class FizzJsonUser : IFizzUser
    {
        private static readonly string KEY_NICK = "nick";
        private static readonly string KEY_STATUS_MESSAGE = "status_message";
        private static readonly string KEY_PROFILE_URL = "profile_url";
        private static readonly string KEY_ONLINE = "is_online";

        public FizzJsonUser(string userId, string json) : this(userId, JSONNode.Parse(json))
        {
        }

        public FizzJsonUser(string userId, JSONNode json)
        {
            Id = userId;
            Nick = json[KEY_NICK];
            StatusMessage = json[KEY_STATUS_MESSAGE];
            ProfileUrl = json[KEY_PROFILE_URL];
            Online = json[KEY_ONLINE].AsBool;
        }

        public string Id { get; private set; }
        public string Nick { get; private set; }
        public string StatusMessage { get; private set; }
        public string ProfileUrl { get; private set; }
        public bool Online { get; private set; }
    }
}