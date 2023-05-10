using Asf.Taxi.BusinessLogic.Events;
using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.BusinessLogic.Mapper;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Models.Licensees;
using Asf.Taxi.BusinessLogic.Models.LicenseesIssuingOffice;
using Asf.Taxi.DAL;
using Asf.Taxi.DAL.Entities;
using Asf.Taxi.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartTech.Common;
using SmartTech.Common.Extensions;
using SmartTech.Common.Models;
using SmartTech.Common.Services;
using SmartTech.Infrastructure.DataAccessLayer.EFCore;
using SmartTech.Infrastructure.Exceptions;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Search;
using SmartTech.Infrastructure.SystemEvents;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Services
{
    class LicenseesService : ILicenseesService
    {
        readonly ITaxiUserContext _userContext;
        readonly TaxiDriverDBContext _dbContext;
        readonly ITaxiDriversService _driverService;
        readonly SmartPAClientData _smartPAClientData;
        readonly IHttpClientFactory _httpClientFactory;
        readonly ISystemEventManager _systemEventManager;
        readonly IOrganizationalChart _organizationalChartService;

        public LicenseesService(TaxiDriverDBContext dbContext, ITaxiUserContext userContext, ISystemEventManager systemEventManager,
            ITaxiDriversService driverService, IOrganizationalChart organizationalChartService, IHttpClientFactory httpClientFactory,
            IOptions<SmartPAClientData> smartPAClientData)
        {
            _dbContext = dbContext;
            _userContext = userContext;
            _systemEventManager = systemEventManager;
            _driverService = driverService;
            _organizationalChartService = organizationalChartService;
            _httpClientFactory = httpClientFactory;
            _smartPAClientData = smartPAClientData.Value;
        }

        async Task<List<string?>> ILicenseesService.GetRecipientsAutocomplete(long licenseId, string mail)
        {
            var query = _dbContext.Recipients.Where(r => r.AuthorityId == _userContext.AuthorityId && r.LicenseeId == licenseId)
                                    .OrderBy(r => r.Order)
                                    .AsNoTracking();

            query = !string.IsNullOrWhiteSpace(mail) ? query.Where(r => r.Mail!.Contains(mail)) : query;

            return (await query.Select(r => r.Mail).ToListAsync()).Distinct().ToList();
        }

        async Task<List<string?>> ILicenseesService.GetRecipients(long licenseId)
        {
            var query = _dbContext.Recipients
                                    .Where(r => r.AuthorityId == _userContext.AuthorityId && r.LicenseeId == licenseId)
                                    .OrderBy(r => r.Order)
                                    .AsNoTracking();

            return (await query.Select(r => r.Mail).ToListAsync()).Distinct().ToList();
        }

        async Task<FilterResult<T>> ILicenseesService.SearchLicensees<T>(LicenseesFilterCriteria filterCriteria)
        {
            var sc = filterCriteria ?? new LicenseesFilterCriteria
            {
                ItemsPerPage = 50,
                PageNumber = 1
            };
            var query = _dbContext.Licensees
                .Include(l => l.Shift)
                .Include(l => l.SubShift)
                .Include(l => l.TaxiDriverAssociation)
                .Include(l => l.Vehicle)
                .Include(l => l.LicenseesIssuingOffice)
                .Include(l => l.LicenseesTaxiDrivers)
                    .ThenInclude(a => a.TaxiDriver)
                .Include(l => l.TaxiDriverSubstitutions)
                .AsNoTracking()
                .Where(l => l.AuthorityId == _userContext.AuthorityId);

            query = sc.EndFrom.HasValue ?
                query.Where(q => q.EndDate >= sc.EndFrom) : query;
            query = sc.EndTo.HasValue ?
                query.Where(q => q.EndDate <= sc.EndTo) : query;
            query = sc.ReleaseDateFrom.HasValue ?
                query.Where(q => q.ReleaseDate >= sc.ReleaseDateFrom) : query;
            query = sc.ReleaseDateTo.HasValue ?
                query.Where(q => q.ReleaseDate <= sc.ReleaseDateTo) : query;
            query = sc.Id.HasValue ? query.Where(q => q.Id == sc.Id) : query;
            query = (sc.Ids?.Count ?? 0) > 0 ? query.Where(q => sc.Ids.Contains(q.Id)) : query;
            query = !string.IsNullOrWhiteSpace(sc.Number) ?
                query.Where(q => q.Number.Contains(sc.Number)) : query;
            query = sc.Status.HasValue ? query.Where(q => q.Status == sc.Status) : query;
            query = sc.Type.HasValue ? query.Where(q => q.Type == sc.Type) : query;
            if (sc.IsSubstitution.HasValue)
            {
                if (sc.IsSubstitution.Value)
                {
                    query = query.Where(q => q.TaxiDriverSubstitutions.Any());
                    if (sc.HasActiveSubstitution.HasValue)
                    {
                        if (sc.HasActiveSubstitution.Value)
                            query = query.Where(q => q.TaxiDriverSubstitutions.Any(a => a.Status == SubstitutionStatus.Active));
                        else
                            query = query.Where(q => q.TaxiDriverSubstitutions
                                                        .OrderByDescending(a => a.EndDate)
                                                        .FirstOrDefault().Status != SubstitutionStatus.Active);
                    }
                }
                else
                    query = query.Where(q => !q.TaxiDriverSubstitutions.Any());
            }
            query = sc.IsFinancialAdministration.HasValue ? query.Where(q => q.LicenseesTaxiDrivers.Any(ltd => ltd.IsFinancialAdministration)) : query;
            query = sc.IsFamilyCollaboration.HasValue ? query.Where(q => q.LicenseesTaxiDrivers.Any(ltd => ltd.TaxiDriverType == DriverTypes.FamilyCollaborator)) : query;
            query = !string.IsNullOrWhiteSpace(sc.VehiclePlate) ?
                query.Where(q => q.Vehicle.LicensePlate == sc.VehiclePlate) : query;
            query = sc.CarFuelType.HasValue ?
                query.Where(q => q.Vehicle.CarFuelType.Equals(sc.CarFuelType)) : query;
            query = sc.TaxiDriverAssociationId.HasValue ? query.Where(q => q.TaxiDriverAssociationId == sc.TaxiDriverAssociationId) : query;
            query = sc.ShiftId.HasValue ? query.Where(q => q.SubShift.ShiftId == sc.ShiftId) : query;
            query = sc.SubShiftId.HasValue ? query.Where(q => q.SubShiftId == sc.SubShiftId) : query;
            query = !string.IsNullOrWhiteSpace(sc.Note) ? query.Where(q => q.Note.Contains(sc.Note)) : query;
            query = !string.IsNullOrWhiteSpace(sc.ShiftName) ? query.Where(q => q.Shift.Name.Contains(sc.ShiftName)) : query;
            query = !string.IsNullOrWhiteSpace(sc.SubShiftName) ? query.Where(q => q.SubShift.Name.Contains(sc.SubShiftName)) : query;
            query = !string.IsNullOrWhiteSpace(sc.GarageAddress) ? query.Where(q => q.GarageAddress.Contains(sc.GarageAddress)) : query;
            query = !string.IsNullOrWhiteSpace(sc.TaxiDriverLastName) ? query.Where(q => q.LicenseesTaxiDrivers.Any(ltd => ltd.TaxiDriverType == DriverTypes.Master && ltd.TaxiDriver.LastName.StartsWith(sc.TaxiDriverLastName))) : query;
            query = !string.IsNullOrWhiteSpace(sc.Acronym) ? query.Where(q => q.Acronym.Contains(sc.Acronym)) : query;

            var result = await query.OrderAndPageAsync(sc.ToTypedCriteria(sc.MapSortCriteria()));

            return result.MapFilterResult(m => m.Map<T>(_driverService.CheckAllDocumentsExists(m.Id,
                                               m?.LicenseesTaxiDrivers?.FirstOrDefault(a => a.TaxiDriverType == DriverTypes.Master)?.TaxiDriverId ?? 0).Result,
                                               _driverService.CheckAllDocumentsExists(m.Id,
                                               m?.LicenseesTaxiDrivers?.FirstOrDefault(a => a.TaxiDriverType == DriverTypes.FamilyCollaborator)?.TaxiDriverId ?? 0).Result));
        }

        async Task<long> ILicenseesService.AddLicensee<T>(T licenseeWrite)
        {
            licenseeWrite?.Validate();
            await CheckEntityAlreadyExist(licenseeWrite, null);

            if (licenseeWrite.Status != LicenseeStatus.Created && licenseeWrite.Status != LicenseeStatus.Released)
                throw new BusinessLogicValidationException(string.Format(Errors.InvalidInitialState, LicenseeStatus.Created.GetDescription<EnumDescriptions>(), LicenseeStatus.Released.GetDescription<EnumDescriptions>()));

            var licensee = licenseeWrite.Map(_userContext.AuthorityId);

            if (licenseeWrite is LicenseeNCCWrite ncc)
            {
                if (ncc.IsFamilyCollaboration!.Value && ncc.IsFinancialAdministration!.Value)
                    throw new BusinessLogicValidationException("Inserire solo uno tra Collaborazione familiare e Gestione Economica");
            }

            if (licenseeWrite is LicenseeTaxiWrite taxi)
            {
                if ((taxi.SubShiftId.HasValue || taxi.ShiftId.HasValue) && !await _dbContext.SubShifts.AnyAsync(ss => ss.AuthorityId == _userContext.AuthorityId &&
                                                            ss.Id == taxi.SubShiftId && ss.ShiftId == taxi.ShiftId))
                    throw new BusinessLogicValidationException(Errors.InvalidSubShift);

            }

            await _dbContext.Licensees.AddAsync(licensee);
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
                            ItemId = licensee.Id,
                            ItemType = ItemTypes.Licensee,
                            MemoLine = null,
                            OperationType = OperationTypes.Creating
                        }
                    }
                }.JsonSerialize(false),
                Type = TaxiDriverEventTypes.TaxiDriverAudit
            });

            var accessToken = await _httpClientFactory.GetClientApplicationToken(_smartPAClientData, _userContext.AuthorityId.ToString(), _userContext.TenantId);
            var rolePath = (await _organizationalChartService.SearchLevels(_userContext.AuthorityId, new LevelSearchCriteria
            {
                UserId = _userContext.SmartPAUserId
            }, accessToken)).FirstOrDefault()?.RolePath;

            await _systemEventManager.SendAsync(ApplicationsTopics.TaxiDriverEvents, new SystemEventBase
            {
                Payload = new TaxiDriverFolderCreateEvent
                {
                    AuthorityId = _userContext.AuthorityId,
                    TenantId = _userContext.TenantId,
                    UserId = _userContext.SmartPAUserId!.Value,
                    LicenseeId = licensee.Id,
                    LicenseeNumber = licensee.Number,
                    LicenseeType = licensee.Type,
                    RolePath = rolePath
                }.JsonSerialize(false),
                Type = TaxiDriverEventTypes.CreateFolder
            });

            return licensee.Id;
        }

        async Task ILicenseesService.UpdateLicensee<T>(long id, T licenseeWrite)
        {
            licenseeWrite?.Validate();

            var isVariation = licenseeWrite is LicenseeNCCVariation || licenseeWrite is LicenseeTaxiVariation;
            var taxiDriverAssociationId = licenseeWrite.TaxiDriverAssociationId;

            if (taxiDriverAssociationId.HasValue && !await _dbContext.TaxiDriverAssociations.AsNoTracking()
                        .AnyAsync(tda => tda.AuthorityId == _userContext.AuthorityId && tda.Id == taxiDriverAssociationId && !tda.IsDeleted))
                throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound, Errors.TaxiDriverAssociationNotFound);

            if (licenseeWrite is LicenseeTaxiWrite taxi)
            {
                if ((taxi.SubShiftId.HasValue || taxi.ShiftId.HasValue) && !await _dbContext.SubShifts.AnyAsync(ss => ss.AuthorityId == _userContext.AuthorityId &&
                                                            ss.Id == taxi.SubShiftId && ss.ShiftId == taxi.ShiftId))
                    throw new BusinessLogicValidationException(Errors.InvalidSubShift);
            }

            await CheckEntityAlreadyExist(licenseeWrite, id);

            var licenseeEntity = await _dbContext.Licensees
                                        .Include(x => x.SubShift)
                                        .Include(l => l.TaxiDriverAssociation)
                                        .Include(l => l.Vehicle)
                                        .Include(l => l.LicenseesIssuingOffice)
                                        .Include(l => l.FinancialAdministration)
                                        .FirstOrDefaultAsync(a => a.Id == id && a.AuthorityId == _userContext.AuthorityId) ??
                                 throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);

            if (licenseeEntity.Status == LicenseeStatus.Terminated)
                throw new BusinessLogicValidationException(Errors.LicenseeModifyStateError);

            var newState = CheckAndGetNewState(licenseeEntity.Status, licenseeWrite.Status.Value);

            bool subShifNull = false;

            if (licenseeWrite is LicenseeTaxiWrite)
                subShifNull = (licenseeWrite as LicenseeTaxiWrite).SubShiftId == null;
            if (licenseeWrite is LicenseeTaxiVariation)
                subShifNull = (licenseeWrite as LicenseeTaxiVariation).SubShiftId == null;

            if (newState == LicenseeStatus.Released && subShifNull && licenseeEntity.Type == LicenseeTypes.Taxi)
                throw new BusinessLogicValidationException(Errors.ReleaseLicenseeShiftError);

            if (isVariation)
            {
                var licenseeHistory = licenseeEntity.Map((licenseeWrite is LicenseeTaxiVariation taxivar) ? taxivar.VariationNote
                                    : (licenseeWrite is LicenseeNCCVariation nccvar) ? nccvar.VariationNote : string.Empty,
                                    DateTime.UtcNow);

                await _dbContext.AddAsync(licenseeHistory);

                licenseeEntity.SysStartTime = DateTime.UtcNow;
            }
            else
            {
                if (licenseeWrite.Status == LicenseeStatus.Terminated)
                    throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.Validation, "Non è possibile terminare una licenza in modifica");
            }

            var oldLicensee = GetLicenseeEntityToSerialize(licenseeEntity);


            licenseeEntity.Map(licenseeWrite);

            _dbContext.Licensees.Update(licenseeEntity);

            var toUpdate = await _dbContext.LicenseesTaxiDrivers
                                    .Where(a => a.AuthorityId == _userContext.AuthorityId && a.LicenseeId == id).ToListAsync();

            if (toUpdate != null && licenseeEntity.Status != LicenseeStatus.Terminated)
            {
                foreach (var item in toUpdate)
                {
                    item.IsFinancialAdministration = licenseeEntity.IsFinancialAdministration;
                    item.LicenseeStatus = licenseeEntity.Status;
                    item.LicenseeType = licenseeEntity.Type;
                    _dbContext.LicenseesTaxiDrivers.Update(item);
                }
            }
            else if (toUpdate != null && licenseeEntity.Status == LicenseeStatus.Terminated)
            {
                _dbContext.LicenseesTaxiDrivers.RemoveRange(toUpdate);
            }

            if (oldLicensee.IsFamilyCollaboration && !licenseeWrite.IsFamilyCollaboration!.Value)
            {
                var coll = await _dbContext.LicenseesTaxiDrivers
                    .Where(a => a.AuthorityId == _userContext.AuthorityId && a.LicenseeId == id && a.TaxiDriverType == DriverTypes.FamilyCollaborator)
                    .ToListAsync();
                _dbContext.LicenseesTaxiDrivers.RemoveRange(coll);
            }
            if (licenseeEntity.Type == LicenseeTypes.NCC_Auto && oldLicensee.IsFinancialAdministration)
            {
                if (licenseeWrite is LicenseeNCCWrite nccwrite && !nccwrite.IsFinancialAdministration!.Value)
                {
                    var coll = await _dbContext.LicenseesTaxiDrivers
                        .Where(a => a.AuthorityId == _userContext.AuthorityId && a.LicenseeId == id && a.TaxiDriverType == DriverTypes.FinancialAdministration)
                        .ToListAsync();
                    _dbContext.LicenseesTaxiDrivers.RemoveRange(coll);

                    var economic = await _dbContext.FinancialAdministrations
                        .Where(f => f.AuthorityId == _userContext.AuthorityId && f.LicenseeId == id && !f.Deleted)
                        .ToListAsync();
                    _dbContext.FinancialAdministrations.RemoveRange(economic);
                }
            }

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
                            ItemId = licenseeEntity.Id,
                            ItemType = ItemTypes.Licensee,
                            MemoLine = isVariation ? (licenseeWrite is LicenseeTaxiVariation taximemo) ? taximemo.VariationNote
                                    : (licenseeWrite is LicenseeNCCVariation nccmemo) ? nccmemo.VariationNote : string.Empty : default,
                            OperationType = isVariation ? OperationTypes.Changing : OperationTypes.Updating
                        },
                        Data = oldLicensee
                    }
                }.JsonSerialize(false),
                Type = TaxiDriverEventTypes.TaxiDriverAudit
            });
        }

        async Task ILicenseesService.RenewLicensee(long licenseeId)
        {
            var licenseeEntity = await _dbContext.Licensees
                                        .FirstOrDefaultAsync(a => a.Id == licenseeId && a.AuthorityId == _userContext.AuthorityId) ??
                                 throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);

            if (licenseeEntity.Status == LicenseeStatus.Terminated)
                throw new BusinessLogicValidationException(Errors.LicenseeModifyStateError);

            licenseeEntity.EndDate = licenseeEntity.EndDate.Year < 9994 ? licenseeEntity.EndDate.AddYears(5) : licenseeEntity.EndDate;
            _dbContext.Update(licenseeEntity);
            await _dbContext.SaveChangesAsync();
        }

        async Task<FilterResult<LicenseeHistory>> ILicenseesService.SearchLicenseeVariations(LicenseeVariationFilterCriteria filterCriteria)
        {
            var sc = filterCriteria ?? new LicenseeVariationFilterCriteria();

            if (string.IsNullOrWhiteSpace(sc.KeySelector))
            {
                sc.KeySelector = nameof(LicenseeHistory.StartDate);
                sc.Ascending = false;
            }

            var query = _dbContext.LicenseesHistory
                            .AsNoTracking()
                            .Where(l => l.AuthorityId == _userContext.AuthorityId && l.VariationType == VariationTypes.LicenseesVariation);

            query = sc.VariationDateFrom.HasValue ?
            query.Where(q => q.SysStartTime >= sc.VariationDateFrom) : query;
            query = sc.VariationDateTo.HasValue ?
                query.Where(q => q.SysStartTime <= sc.VariationDateTo) : query;
            query = sc.Id.HasValue ? query.Where(q => q.LicenseeId == sc.Id) : query;
            query = (sc.Ids?.Count ?? 0) > 0 ? query.Where(q => sc.Ids.Contains(q.LicenseeId)) : query;
            query = !string.IsNullOrWhiteSpace(sc.VariationNote) ? query.Where(q => sc.VariationNote.Contains(q.Note)) : query;
            query = !string.IsNullOrWhiteSpace(sc.Note) ? query.Where(q => sc.Note.Contains(q.LicenseeNote)) : query;
            query = sc.EndFrom.HasValue ?
            query.Where(q => q.EndDate >= sc.EndFrom) : query;
            query = sc.EndTo.HasValue ?
               query.Where(q => q.EndDate <= sc.EndTo) : query;
            query = !string.IsNullOrWhiteSpace(sc.Number) ? query.Where(q => sc.Number.Contains(q.Number)) : query;
            query = sc.ReleaseDateFrom.HasValue ?
            query.Where(q => q.ReleaseDate >= sc.ReleaseDateFrom) : query;
            query = sc.ReleaseDateTo.HasValue ?
               query.Where(q => q.ReleaseDate <= sc.ReleaseDateTo) : query;
            query = sc.Status.HasValue ? query.Where(q => q.Status == sc.Status) : query;
            query = sc.TaxiDriverAssociationId.HasValue ? query.Where(q => q.TaxiDriverAssociationId == sc.TaxiDriverAssociationId) : query;
            query = sc.Type.HasValue ? query.Where(q => q.Type == sc.Type) : query;

            var subShifts = await _dbContext.SubShifts
                .AsNoTracking()
                .Where(s => s.AuthorityId == _userContext.AuthorityId && query.Select(s => s.SubShiftId).Contains(s.Id))
                .ToDictionaryAsync(k => k.Id, v => v.Name!);

            var shifts = await _dbContext.Shifts
                .AsNoTracking()
                .Where(s => s.AuthorityId == _userContext.AuthorityId && query.Select(s => s.ShiftId).Contains(s.Id))
                .ToDictionaryAsync(k => k.Id, v => v.Name!);

            return (await query.OrderAndPageAsync(sc.ToTypedCriteria(sc.MapSortCriteria()))).MapFilterResult(m => m.Map(subShifts, shifts));
        }

        #region LicenseesIssuingOffice
        async Task<FilterResult<LicenseesIssuingOffice>> ILicenseesService.SearchLicenseesIssuingOffices(LicenseesIssuingOfficesFilterCriteria filterCriteria)
        {
            var sc = filterCriteria ?? new LicenseesIssuingOfficesFilterCriteria();

            var query = _dbContext.LicenseesIssuingOffices
                            .AsNoTracking()
                            .Where(l => l.AuthorityId == _userContext.AuthorityId && !l.Deleted);

            query = sc.Id.HasValue ?
            query.Where(q => q.Id == sc.Id) : query;
            query = (sc.Ids?.Count ?? 0) > 0 ? query.Where(q => sc.Ids.Contains(q.Id)) : query;
            query = !string.IsNullOrWhiteSpace(sc.Description) ? query.Where(q => sc.Description.Contains(q.Description)) : query;

            return (await query.OrderAndPageAsync(sc.ToTypedCriteria(sc.MapSortCriteria()))).MapFilterResult(m => m.Map());
        }

        async Task<long> ILicenseesService.AddLicenseesIssuingOffice(LicenseesIssuingOfficeWrite licenseeIssuingOfficeWrite)
        {
            licenseeIssuingOfficeWrite?.Validate();

            if (await _dbContext.LicenseesIssuingOffices.AsNoTracking()
                .AnyAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Description == licenseeIssuingOfficeWrite.Description && !l.Deleted))
                throw new BusinessLogicValidationException(Errors.LicenseesIssuingOfficeAlreadyExists, $" {licenseeIssuingOfficeWrite.Description}");

            var licenseesIssuingOfficeEntity = licenseeIssuingOfficeWrite.Map(_userContext.AuthorityId);

            await _dbContext.LicenseesIssuingOffices.AddAsync(licenseesIssuingOfficeEntity);
            await _dbContext.SaveChangesAsync();

            return licenseesIssuingOfficeEntity.Id;
        }

        async Task ILicenseesService.UpdateLicenseesIssuingOffice(long id, LicenseesIssuingOfficeWrite licenseeIssuingOfficeWrite)
        {
            licenseeIssuingOfficeWrite?.Validate();

            var entity = await GetLicenseesIssuingOffice(id) ??
                         throw new BusinessLogicValidationException(Errors.LicenseesIssuingOfficeNotFound);

            if (await _dbContext.LicenseesIssuingOffices.AsNoTracking()
                .AnyAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id != id && l.Description == licenseeIssuingOfficeWrite!.Description && !l.Deleted))
                throw new BusinessLogicValidationException(Errors.LicenseesIssuingOfficeAlreadyExists, $" {licenseeIssuingOfficeWrite!.Description}");

            entity.Map(licenseeIssuingOfficeWrite!, _userContext.AuthorityId);
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        async Task ILicenseesService.DeleteLicenseesIssuingOffice(long id)
        {
            var entity = await GetLicenseesIssuingOffice(id) ??
                throw new BusinessLogicValidationException(Errors.LicenseesIssuingOfficeNotFound);

            entity.Deleted = true;
            _dbContext.Update(entity);

            await _dbContext.SaveChangesAsync();
        }

        async Task<LicenseesIssuingOfficeEntity?> GetLicenseesIssuingOffice(long id)
        {
            return await _dbContext.LicenseesIssuingOffices
                            .FirstOrDefaultAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == id && !l.Deleted);
        }

        #endregion

        #region private methods

        static LicenseeStatus CheckAndGetNewState(LicenseeStatus licenseeEntityState, LicenseeStatus newState)
        {
            return (licenseeEntityState, newState) switch
            {
                (LicenseeStatus.Created, LicenseeStatus.Created) => newState,
                (LicenseeStatus.Released, LicenseeStatus.Released) => newState,
                (LicenseeStatus.Suspended, LicenseeStatus.Suspended) => newState,
                (LicenseeStatus.Terminated, LicenseeStatus.Terminated) => newState,
                (LicenseeStatus.Created, LicenseeStatus.Released) => newState,
                (LicenseeStatus.Released, LicenseeStatus.Suspended) => newState,
                (LicenseeStatus.Released, LicenseeStatus.Terminated) => newState,
                (LicenseeStatus.Suspended, LicenseeStatus.Terminated) => newState,
                (LicenseeStatus.Suspended, LicenseeStatus.Released) => newState,
                _ => throw new BusinessLogicValidationException(Errors.InvalidLicenseeStateTransition)
            };
        }

        async Task CheckEntityAlreadyExist(LicenseeWrite licenseeWrite, long? id)
        {
            if (await _dbContext.Licensees.AsNoTracking()
                .AnyAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Number == licenseeWrite.Number &&
                                l.Type == licenseeWrite.Type && l.Status != LicenseeStatus.Terminated && (id == null || l.Id != id)))
                throw new BusinessLogicValidationException(Errors.LicenseeAlredyExist, $"{licenseeWrite.Number}");
        }

        static LicenseeEntity GetLicenseeEntityToSerialize(LicenseeEntity licensee) =>
            new()
            {
                Acronym = licensee.Acronym,
                AuthorityId = licensee.AuthorityId,
                EndDate = licensee.EndDate,
                ExpireActivityCause = licensee.ExpireActivityCause,
                Id = licensee.Id,
                Note = licensee.Note,
                Number = licensee.Number,
                GarageAddress = licensee.GarageAddress,
                IsFinancialAdministration = licensee.IsFinancialAdministration,
                IsFamilyCollaboration = licensee.IsFamilyCollaboration,
                FinancialAdministration = licensee.FinancialAdministration != default ?
                new FinancialAdministrationEntity
                {
                    LegalPersonId = licensee.FinancialAdministration.LegalPersonId,
                    AuthorityId = licensee.AuthorityId,
                    Id = licensee.FinancialAdministration.Id,
                } : default,
                LicenseesIssuingOffice = licensee.LicenseesIssuingOffice != default ?
                new LicenseesIssuingOfficeEntity
                {
                    Description = licensee.LicenseesIssuingOffice.Description,
                    AuthorityId = licensee.AuthorityId,
                    Id = licensee.LicenseesIssuingOffice.Id
                } : default,
                LicenseesIssuingOfficeId = licensee.LicenseesIssuingOfficeId,
                ReleaseDate = licensee.ReleaseDate,
                SysStartTime = licensee.SysStartTime,
                Status = licensee.Status,
                ShiftId = licensee.ShiftId,
                SubShift = licensee.SubShift != default ? new SubShiftEntity
                {
                    Name = licensee.SubShift.Name,
                    Id = licensee.SubShift.Id,
                    RestDay = licensee.SubShift.RestDay
                } : default,
                SubShiftId = licensee.SubShiftId,
                TaxiDriverAssociation = licensee.TaxiDriverAssociation != default ? new TaxiDriverAssociationEntity
                {
                    AuthorityId = licensee.TaxiDriverAssociation.AuthorityId,
                    Email = licensee.TaxiDriverAssociation.Email,
                    FiscalCode = licensee.TaxiDriverAssociation.FiscalCode,
                    Id = licensee.TaxiDriverAssociation.Id,
                    IsDeleted = licensee.TaxiDriverAssociation.IsDeleted,
                    Name = licensee.TaxiDriverAssociation.Name,
                    TelephoneNumber = licensee.TaxiDriverAssociation.TelephoneNumber
                } : default,
                TaxiDriverAssociationId = licensee.TaxiDriverAssociationId,
                Type = licensee.Type,
                Vehicle = licensee.Vehicle != default ? new VehicleEntity
                {
                    AuthorityId = licensee.Vehicle.AuthorityId,
                    SysStartTime = licensee.Vehicle.SysStartTime,
                    Id = licensee.Vehicle.Id,
                    InUseSince = licensee.Vehicle.InUseSince,
                    LicenseeId = licensee.Id,
                    LicensePlate = licensee.Vehicle.LicensePlate,
                    Model = licensee.Vehicle.Model,
                    RegistrationDate = licensee.Vehicle.RegistrationDate,
                } : default
            };

        #endregion
    }
}