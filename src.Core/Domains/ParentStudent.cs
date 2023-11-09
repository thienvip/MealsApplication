using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src.Core.Domains
{
    public class ParentStudent
    {
        public int ParentId { get; set; }
        public string StudentCode { get; set; }
        public DateTime ConfirmedAt { get; set; }
        public bool? IsSendSMS { get; set; }
        public bool? IsSendEmail { get; set; }
        public bool? IsPotentiallyLifeThreatening { get; set; }
        public string PotentiallyLifeThreateningContent { get; set; }
        public bool? IsMedicationOrReceivingTreatment { get; set; }
        public string MedicationOrReceivingTreatmentContent { get; set; }
    }
}
