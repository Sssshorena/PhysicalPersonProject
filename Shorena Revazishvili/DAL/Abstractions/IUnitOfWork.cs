namespace DAL.Abstractions
{
    public interface IUnitOfWork
    {
        IPhysicalPersonRepository physicalPersonRepo { get; }
        Task CommitAsync();
    }
}
