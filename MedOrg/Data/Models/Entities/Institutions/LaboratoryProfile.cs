using MedOrg.Data.Models.Entities.Auth;
using System.ComponentModel.DataAnnotations;

namespace MedOrg.Data.Models.Entities.Institutions
{
    public class LaboratoryProfile
    {
        [Key]
        public int Id { get; set; }

        public int LaboratoryId { get; set; }
        public Laboratory Laboratory { get; set; } = null!;

        public ProfileType ProfileType { get; set; }
    }
}