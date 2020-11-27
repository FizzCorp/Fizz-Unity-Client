using Fizz.Common;
using Fizz.Common.Json;

namespace Fizz.Chat.Impl 
{
    public class FizzJsonUserGroup: IFizzUserGroup 
    {
        public static readonly string KEY_GROUP_ID = "group_id";
        public static readonly string KEY_STATE = "state";
        public static readonly string KEY_ROLE = "role";
        public static readonly string KEY_LAST_READ_MESSAGE_ID = "last_read_message_id";
        public static readonly string KEY_CREATED = "created";

        public FizzJsonUserGroup(string json): this(JSONNode.Parse(json))
        {
        }

        public FizzJsonUserGroup(JSONNode json)
        {
            GroupId = json[KEY_GROUP_ID];
            State = FizzUtils.ParseState(json[KEY_STATE]);
            Role = FizzUtils.ParseRole(json[KEY_ROLE]);
            if (json[KEY_LAST_READ_MESSAGE_ID] != null)
            {
                LastReadMessageId = (long)json[KEY_LAST_READ_MESSAGE_ID].AsDouble;
            }
            Created = (long)json[KEY_CREATED].AsDouble;
        }

        public string GroupId { get; private set; }
        public FizzGroupMemberRole Role { get; private set; }
        public FizzGroupMemberState State { get; private set; }
        public long? LastReadMessageId { get; private set; }
        public long Created { get; private set; }
    }
}