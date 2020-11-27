using System;

namespace Fizz.Chat {
    public class FizzGroupUpdateEventData 
    {
        public enum UpdateReason 
        {
            Profile,
            Unknown
        }

        public FizzGroupUpdateEventData() 
        {
            Reason = UpdateReason.Unknown;
        }

        public UpdateReason Reason { get; set; }
        public string GroupId { get; set; }
        public string Title { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }

    public class FizzGroupMemberEventData {
        public FizzGroupMemberEventData() {
            State = FizzGroupMemberState.Unknown;
            Role = FizzGroupMemberRole.Unknown;
        }

        public string GroupId { get; set; }
        public string MemberId { get; set; }
        public FizzGroupMemberState State { get; set; }
        public FizzGroupMemberRole Role { get; set; }
    }

    public interface IFizzGroupListener
    {
        Action<FizzGroupUpdateEventData> OnGroupUpdated { get; set; }
        Action<FizzGroupMemberEventData> OnGroupMemberAdded { get; set; }
        Action<FizzGroupMemberEventData> OnGroupMemberRemoved { get; set; }
        Action<FizzGroupMemberEventData> OnGroupMemberUpdated { get; set; }
    }
}