using Asf.Taxi.BusinessLogic.Mapper;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.DAL;
using Asf.Taxi.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using SmartTech.Infrastructure.DataAccessLayer.EFCore;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Search;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Services
{
	class AuditsService : IAuditsService
	{
		readonly ITaxiUserContext _userContext;
		readonly TaxiDriverDBContext _dbContext;

		public AuditsService(ITaxiUserContext userContext, TaxiDriverDBContext dbContext)
		{
			_dbContext = dbContext;
			_userContext = userContext;
		}

		public async Task AddAudit(AuditWrite auditWrite)
		{
			auditWrite?.Validate();

			var entity = auditWrite.Map(_userContext.AuthorityId, _userContext.SmartPAUserId!.Value, _userContext.DisplayName);

			await _dbContext.Audits.AddAsync(entity);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<FilterResult<AuditInfo>> SearchAudits(AuditsFilterCriteria filter)
		{
			var sc = filter ?? new AuditsFilterCriteria();
			var query = _dbContext.Audits.AsNoTracking().Where(x => x.AuthorityId == _userContext.AuthorityId);

			query = sc.ItemId.HasValue ? query.Where(x => x.ItemId == sc.ItemId) : query;
			query = sc.VariationDateFrom.HasValue ? query.Where(q => q.CreatedAt >= filter.VariationDateFrom) : query;
			query = sc.VariationDateTo.HasValue ? query.Where(q => q.CreatedAt <= filter.VariationDateTo) : query;
			query = sc.ItemType.HasValue ? query.Where(q => q.ItemType == sc.ItemType) : query;
			query = sc.OperationType.HasValue ? query.Where(q => q.OperationType == sc.OperationType) : query;

			return (await query.OrderAndPageAsync(filter.ToTypedCriteria<AuditEntity>()))
						.MapFilterResult(m => m.Map());
		}
	}
}