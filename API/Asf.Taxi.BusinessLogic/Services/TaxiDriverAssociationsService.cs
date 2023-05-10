using Asf.Taxi.BusinessLogic.Events;
using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.BusinessLogic.Mapper;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.DAL;
using Asf.Taxi.DAL.Entities;
using Asf.Taxi.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using SmartTech.Common;
using SmartTech.Infrastructure.DataAccessLayer.EFCore;
using SmartTech.Infrastructure.Exceptions;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Search;
using SmartTech.Infrastructure.SystemEvents;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Services
{
	class TaxiDriverAssociationsService : ITaxiDriverAssociationsService
	{
		readonly ITaxiUserContext _userContext;
		readonly TaxiDriverDBContext _dbContext;
		readonly ISystemEventManager _systemEventManager;

		public TaxiDriverAssociationsService(TaxiDriverDBContext dbContext, ITaxiUserContext userContext, ISystemEventManager systemEventManager)
		{
			_dbContext = dbContext;
			_userContext = userContext;
			_systemEventManager = systemEventManager;
		}

		public async Task<FilterResult<TaxiDriverAssociation>> SearchTaxiDriverAssociations(TaxiDriverAssociationFilterCriteria filterCriteria)
		{
			var sc = filterCriteria ?? new();
			var query = _dbContext.TaxiDriverAssociations.AsNoTracking().Include(x => x.Licensees)
				.Where(x => x.AuthorityId == _userContext.AuthorityId);

			sc.IsDeleted = false;

			query = sc.Id.HasValue ? query.Where(q => q.Id == sc.Id) : query;
			query = sc.IsDeleted.HasValue ? query.Where(q => q.IsDeleted == sc.IsDeleted) : query;
			query = sc.LicenseeId.HasValue ?
				query.Where(x => x.Licensees.Any(x => x.Id == sc.LicenseeId.Value)) : query;
			query = !string.IsNullOrWhiteSpace(sc.FiscalCode) ?
				query.Where(x => x.FiscalCode.Contains(sc.FiscalCode)) : query;
			query = !string.IsNullOrWhiteSpace(sc.Name) ?
				query.Where(x => x.Name.Contains(sc.Name)) : query;

			var result = await query.OrderAndPageAsync(sc.ToTypedCriteria<TaxiDriverAssociationEntity>());

			return result.MapFilterResult(m => m.Map());

		}

		public async Task<long> AddTaxiDriverAssociation(TaxiDriverAssociationWrite associationWrite)
		{
			associationWrite?.Validate();
			await CheckEntityAlreadyExist(associationWrite);

			var entity = associationWrite.Map(_userContext.AuthorityId);

			entity.AuthorityId = _userContext.AuthorityId;
			await _dbContext.AddAsync(entity);
			await _dbContext.SaveChangesAsync();
			await _systemEventManager.SendAsync(ApplicationsTopics.TaxiDriverEvents, new SystemEventBase
			{
				Payload = new TaxiDriverAuditEvent
				{
					AuthorityId = _userContext.AuthorityId,
					DisplayName = _userContext.DisplayName,
					TenantId = _userContext.TenantId,
					UserId = _userContext.SmartPAUserId!.Value,
					AuditPayload = new PayloadItem
					{
						Audit = new AuditWrite
						{
							ItemId = entity.Id,
							ItemType = ItemTypes.TaxiDriverAssociation,
							MemoLine = null,
							OperationType = OperationTypes.Creating
						}
					}
				}.JsonSerialize(false),
				Type = TaxiDriverEventTypes.TaxiDriverAudit
			});

			return entity.Id;
		}

		public async Task EditTaxiDriverAssociation(TaxiDriverAssociationWrite associationWrite, long id)
		{
			associationWrite?.Validate();

			var entity = await GetTaxiDriverAssociationEntityById(id) ??
						 throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);
			var oldEntity = PrepareTaxiDriverAssociationEntityToDeserialize(entity);

			await CheckEntityAlreadyExist(associationWrite, entity.Id);
			BaseCategoryUpdate(entity, associationWrite);
			_dbContext.Update(entity);
			await _dbContext.SaveChangesAsync();
			await _systemEventManager.SendAsync(ApplicationsTopics.TaxiDriverEvents, new SystemEventBase
			{
				Payload = new TaxiDriverAuditEvent
				{
					AuthorityId = _userContext.AuthorityId,
					DisplayName = _userContext.DisplayName,
					TenantId = _userContext.TenantId,
					UserId = _userContext.SmartPAUserId!.Value,
					AuditPayload = new PayloadItem
					{
						Audit = new AuditWrite
						{
							ItemId = id,
							ItemType = ItemTypes.TaxiDriverAssociation,
							MemoLine = null,
							OperationType = OperationTypes.Updating
						},
						Data = oldEntity
					}
				}.JsonSerialize(false),
				Type = TaxiDriverEventTypes.TaxiDriverAudit
			});
		}

		public async Task DeleteTaxiDriverAssociation(long id)
		{
			var association = await GetTaxiDriverAssociationEntityById(id) ??
					throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);

			if ((association.Licensees?.Count ?? 0) > 0)
				throw new BusinessLogicValidationException(string.Format(Errors.TaxiDriverAssociationHaveLicensees, association.Name));

			association.IsDeleted = true;
			_dbContext.Update(association);
			await _dbContext.SaveChangesAsync();
			await _systemEventManager.SendAsync(ApplicationsTopics.TaxiDriverEvents, new SystemEventBase
			{
				Payload = new TaxiDriverAuditEvent
				{
					AuthorityId = _userContext.AuthorityId,
					DisplayName = _userContext.DisplayName,
					TenantId = _userContext.TenantId,
					UserId = _userContext.SmartPAUserId!.Value,
					AuditPayload = new PayloadItem
					{
						Audit = new AuditWrite
						{
							ItemId = id,
							ItemType = ItemTypes.TaxiDriverAssociation,
							MemoLine = null,
							OperationType = OperationTypes.Deleting
						},
						Data = PrepareTaxiDriverAssociationEntityToDeserialize(association)
					}
				}.JsonSerialize(false),
				Type = TaxiDriverEventTypes.TaxiDriverAudit
			});
		}

		#region private methods

		static void BaseCategoryUpdate(TaxiDriverAssociationEntity entity, TaxiDriverAssociationWrite associationWrite)
		{
			entity.TelephoneNumber = associationWrite.TelephoneNumber;
			entity.Email = associationWrite.Email;
			entity.FiscalCode = associationWrite.FiscalCode;
			entity.Name = associationWrite.Name;
		}

		async Task<TaxiDriverAssociationEntity> GetTaxiDriverAssociationEntityById(long id) =>
			await _dbContext.TaxiDriverAssociations.Include(tda => tda.Licensees)
				.FirstOrDefaultAsync(tda => tda.AuthorityId == _userContext.AuthorityId && tda.Id == id);

		async Task CheckEntityAlreadyExist(TaxiDriverAssociationWrite taxiDriverAssociationWrite, long? id = null)
		{
			var sourceEntityId = id ?? 0;

			if (await _dbContext.TaxiDriverAssociations.AsNoTracking()
				.AnyAsync(x => x.AuthorityId == _userContext.AuthorityId && x.Id != sourceEntityId && x.FiscalCode == taxiDriverAssociationWrite.FiscalCode &&
								x.Name == taxiDriverAssociationWrite.Name && !x.IsDeleted))
				throw new BusinessLogicValidationException(Errors.TaxiDriverAssociationAlredyExist, $"{taxiDriverAssociationWrite.FiscalCode} - {taxiDriverAssociationWrite.Name}");
		}

		static TaxiDriverAssociationEntity PrepareTaxiDriverAssociationEntityToDeserialize(TaxiDriverAssociationEntity entity) =>
			new()
			{
				AuthorityId = entity.AuthorityId,
				Email = entity.Email,
				FiscalCode = entity.FiscalCode,
				Id = entity.Id,
				IsDeleted = entity.IsDeleted,
				Name = entity.Name,
				TelephoneNumber = entity.TelephoneNumber
			};

		#endregion
	}
}