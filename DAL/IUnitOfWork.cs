#pragma warning disable 

using DAL2.Entities;

namespace DAL2.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Content> Contents { get; }
        IGenericRepository<Storage> Storages { get; }
        IGenericRepository<ContentLocation> ContentLocations { get; }

        Task<int> CompleteAsync();
    }
}
