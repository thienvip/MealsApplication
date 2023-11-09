using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static src.Core.Constants;

namespace src.Core.Domains
{
    public class EmailConfig
    {
        [Key]
        public Guid configId { get; set; }

        [ForeignKey("confirmation")]
        public virtual Guid for_parent_confirmation { get; set; }

        [ForeignKey("confirmation_meal")]
        public virtual Guid for_parent_confirmation_meal { get; set; }

        [ForeignKey("validate")]
        public virtual Guid for_validate { get; set; }

        [ForeignKey("sms_validate")]
        public virtual Guid for_sms_validate { get; set; }

        [ForeignKey("confirmation_success")]
        public virtual Guid for_confirmation_success { get; set; }

        [ForeignKey("email_not_yet_confirmed")]
        public virtual Guid for_email_not_yet_confirmation { get; set; }


        [ForeignKey("sms_not_yet_confirmed")]
        public virtual Guid for_sms_not_yet_confirmation { get; set; }

        public virtual EmailTemplate validate { get; set; }
        public virtual EmailTemplate sms_validate { get; set; }
        public virtual EmailTemplate confirmation { get; set; }
        public virtual EmailTemplate confirmation_meal { get; set; }
        public virtual EmailTemplate confirmation_success { get; set; }
        public virtual EmailTemplate email_not_yet_confirmed { get; set; }
        public virtual EmailTemplate sms_not_yet_confirmed { get; set; }
    }
}
