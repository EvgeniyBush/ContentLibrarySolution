#pragma warning disable 

using DAL2.Entities;

namespace DAL2.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IGenericRepository<Content> Contents { get; }
        public IGenericRepository<Storage> Storages { get; }
        public IGenericRepository<ContentLocation> ContentLocations { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Contents = new GenericRepository<Content>(_context);
            Storages = new GenericRepository<Storage>(_context);
            ContentLocations = new GenericRepository<ContentLocation>(_context);
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
