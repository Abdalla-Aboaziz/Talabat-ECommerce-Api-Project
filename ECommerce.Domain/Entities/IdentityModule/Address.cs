namespace ECommerce.Domain.Entities.IdentityModule
{
    public class Address
    {
        public int Id { get; set; }
        public string City { get; set; } = default!;
        public string Street { get; set; } = default!;

        public string Country { get; set; } = default!;

        public string FistName { get; set; } = default!;
        public string LastName { get; set; } = default!;

        public ApplicationUser user { get; set; } = default!;
        public string UserId { get; set; } = default!; // fk [Unique Constraint ]
    }
}