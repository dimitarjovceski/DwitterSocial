namespace DwitterSocial.Interfaces
{
    public interface IUnitOfWork
    {
        ILikeRepository LikeRepository { get; }
        IUserRepository UserRepository { get; }
        IMessageRepository MessageRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
