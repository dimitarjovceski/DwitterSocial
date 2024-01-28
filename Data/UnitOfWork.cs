using AutoMapper;
using DwitterSocial.Interfaces;

namespace DwitterSocial.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;

        public UnitOfWork(AppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public ILikeRepository LikeRepository => new LikeRepository(context);

        public IUserRepository UserRepository => new UserRepository(context, mapper);

        public IMessageRepository MessageRepository => new MessageRepository(context, mapper);

        public async Task<bool> Complete()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();  
        }
    }
}
