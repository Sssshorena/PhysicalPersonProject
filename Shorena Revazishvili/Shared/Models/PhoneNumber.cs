using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public enum PhoneType
    {
        Mobile = 1,
        Office,
        Home
    }
    public class PhoneNumber
    {
        public int Id { get; set; }
        public PhoneType Type { get; set; }

        
        [StringLength(50, MinimumLength = 4)]
        public string? Number { get; set; }

        public int PhysicalPersonId { get; set; } // Foreign Key
        public virtual PhysicalPerson? PhysicalPerson { get; set; } // კავშირი ფიზიკურ პირებთან
    }
}
