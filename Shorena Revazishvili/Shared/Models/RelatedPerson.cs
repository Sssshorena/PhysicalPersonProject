namespace Shared.Models
{
    public enum RelationType
    {
        Collegue = 1,
        Acquaintance,
        Family,
        Other
        
    }
    public class RelatedPerson
    {
        public int Id { get; set; }
        public RelationType RelationType { get; set; } //კავშირის ტიპი


        public int PersonId { get; set; }  // მთავარი პიროვნება
        public virtual PhysicalPerson? Person { get; set; } 


        public int RelatedPersonId { get; set; }  // დაკავშირებული პიროვნების Id
        public virtual PhysicalPerson? Related { get; set; } //დაკავშირებული პიროვნება

    }
}
