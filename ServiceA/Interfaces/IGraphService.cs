using ServiceA.Models;

namespace ServiceA.Interfaces
{
    public interface IGraphService
    {
        Task<Graph> CreateGraphAsync(Graph graph);
        Task<Graph?> GetGraphAsync(Guid id);
        Task UpdateGraphAsync(Guid id, Graph graph);
        Task DeleteGraphAsync(Guid id);
    }
}
