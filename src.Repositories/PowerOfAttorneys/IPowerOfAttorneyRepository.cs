using src.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Repositories.PowerOfAttorneys
{
    public interface IPowerOfAttorneyRepository
    {
        Task<List<PowerOfAttorney>> GetAll();
        Task<PowerOfAttorney> GetById(int id);
        void Insert(PowerOfAttorney data);
        void Update(PowerOfAttorney data);
        Task<List<PowerOfAttorney>> GetByStudentCode(string studentCode);
        Task<List<PowerOfAttorney>> GetByMandatorId(int mandatorId);
    }
}
