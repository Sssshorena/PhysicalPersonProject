using DAL.Abstractions;
using Project.Infrastructure;


namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PhysicalPersonsDbContext _dbContext;
        public IPhysicalPersonRepository physicalPersonRepo { get; }

        public UnitOfWork(PhysicalPersonsDbContext dbContext, IPhysicalPersonRepository repository)
        {
            _dbContext = dbContext;
            physicalPersonRepo = repository;
        }


        public async Task CommitAsync() 
        {
            await _dbContext.SaveChangesAsync();
        }

      
    }
        
}

