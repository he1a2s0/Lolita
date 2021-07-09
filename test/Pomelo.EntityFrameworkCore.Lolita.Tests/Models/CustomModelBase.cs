
namespace Pomelo.EntityFrameworkCore.Lolita.Tests.Models
{
    public abstract class CustomModelBase : ICustomModelInterface
    {
        public long Id { get; set; }

        public string StringProperty { get; set; }

        public int IntProperty { get; set; }
    }
}
