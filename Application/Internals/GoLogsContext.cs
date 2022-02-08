using GoLogs.Api.BusinessLogic.Interfaces;
using GoLogs.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace GoLogs.Api.Application.Internals
{
    public class GoLogsContext : DbContext
    {
        private readonly HttpContext _httpContext;

        public GoLogsContext(DbContextOptions<GoLogsContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public DbSet<PersonModel> Persons { get; set; }
        public DbSet<EmailTemplateModel> EmailTemplates { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }
        public DbSet<DeliveryOrderModel> DeliveryOrders { get; set; }
        public DbSet<DeliveryOrderContainerModel> DeliveryOrderContainers { get; set; }
        public DbSet<NotifyTemplateModel> NotifyTemplates { get; set; }
        public DbSet<NotifyModel> Notifies { get; set; }

        public override Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            //var AddedEntities = ChangeTracker.Entries()
            //    .Where(E => E.State == EntityState.Added)
            //    .ToList();

            //AddedEntities.ForEach(E =>
            //{
            //    E.Property("CreationTime").CurrentValue = DateTime.Now;
            //});

            //var EditedEntities = ChangeTracker.Entries()
            //    .Where(E => E.State == EntityState.Modified)
            //    .ToList();

            //EditedEntities.ForEach(E =>
            //{
            //    E.Property("ModifiedDate").CurrentValue = DateTime.Now;
            //});

            // Cast to ClaimsIdentity.
            ClaimsIdentity identity = _httpContext.User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claim = identity.Claims;
            var claimId = claim
                .Where(x => x.Type == ClaimTypes.NameIdentifier)
                .FirstOrDefault();
            var claimEmail = claim
                .Where(x => x.Type == ClaimTypes.Email)
                .FirstOrDefault();

            Guid personId = claimId == null ? Guid.Empty : Guid.Parse(claimId.Value);
            var personEmail = claimEmail == null ? "SYSTEM" : claimEmail.Value;

            var entries = ChangeTracker.Entries();
            foreach (var e in entries)
            {
                if (e.Entity is IBaseModel)
                {
                    var entity = (IBaseModel)e.Entity;
                    switch (e.State)
                    {
                        case EntityState.Added:
                            entity.RowStatus = true;
                            entity.CreatedById = personId;
                            entity.CreatedBy = personEmail;
                            entity.CreatedDate = entity.CreatedDate != DateTime.MinValue ? entity.CreatedDate : DateTime.Now;
                            entity.ModifiedById = personId;
                            entity.ModifiedBy = personEmail;
                            entity.ModifiedDate = entity.ModifiedDate != DateTime.MinValue ? entity.ModifiedDate : DateTime.Now;
                            break;
                        case EntityState.Modified:
                            entity.ModifiedById = personId;
                            entity.ModifiedBy = personEmail;
                            entity.ModifiedDate = entity.ModifiedDate != DateTime.MinValue ? entity.ModifiedDate : DateTime.Now;
                            break;
                    }
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

    }
}
