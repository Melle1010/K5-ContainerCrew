namespace AI_Content_Assistant.Services
{
    public interface IAiContentService
    {
        Task<string> CreateAsync(string userQuery, CancellationToken ct);

        //TEMP
        Task<string> ListModelsAsync(CancellationToken ct);

    }
}
