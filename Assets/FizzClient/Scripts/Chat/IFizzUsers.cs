using System;
using System.Collections.Generic;

namespace Fizz.Chat 
{
    public interface IFizzUserGroup 
    {
        string GroupId { get; }
        FizzGroupMemberRole Role { get; }
        FizzGroupMemberState State { get; }
        long? LastReadMessageId { get; }
        long Created { get; }
    }

    public interface IFizzFetchUserGroupsQuery
    {
        bool HasNext { get; }
        void Next(Action<IList<IFizzUserGroup>, FizzException> callback);
    }

    public interface IFizzUser
    {
        string Id { get; }
        string Nick { get; }
        string StatusMessage { get; }
        string ProfileUrl { get; }
        bool Online { get; }
    }

    public interface IFizzUsers 
    {
        void GetUser(string userId, Action<IFizzUser, FizzException> callback);
        void Subscribe(string userId, Action<FizzException> callback);
        void Unsubscribe(string userId, Action<FizzException> callback);
        IFizzFetchUserGroupsQuery BuildFetchUserGroupsQuery(string userId);
        void UpdateGroup(string userId, string groupId, FizzGroupMemberState? state, long? lastReadMessageId, Action<FizzException> callback);
        void RemoveGroup(string userId, string groupId, Action<FizzException> callback);
    }
}