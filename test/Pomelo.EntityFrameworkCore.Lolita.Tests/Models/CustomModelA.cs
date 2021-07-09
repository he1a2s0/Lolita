
using System;

namespace Pomelo.EntityFrameworkCore.Lolita.Tests.Models
{
    public class CustomModelA : CustomModelBase
    {
        public string ModelAProperty { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
