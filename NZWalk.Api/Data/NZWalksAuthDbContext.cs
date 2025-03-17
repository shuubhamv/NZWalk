using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NZWalk.Api.Data
{
    public class NZWalksAuthDbContext : IdentityDbContext
    {
        public NZWalksAuthDbContext(DbContextOptions<NZWalksAuthDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var readerRoleID = "5861790e-ce60-4613-ace6-e365640b8910";
            var writerRoleID = "d63d5e7b-8042-4f56-823d-be1cdfe9af8d";

            var roles= new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerRoleID,
                    ConcurrencyStamp=readerRoleID,
                    Name = "Reader",
                    NormalizedName = "READER".ToUpper()

                },
                new IdentityRole
                {
                   Id = writerRoleID,
                     ConcurrencyStamp=writerRoleID,
                     Name = "Writer",
                        NormalizedName = "WRITER".ToUpper()
                }
            };
             builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
