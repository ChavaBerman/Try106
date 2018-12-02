
using BOL.Models;
using BOL.Validations;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace BOL
{
    public class Worker
    {
        public Worker()
        {
            Projects = new List<Project>();
            PresentsDayWorker = new List<PresentDay>();
            workers = new List<Worker>();
        }
        [Key]
        public int WorkerId { get; set; }

        [Required(ErrorMessage = "name is required")]
        [MinLength(2, ErrorMessage = "name must be more than 2 chars"), MaxLength(15, ErrorMessage = "name must be less than 15 chars")]
        public string WorkerName { get; set; }

        [RequiredIf("IsNewWorker", true,
              ErrorMessageResourceName = "PasswordRequired")]
        [UniquePassword]
      [MinLength(64), MaxLength(64)]
        public string Password { get; set; }
        private string confirmPassword { set; get; }
        [Required(ErrorMessage = "email is required")]
        [UniqueEmail]
        [EmailAddress]
        public string Email { get; set; }
        
        
        //[MinLength(20),MaxLength(50)]
        [UniqueWorkerComputer]
        public string WorkerComputer { get; set; } = "";
        public decimal NumHoursWork { get; set; } = 9;

        public int? ManagerId { get; set; }
        public int StatusId { get; set; }
        [DefaultValue(true)]
        public bool IsNewWorker { get; set; }

        //------------------------------------------------------------
        public Status statusObj { get; set; }
         public List<Project> Projects { get; set; }
        public Worker Manager { get; set; }
        public List<PresentDay> PresentsDayWorker { get; set; }

        public List<Worker> workers { get; set; }

    }
}
