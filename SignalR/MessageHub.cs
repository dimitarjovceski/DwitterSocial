using API.Extensions;
using AutoMapper;
using AutoMapper.Configuration.Annotations;
using DwitterSocial.Dtos;
using DwitterSocial.Entities;
using DwitterSocial.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace DwitterSocial.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHubContext<PresenceHub> hubContext;
        private readonly PresenceTracker presenceTracker;

        public MessageHub(IUnitOfWork unitOfWork,
            IMapper mapper,
            IHubContext<PresenceHub> hubContext, PresenceTracker presenceTracker)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.hubContext = hubContext;
            this.presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"].ToString();
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await unitOfWork.MessageRepository.GetMessageThread(Context.User.GetUsername(), otherUser);

            if (unitOfWork.HasChanges()) await unitOfWork.Complete();

            await Clients.Caller.SendAsync("ReceiveMessageThreads", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();

            if (username == createMessageDto.ReceiverUsername.ToLower())
                throw new HubException("You cannot send messages to yourself");

            var sender = await unitOfWork.UserRepository.GetByUsernameAsync(username);
            var receiver = await unitOfWork.UserRepository.GetByUsernameAsync(createMessageDto.ReceiverUsername);

            if (receiver == null) throw new HubException("User not found");

            var message = new Message
            {
                Sender = sender,
                Receiver = receiver,
                SenderUsername = sender.UserName,
                ReceiverUsername = receiver.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, receiver.UserName);
            var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);

            if (group.Connections.Any(x => x.Username == receiver.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await presenceTracker.GetConnectionsForUser(receiver.UserName);
                if(connections != null)
                {
                    await hubContext.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }

            unitOfWork.MessageRepository.AddMessage(message);  

            if(await unitOfWork.Complete())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDto>(message));
            }
        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await unitOfWork.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());
            
            if(group == null)
            {
                group = new Group(groupName);
                unitOfWork.MessageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if (await unitOfWork.Complete()) return group;

            throw new HubException("Failed to add to group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            unitOfWork.MessageRepository.RemoveConnection(connection);

            if (await unitOfWork.Complete()) return group;

            throw new HubException("Failed to remove from group");
        }

        private string GetGroupName(string caller, string other)
        {
            var stringComparer = string.CompareOrdinal(caller, other) < 0;
            return stringComparer ? $"{caller}-{other}" : $"{other}-{caller}";
        }
    }
}
