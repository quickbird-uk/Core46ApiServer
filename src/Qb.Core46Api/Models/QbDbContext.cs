using Microsoft.EntityFrameworkCore;
using OpenIddict;

namespace Qb.Core46Api.Models
{
    public class QbDbContext : OpenIddictDbContext
    {
        public QbDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}