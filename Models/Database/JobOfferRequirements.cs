using TWAB.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TWAB.Database
{
    public class JobOfferRequirements
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }

        // Klucz obcy do ofert
        public int JobOfferId { get; set; }
        public JobOfferModel JobOffer { get; set; }
    }
}