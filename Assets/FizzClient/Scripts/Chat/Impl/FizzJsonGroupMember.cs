using Fizz.Common;
using Fizz.Common.Json;

namespace Fizz.Chat.Impl 
{
    public class FizzJsonGroupMember: IFizzGroupMember 
    {
        public static readonly string KEY_USER_ID = "user_id";
        public static readonly string KEY_GROUP_ID = "group_id";
        public static readonly string KEY_STATE = "state";
        public static readonly string KEY_ROLE = "role";

        public FizzJsonGroupMember(string json): this(JSONNode.Parse(json))
        {
        }

        public FizzJsonGroupMember(JSONNode json)
        {
            UserId = json[KEY_USER_ID];
            GroupId = json[KEY_GROUP_ID];
            State = FizzUtils.ParseState(json[KEY_STATE]);
            Role = FizzUtils.ParseRole(json[KEY_ROLE]);
        }

        public FizzJsonGroupMember(string userId, string groupId, string role, string state)
        {
            UserId = userId;
            GroupId = groupId;
            State = FizzUtils.ParseState(role);
            Role = FizzUtils.ParseRole(state);
        }

        public string UserId { get; private set; }
        public string GroupId { get; private set; }
        public FizzGroupMemberState State { get; private set; }
        public FizzGroupMemberRole Role { get; private set; }
    }
}