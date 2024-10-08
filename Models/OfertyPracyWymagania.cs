using System.ComponentModel.DataAnnotations.Schema;

namespace TWAB.Models
{
    public class OfertyPracyWymagania
    {
        public int Id { get; set; }
        public string Opis { get; set; }

        // Klucz obcy do ofert
        public int OfertaPracyId { get; set; }
        public OfertyPracyModel OfertaPracy { get; set; }
    }
}
