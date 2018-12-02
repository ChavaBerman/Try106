using System.ComponentModel.DataAnnotations;

namespace Client_WinForm.Models
{
    public class Status
    {
        [Key]
        public int Id { get; set; }

        public string StatusName { get; set; }
    }
}
