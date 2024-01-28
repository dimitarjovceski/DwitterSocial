using AutoMapper;
using AutoMapper.QueryableExtensions;
using DwitterSocial.Dtos;
using DwitterSocial.Entities;
using DwitterSocial.Helpers;
using DwitterSocial.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DwitterSocial.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;

        public MessageRepository(AppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public void AddGroup(Group group)
        {
           context.Groups.Add(group);
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await context.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await context.Groups
                .Include(p => p.Connections)
                .Where(x => x.Connections.Any(s => s.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message> GetMessage(int id)
        {
            return await context.Messages
                .Include(r => r.Receiver)
                .Include(s => s.Sender)
                .SingleOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = context.Messages
                .OrderByDescending(x => x.MessageSent)
                .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.ReceiverUsername == messageParams.Username && u.ReceiverDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
                _ => query.Where(u => u.ReceiverUsername == messageParams.Username && u.ReceiverDeleted == false && u.DateRead == null)
            };

            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string receiverUsername)
        {
            var messages = await context.Messages
                .Where(m => m.Receiver.UserName == currentUsername && m.ReceiverDeleted == false
                && m.Sender.UserName == receiverUsername
                || m.Receiver.UserName == receiverUsername
                && m.Sender.UserName == currentUsername && m.SenderDeleted == false)
                .OrderBy(m => m.MessageSent)
                .ProjectTo<MessageDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null 
            && m.ReceiverUsername == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            return mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public void RemoveConnection(Connection connection)
        {
            context.Connections.Remove(connection); 
        }
    }
}
