using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.Data;
using System.Security.Principal;

namespace NZWalk.Api.Data
{

        //    IdentityDbContext:
        //A built-in class in ASP.NET Core Identity that provides tables for:
        //Users(AspNetUsers)
        //Roles(AspNetRoles)
        //User-Role relationships(AspNetUserRoles)
        //The DbContextOptions<NZWalksAuthDbContext> is injected via dependency injection so it can connect to the database.
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
                     ConcurrencyStamp=writerRoleID,  //	Used for optimistic concurrency control (ensures no conflicts when multiple users modify a role).
                     Name = "Writer",
                        NormalizedName = "WRITER".ToUpper()
                }
            };
             builder.Entity<IdentityRole>().HasData(roles);

//            Seeds roles using HasData(roles):
//This ensures the roles exist when the database is first created.
        }
    }
}
