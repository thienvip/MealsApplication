using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Core.Domains
{
    public class PowerOfAttorney
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Key]
        [ForeignKey("Mandator")]
        public int MandatorId { get; set; }
        public virtual Mandator Mandator { get; set; }
        [Key]
        public int AuthorizedPersonId { get; set; }
        public virtual AuthorizedPerson AuthorizedPerson { get; set; }

        public string StudentCode { get; set; }
        public string CampusCode { get; set; }
        public int ClassCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdated { get; set; }
        public int ParentId { get; set; }
        public int Status { get; set; }
    }
}
