
using Microsoft.EntityFrameworkCore;

namespace Pomelo.EntityFrameworkCore.Lolita.Tests.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public StreetAddress Address { get; set; }
    }

    [Owned]
    public class StreetAddress
    {
        public string Street { get; set; }

        public string City { get; set; }
    }
}
