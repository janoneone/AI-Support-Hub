namespace AiSupportHub.Api.Services;

public interface ISupportFlowService
{
    Task<bool> IsSupportRequestAsync(string message);
}