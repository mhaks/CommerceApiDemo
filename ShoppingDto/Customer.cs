namespace CommerceApiDemo.ShoppingDto
{
    public class Customer
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Address1 { get; set; } 
        public string Address2 { get; set; } = String.Empty;    
        public required string City { get; set; }
        public required string State { get; set; }
        public required string Email { get; set; } 
        public required string PhoneNumber { get; set; } 
        public required string PostalCode { get; set; } 
        public required decimal TaxRate { get; set; }
    }
}
