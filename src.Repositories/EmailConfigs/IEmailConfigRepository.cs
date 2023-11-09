
using src.Core.Domains;


namespace src.Repositories.EmailConfigs
{
    public interface IEmailConfigRepository
    {
        Task addEmaiConfig(EmailConfig model);
        Task<EmailConfig> getAllConfig();
        Task removeAllconfigs();
    }
}