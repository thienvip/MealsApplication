using src.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repositories.Messages
{
    public interface IEmailTemplateRepository
    {
        Task<IList<EmailTemplate>> GetAllEmailTemplates();

        Task<EmailTemplate> GetEmailTemplateById(Guid id);

        Task<EmailTemplate> GetEmailTemplateByName(string name);

        Task<Guid> InsertEmailTemplate(EmailTemplate template);

        Task UpdateEmailTemplate(EmailTemplate template);
    }
}
