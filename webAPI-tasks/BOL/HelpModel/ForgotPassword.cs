using BOL.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL.HelpModel
{
   public class ForgotPassword
    {
        [Required]
        public int RequestId { get; set; }

        [Required]
        public string WorkerName { get; set; }

        [MinLength(64), MaxLength(64)]
        [Required]
        public string Password { get; set; }
    }
}
