using System;
using System.Collections.Generic;

namespace Fizz.Chat 
{
    public enum FizzGroupMemberState 
    {
        Pending,
        Joined,
        Unknown
    }

    public enum FizzGroupMemberRole 
    {
        Moderator,
        Member,
        Unknown
    }

    public interface IFizzGroupMember 
    {
        string UserId { get; }
        string GroupId { get; }
        FizzGroupMemberState State { get; }
        FizzGroupMemberRole Role { get; }
    }
    
    public interface IFizzGroup
    {
        string Id { get; }
        string ChannelId { get; }
        string CreatedBy { get; }
        string Title { get; }
        string ImageURL { get; }
        string Description { get; }
        string Type { get; }
        long Created { get; }
        long Updated { get; }
    }

    public interface IFizzGroups 
    {
        void FetchGroup(
            string groupId, 
            Action<IFizzGroup, FizzException> callback
        );

        void GetGroupMembers(
            string groupId, 
            Action<IList<IFizzGroupMember>, FizzException> callback
        );

        void PublishMessage(
            string groupId,
            string nick, 
            string body, 
            Dictionary<string, string> data, 
            bool translate, 
            bool filter, 
            bool persist, 
            Action<FizzException> callback
        );

        void QueryLatest(
            string channelId, 
            int count, 
            Action<IList<FizzChannelMessage>, FizzException> callback
        );

        void QueryLatest(
            string channelId, 
            int count, 
            long beforeId, 
            Action<IList<FizzChannelMessage>, FizzException> callback
        );
    }
}