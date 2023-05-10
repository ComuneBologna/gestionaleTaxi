using Asf.Taxi.BusinessLogic.Events;
using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.BusinessLogic.Mapper;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Models.Drivers;
using Asf.Taxi.DAL;
using Asf.Taxi.DAL.Entities;
using Asf.Taxi.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using SmartTech.Common;
using SmartTech.Common.Enums;
using SmartTech.Infrastructure;
using SmartTech.Infrastructure.DataAccessLayer.EFCore;
using SmartTech.Infrastructure.Exceptions;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Search;
using SmartTech.Infrastructure.SystemEvents;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Services
{
    class TaxiDriversService : ITaxiDriversService
    {
        readonly ITaxiUserContext _userContext;
        readonly TaxiDriverDBContext _dbContext;
        readonly ISystemEventManager _systemEventManager;

        public TaxiDriversService(TaxiDriverDBContext dbContext, ITaxiUserContext userContext, ISystemEventManager systemEventManager)
        {
            _dbContext = dbContext;
            _userContext = userContext;
            _systemEventManager = systemEventManager;
        }

        async Task<FilterResult<TaxiDriverInfo>> ITaxiDriversService.SearchDrivers(TaxiDriversFilterCriteria filterCriteria, DriverTypes? type, bool? isAutoComplete)
        {
            var fc = filterCriteria ?? new TaxiDriversFilterCriteria
            {
                ItemsPerPage = 50,
                PageNumber = 1
            };

            var query = _dbContext.Drivers
                .Include(a => a.Documents)
                .Include(a => a.LicenseesTaxiDrivers)
                .Include(a => a.Substitutions)
                .AsNoTracking()
                .Where(d => d.AuthorityId == _userContext.AuthorityId);

            if (isAutoComplete.HasValue && isAutoComplete.Value)
            {
                if (fc.LicenseeId.HasValue)
                {
                    var licensee = await _dbContext.Licensees
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.Id == fc.LicenseeId);

                    if (licensee != default && licensee.Type == LicenseeTypes.Taxi)
                    {
                        query = query.Where(q => !q.LicenseesTaxiDrivers.Any() || q.LicenseesTaxiDrivers.Any(a => a.LicenseeStatus == LicenseeStatus.Terminated || a.LicenseeStatus == LicenseeStatus.Suspended));

                        var ltdids = await query.Select(a => a.Id).ToListAsync();
                        var subs = await _dbContext.DriverSubstitutions
                            .Where(s => ltdids.Contains(s.DriverToId))
                            .GroupBy(s => s.DriverToId)
                            .Select(s => s.FirstOrDefault(x => x.EndDate == s.Max(w => w.EndDate)))
                            .ToListAsync();
                        var filteredSubs = subs.Where(s => s.EndDate < DateTime.UtcNow.Date)
                            .Select(s => s.DriverToId)
                            .ToList();

                        query = query.Where(q => !q.Substitutions.Any() || filteredSubs.Contains(q.Id));
                    }
                    else if (licensee != default && licensee.Type == LicenseeTypes.NCC_Auto)
                    {
                        query = query.Where(a => (!a.LicenseesTaxiDrivers.Any())
                            || (a.LicenseesTaxiDrivers
                                .Where(a => a.TaxiDriverType == DriverTypes.Master)
                                .GroupBy(b => b.TaxiDriverId)
                                .Select(c => c.Count() <= 7)
                                .Any(c => c) && a.LicenseesTaxiDrivers.Any(b => b.LicenseeType == LicenseeTypes.NCC_Auto))
                            || a.LicenseesTaxiDrivers
                                .Any(b => b.LicenseeStatus == (LicenseeStatus.Suspended | LicenseeStatus.Terminated)));
                    }
                }
            }
            else
            {
                query = fc.LicenseeId.HasValue ? query.Where(q => q.LicenseesTaxiDrivers.Any(b => b.LicenseeId == fc.LicenseeId.Value)) : query;
            }
            query = type != null ? query.Where(a => a.LicenseesTaxiDrivers.Any(b => b.TaxiDriverType == type)) : query;
            query = !string.IsNullOrWhiteSpace(fc.PersonDescription) ? query.Where(q => q.LastName.Contains(fc.PersonDescription) || q.FirstName.Contains(fc.PersonDescription) || q.FiscalCode.Contains(fc.PersonDescription)) : query;
            query = fc.Id.HasValue ? query.Where(q => q.Id == fc.Id) : query;
            query = (fc.Ids?.Count ?? 0) > 0 ? query.Where(q => fc.Ids.Contains(q.Id)) : query;
            query = !string.IsNullOrWhiteSpace(fc.FiscalCode) ? query.Where(q => q.FiscalCode.Contains(fc.FiscalCode)) : query;
            query = !string.IsNullOrWhiteSpace(fc.PhoneNumber) ? query.Where(q => q.PhoneNumber.Contains(fc.PhoneNumber)) : query;
            query = !string.IsNullOrWhiteSpace(fc.EmailOrPEC) ? query.Where(q => q.EmailOrPEC.Contains(fc.EmailOrPEC)) : query;
            query = fc.VehicleId.HasValue ? query.Where(q => q.LicenseesTaxiDrivers.Any(b => b.Licensee.Vehicle.Id == fc.VehicleId.Value)) : query;
            query = !string.IsNullOrWhiteSpace(fc.LicenseeNumber) ? query.Where(q => q.LicenseesTaxiDrivers.Any(b => b.Licensee.Number == fc.LicenseeNumber)) : query;
            query = fc.PersonType.HasValue ? query.Where(q => q.Type == fc.PersonType) : query;

            var result = await query.OrderAndPageAsync(fc.ToTypedCriteria(fc.MapSortCriteria()));

            return isAutoComplete.HasValue && isAutoComplete.Value ?
                result.MapFilterResult(m => m.Map()) :
                result.MapFilterResult(m => m.Map(((ITaxiDriversService)this).CheckAllDocumentsExists(0, m.Id).Result));
        }

        async Task<FilterResult<T>> ITaxiDriversService.SearchDriverVariations<T>(TaxiDriverVariationFilterCriteria filterCriteria, DriverTypes type)
        {
            var sc = filterCriteria ?? new TaxiDriverVariationFilterCriteria();

            if (string.IsNullOrWhiteSpace(sc.KeySelector))
            {
                sc.KeySelector = nameof(TaxiDriverHistory.StartDate);
                sc.Ascending = false;
            }

            var query = _dbContext.LicenseesHistory
                            .AsNoTracking()
                            .Where(l => l.AuthorityId == _userContext.AuthorityId && l.VariationType == VariationTypes.TaxiDriversVariation
                                        && l.TaxiDriverType == type);

            query = sc.VariationDateFrom.HasValue ?
            query.Where(q => q.SysStartTime >= sc.VariationDateFrom) : query;
            query = sc.VariationDateTo.HasValue ?
                query.Where(q => q.SysStartTime <= sc.VariationDateTo) : query;
            query = sc.TaxiDriverId.HasValue ? query.Where(q => q.TaxiDriverId == sc.TaxiDriverId) : query;
            query = sc.TaxiDriverIds.Count > 0 ? query.Where(q => sc.TaxiDriverIds.Contains(q.TaxiDriverId.GetValueOrDefault())) : query;
            query = sc.LicenseeId.HasValue ? query.Where(q => q.LicenseeId == sc.LicenseeId) : query;
            query = sc.LicenseeIds.Count > 0 ? query.Where(q => sc.LicenseeIds.Contains(q.LicenseeId)) : query;
            query = !string.IsNullOrWhiteSpace(sc.Note) ? query.Where(q => sc.Note.Contains(q.LicenseeNote)) : query;

            var result = await query.OrderAndPageAsync(sc.ToTypedCriteria(sc.MapSortCriteria()));

            return result.MapFilterResult(m => m.Map<T>());
        }

        async Task<TaxiDriverInfo> ITaxiDriversService.AddPerson<T>(T person, PersonType type)
        {
            TaxiDriverEntity entity;
            if (type == PersonType.Physical && typeof(T).Equals(typeof(PhysicalPersonWrite)))
            {
                var physical = person as PhysicalPersonWrite;
                physical?.Validate();

                if (await _dbContext.Drivers
                    .AsNoTracking()
                    .AnyAsync(p => p.AuthorityId == _userContext.AuthorityId && p.FiscalCode == physical!.FiscalCode))
                    throw new BusinessLogicValidationException(string.Format(Errors.UserAlredyExist, "Codice Fiscale", physical!.FiscalCode));
                entity = physical!.Map(_userContext.AuthorityId);
            }
            else if (type == PersonType.Legal && typeof(T).Equals(typeof(LegalPersonWrite)))
            {
                var legal = person as LegalPersonWrite;
                legal?.Validate();

                if (await _dbContext.Drivers
                    .AsNoTracking()
                    .AnyAsync(p => p.AuthorityId == _userContext.AuthorityId && p.FiscalCode == legal!.VATNumber))
                    throw new BusinessLogicValidationException(string.Format(Errors.UserAlredyExist, "Partita IVA", legal!.VATNumber));
                entity = legal!.Map(_userContext.AuthorityId);
            }
            else
                throw new BusinessLogicValidationException(Errors.PersonTypeNotValid);

            await _dbContext.Drivers.AddAsync(entity);
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
                            ItemType = ItemTypes.Driver,
                            MemoLine = null,
                            OperationType = OperationTypes.Creating
                        }
                    }
                }.JsonSerialize(false),
                Type = TaxiDriverEventTypes.TaxiDriverAudit
            });

            return entity.Map();
        }

        async Task ITaxiDriversService.EditPerson<T>(long personId, T person, PersonType type)
        {
            if (personId <= 0)
                throw new BusinessLogicValidationException(Errors.UserNotFound);

            var entity = await _dbContext.Drivers
                .Include(a => a.LicenseesTaxiDrivers)
                .FirstOrDefaultAsync(a => a.AuthorityId == _userContext.AuthorityId && a.Id == personId)
                ?? throw new BusinessLogicValidationException(Errors.UserNotFound);

            if (entity.Type.HasValue && entity.Type.Value != type)
                throw new BusinessLogicValidationException(Errors.UserNotFound);

            if (type == PersonType.Physical && typeof(T).Equals(typeof(PhysicalPersonWrite)))
            {
                var physical = person as PhysicalPersonWrite;
                physical?.Validate();

                if (await _dbContext.Drivers
                    .AsNoTracking()
                    .AnyAsync(p => p.AuthorityId == _userContext.AuthorityId &&
                    p.FiscalCode == physical!.FiscalCode && p.Id != personId))
                    throw new BusinessLogicValidationException(string.Format(Errors.UserAlredyExist, "Codice Fiscale", physical!.FiscalCode));

                entity.Map(physical!);
            }
            else if (type == PersonType.Legal && typeof(T).Equals(typeof(LegalPersonWrite)))
            {
                var legal = person as LegalPersonWrite;
                legal?.Validate();

                if (await _dbContext.Drivers
                    .AsNoTracking()
                    .AnyAsync(p => p.AuthorityId == _userContext.AuthorityId &&
                    p.FiscalCode == legal!.VATNumber && p.Id != personId))
                    throw new BusinessLogicValidationException(string.Format(Errors.UserAlredyExist, "Partita IVA", legal!.VATNumber));
                entity.Map(legal!);
            }
            else
                throw new BusinessLogicValidationException(Errors.PersonTypeNotValid);

            if (entity.EmailOrPEC != default && !await _dbContext.Recipients.AnyAsync(r => r.Mail!.Equals(entity.EmailOrPEC)))
            {
                var licenseeIds = entity.LicenseesTaxiDrivers.Select(ltd => ltd.LicenseeId).ToList();

                foreach (var licenseeId in licenseeIds)
                    await _dbContext.Recipients.AddAsync(new RecipientEntity()
                    {
                        AuthorityId = _userContext.AuthorityId,
                        LicenseeId = licenseeId,
                        Mail = entity.EmailOrPEC,
                        Order = (byte)entity.LicenseesTaxiDrivers.FirstOrDefault(ltd => ltd.TaxiDriverId == personId).TaxiDriverType
                    });
            }

            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        async Task<TaxiDriverInfo> ITaxiDriversService.AddDriver(long licenseeId, TaxiDriverWrite write, DriverTypes type)
        {
            write?.Validate();

            await CheckDocuments(write!.Documents);

            var entity = await _dbContext.Drivers
                .Include(a => a.LicenseesTaxiDrivers)
                .FirstOrDefaultAsync(a => a.AuthorityId == _userContext.AuthorityId && a.Id == write.PersonId)
                ?? throw new BusinessLogicValidationException(Errors.UserNotFound);

            var licensee = await _dbContext.Licensees
                .Include(l => l.LicenseesTaxiDrivers)
                .Include(l => l.Shift)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == licenseeId && l.Status != LicenseeStatus.Terminated)
                ?? throw new BusinessLogicValidationException(Errors.LicenseeUseStateError);

            await CheckDriverInLicensees(licensee, entity);

            entity.Map(write, _userContext.AuthorityId);
            _dbContext.Drivers.Update(entity);

            if (entity.EmailOrPEC != default && !await _dbContext.Recipients.AnyAsync(r => r.Mail!.Equals(entity.EmailOrPEC)))
                await _dbContext.Recipients.AddAsync(new RecipientEntity()
                {
                    AuthorityId = _userContext.AuthorityId,
                    LicenseeId = licenseeId,
                    Mail = entity.EmailOrPEC,
                    Order = (byte)entity.LicenseesTaxiDrivers.FirstOrDefault(ltd => ltd.TaxiDriverId == write.PersonId).TaxiDriverType
                });

            var obj = new LicenseeTaxiDriverEntity
            {
                AuthorityId = _userContext.AuthorityId,
                IsFinancialAdministration = licensee.IsFinancialAdministration,
                LicenseeId = licenseeId,
                LicenseeStatus = licensee.Status,
                LicenseeType = licensee.Type,
                TaxiDriverId = write.PersonId,
                TaxiDriverType = type,
            };

            await _dbContext.LicenseesTaxiDrivers.AddAsync(obj);

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
                            ItemType = ItemTypes.Driver,
                            MemoLine = null,
                            OperationType = OperationTypes.Creating
                        }
                    }
                }.JsonSerialize(false),
                Type = TaxiDriverEventTypes.TaxiDriverAudit
            });

            return entity.Map();
        }

        async Task ITaxiDriversService.UpsertDriver(long licenseeId, TaxiDriverWrite write, DriverTypes type)
        {
            var driverToUpdate = write is TaxiDriverVariation ? write as TaxiDriverVariation : write;

            driverToUpdate?.Validate();

            var licensee = await _dbContext.Licensees.AsNoTracking()
                                    .Include(l => l.LicenseesTaxiDrivers)
                                    .Include(l => l.Shift)
                                    .FirstOrDefaultAsync(l => l.AuthorityId == _userContext.AuthorityId &&
                                            l.Id == licenseeId);

            if (licensee.Status == LicenseeStatus.Terminated)
                throw new BusinessLogicValidationException(Errors.LicenseeUseStateError);

            await CheckDocuments(write.Documents);

            var driverEntity = await _dbContext.Drivers.Include(a => a.LicenseesTaxiDrivers)
                                                            .ThenInclude(a => a.Licensee)
                                                            .FirstOrDefaultAsync(d => d.AuthorityId == _userContext.AuthorityId &&
                                                                        d.LicenseesTaxiDrivers.Any(a => a.LicenseeId == licenseeId && a.TaxiDriverType == type))
                                            ?? throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);

            if (type == DriverTypes.FinancialAdministration)
            {
                driverEntity = await _dbContext.Drivers.Include(a => a.LicenseesTaxiDrivers)
                                                           .ThenInclude(a => a.Licensee)
                                                           .FirstOrDefaultAsync(d => d.AuthorityId == _userContext.AuthorityId &&
                                                                       d.LicenseesTaxiDrivers.Any(a => a.LicenseeId == licenseeId && a.TaxiDriverType == type) && d.Id == write.PersonId)
                                           ?? throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);
            }

            await CheckDriverInLicensees(licensee, driverEntity);

            var driverOld = GetDriverEntityToSerialize(driverEntity);

            if (write is TaxiDriverVariation driverVariation)
            {
                driverEntity = await _dbContext.Drivers.Include(a => a.LicenseesTaxiDrivers)
                                                .ThenInclude(a => a.Licensee)
                                                    .ThenInclude(a => a.FinancialAdministration)
                                            .Include(a => a.LicenseesTaxiDrivers)
                                                .ThenInclude(a => a.Licensee)
                                                    .ThenInclude(a => a.TaxiDriverAssociation)
                                            .Include(a => a.LicenseesTaxiDrivers)
                                                .ThenInclude(a => a.Licensee)
                                                    .ThenInclude(a => a.Vehicle)
                                            .Include(a => a.LicenseesTaxiDrivers)
                                                .ThenInclude(a => a.Licensee)
                                                    .ThenInclude(a => a.LicenseesIssuingOffice)
                                .FirstOrDefaultAsync(d => d.AuthorityId == _userContext.AuthorityId &&
                                                            d.LicenseesTaxiDrivers.Any(a => a.LicenseeId == licenseeId && a.TaxiDriverType == type))
                                ?? throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);

                var driverHistory = driverEntity.Map(driverVariation.Note, DateTime.UtcNow);

                await _dbContext.AddAsync(driverHistory);

                driverEntity.SysStartTime = DateTime.UtcNow;


                var newDriverEntity = await _dbContext.Drivers.Include(a => a.LicenseesTaxiDrivers).FirstOrDefaultAsync(a => a.AuthorityId == _userContext.AuthorityId && a.Id == write.PersonId);
                newDriverEntity.Map(write);
                _dbContext.Drivers.Update(newDriverEntity);

                var licenseeTaxidriver = _dbContext.LicenseesTaxiDrivers
                    .FirstOrDefault(a => a.AuthorityId == _userContext.AuthorityId && a.LicenseeId == licenseeId && a.TaxiDriverType == type && a.TaxiDriverId == driverEntity.Id);
                _dbContext.LicenseesTaxiDrivers.Remove(licenseeTaxidriver);

                var newLicenseeTaxidriver = new LicenseeTaxiDriverEntity
                {
                    AuthorityId = _userContext.AuthorityId,
                    IsFinancialAdministration = licenseeTaxidriver.IsFinancialAdministration,
                    LicenseeId = licenseeTaxidriver.LicenseeId,
                    LicenseeStatus = licenseeTaxidriver.LicenseeStatus,
                    LicenseeType = licenseeTaxidriver.LicenseeType,
                    TaxiDriverId = newDriverEntity.Id,
                    TaxiDriverType = type,
                };
                await _dbContext.LicenseesTaxiDrivers.AddAsync(newLicenseeTaxidriver);

            }
            else
            {
                if (driverEntity.Id != write.PersonId)
                {
                    throw new BusinessLogicValidationException("Per modificare il tassista è necessario operare in modalità variazione");
                }

            }

            driverEntity.Map(write);
            _dbContext.Drivers.Update(driverEntity);

            var uiDocuments = write.Documents != default ? write.Documents.Select(a => new DocumentEntity
            {
                Id = a.Id ?? 0,
                AuthorityId = _userContext.AuthorityId,
                Number = a.Number,
                ReleasedBy = a.ReleasedBy,
                TaxiDriverId = write.PersonId,
                Type = a.Type ?? default,
                ValidityDate = a.ValidityDate.GetValueOrDefault()
            }).ToList() : new List<DocumentEntity>();

            var dbDocuments = await _dbContext.Documents
                                    .Where(a => a.AuthorityId == _userContext.AuthorityId && a.TaxiDriverId == write.PersonId)
                                    .AsNoTracking()
                                    .ToListAsync();

            var docuementsToDelete = dbDocuments.Except(uiDocuments, GenericComparer.Create((DocumentEntity td) => td.Id)).ToList();

            _dbContext.Documents.RemoveRange(docuementsToDelete);
            await _dbContext.SaveChangesAsync();

            var documentToAdd = uiDocuments.Where(a => a.Id <= 0).ToList();

            await _dbContext.Documents.AddRangeAsync(documentToAdd);

            var documentsToUpdate = uiDocuments.Except(documentToAdd);
            _dbContext.Documents.UpdateRange(documentsToUpdate);

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
                            ItemId = write.PersonId,
                            ItemType = ItemTypes.Driver,
                            MemoLine = write is TaxiDriverVariation variation ? variation.Note : null,
                            OperationType = write is TaxiDriverVariation ? OperationTypes.Changing : OperationTypes.Updating
                        },
                        Data = driverOld
                    }
                }.JsonSerialize(false),
                Type = TaxiDriverEventTypes.TaxiDriverAudit
            });
        }

        async Task<bool> ITaxiDriversService.CheckAllDocumentsExists(long licenseeId, long taxiDriverId)
        {
            bool ret = false;

            var driverEntity = await _dbContext.Drivers
                .AsNoTracking()
                .Include(a => a.Documents)
                .FirstOrDefaultAsync(d => d.AuthorityId == _userContext.AuthorityId && d.Id == taxiDriverId);

            if (driverEntity != default)
                ret = driverEntity.Documents.GroupBy(a => a.Type).Count() >= Enum.GetNames(typeof(DriverTypes)).Length;

            return ret;
        }

        #region substitution

        async Task<long> ITaxiDriversService.AddSubstitution(long licenseeId, SubstitutionWrite substitution)
        {
            substitution?.Validate();

            var licensee = await _dbContext.Licensees
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == licenseeId)
                ?? throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            if (licensee!.Type == LicenseeTypes.NCC_Auto)
                throw new BusinessLogicValidationException(Errors.SubstitutionNCCLicensee);

            if (await _dbContext.LicenseesTaxiDrivers
                .AsNoTracking()
                .Where(a => a.AuthorityId == _userContext.AuthorityId && a.LicenseeId == licenseeId)
                .AnyAsync(a => a.TaxiDriverId == substitution!.SubstituteDriverId))
                throw new BusinessLogicValidationException(Errors.SubstituionDriverNotReplaceableWithHimself);

            if (await _dbContext.LicenseesTaxiDrivers
                .AsNoTracking()
                .AnyAsync(d => d.AuthorityId == _userContext.AuthorityId &&
                                    d.LicenseeStatus != LicenseeStatus.Terminated &&
                                    d.TaxiDriverId == substitution!.SubstituteDriverId))
                throw new BusinessLogicValidationException(Errors.SubstituionDriverNotReplaceableActiveLicensee);

            if (await _dbContext.Licensees
                .AsNoTracking()
                .AnyAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == licenseeId && l.Status == LicenseeStatus.Terminated))
                throw new BusinessLogicValidationException(Errors.LicenseeUseStateError);

            if (await _dbContext.DriverSubstitutions
                .AsNoTracking()
                .AnyAsync(ds => ds.AuthorityId == _userContext.AuthorityId
                && ds.LicenseeId == licenseeId
                && (ds.StartDate >= substitution!.StartDate &&
                                    ds.StartDate <= substitution.EndDate || ds.EndDate >= substitution.StartDate &&
                                    ds.EndDate <= substitution.EndDate || ds.StartDate <= substitution.StartDate &&
                                    ds.EndDate >= substitution.EndDate)))
                throw new BusinessLogicValidationException(Errors.SubstitutionInvalidDateRange);

            var entity = substitution!.Map(_userContext.AuthorityId, licenseeId);
            var driver = await _dbContext.Drivers.FirstOrDefaultAsync(d => d.AuthorityId == _userContext.AuthorityId && d.Id == substitution!.SubstituteDriverId);

            if (driver?.EmailOrPEC != default && !await _dbContext.Recipients.AnyAsync(r => r.Mail!.Equals(driver.EmailOrPEC)))
                await _dbContext.Recipients.AddAsync(new RecipientEntity()
                {
                    AuthorityId = _userContext.AuthorityId,
                    LicenseeId = licenseeId,
                    Mail = driver.EmailOrPEC,
                    Order = (byte)driver.LicenseesTaxiDrivers.FirstOrDefault(ltd => ltd.TaxiDriverId == substitution!.SubstituteDriverId).TaxiDriverType
                });

            await _dbContext.DriverSubstitutions.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        async Task ITaxiDriversService.EditSubstitution(long licenseeId, long id, SubstitutionEdit substitution)
        {
            substitution?.Validate();

            var licensee = await _dbContext.Licensees
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == licenseeId)
                ?? throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            if (licensee!.Type == LicenseeTypes.NCC_Auto)
                throw new BusinessLogicValidationException(Errors.SubstitutionNCCLicensee);

            var substitutionEntity = await _dbContext.DriverSubstitutions
                .Include(ds => ds.DriverTo)
                .FirstOrDefaultAsync(ds => ds.AuthorityId == _userContext.AuthorityId && ds.LicenseeId == licenseeId && ds.Id == id)
                ?? throw new BusinessLogicValidationException(Errors.SubstitutionNotFound);

            if ((substitution!.Status!.Value != SubstitutionStatus.Archived && substitution.Status.Value != substitutionEntity.Status)
                || (substitution.Status.Value == SubstitutionStatus.Archived && substitutionEntity.Status != SubstitutionStatus.Terminated))
                throw new BusinessLogicValidationException(Errors.SubstitutionStatusIsInvalid);

            var entityOld = GetDriverSubstitutionEntityToSerialize(substitutionEntity);

            if (await _dbContext.LicenseesTaxiDrivers.Include(a => a.Licensee)
                .AsNoTracking()
                .Where(a => a.AuthorityId == _userContext.AuthorityId && a.LicenseeId == licenseeId)
                .AnyAsync(a => a.TaxiDriverId == substitution!.SubstituteDriverId))
                throw new BusinessLogicValidationException(Errors.SubstituionDriverNotReplaceableWithHimself);

            if (await _dbContext.LicenseesTaxiDrivers.AsNoTracking()
                    .AnyAsync(d => d.AuthorityId == _userContext.AuthorityId && d.LicenseeId == licenseeId &&
                                    d.TaxiDriverId == substitution!.SubstituteDriverId))
                throw new BusinessLogicValidationException(Errors.SubstitutionTaxiDriverNotSelectable);

            if (await _dbContext.Licensees.AsNoTracking()
                                .AnyAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == licenseeId &&
                                                l.Status == LicenseeStatus.Terminated))
                throw new BusinessLogicValidationException(Errors.LicenseeUseStateError);

            if (await _dbContext.DriverSubstitutions.AsNoTracking().AnyAsync(ds => ds.AuthorityId == _userContext.AuthorityId &&
                    ds.LicenseeId == licenseeId && ds.Id != id && (ds.StartDate >= substitution.StartDate &&
                    ds.StartDate <= substitution.EndDate || ds.EndDate >= substitution.StartDate &&
                    ds.EndDate <= substitution.EndDate || ds.StartDate <= substitution.StartDate &&
                    ds.EndDate >= substitution.EndDate)))
                throw new BusinessLogicValidationException(Errors.SubstitutionInvalidDateRange);

            substitutionEntity.DriverToId = substitution!.SubstituteDriverId!.Value;
            substitutionEntity.Note = substitution.Note;
            substitutionEntity.EndDate = substitution.EndDate!.Value;
            substitutionEntity.StartDate = substitution.StartDate!.Value;
            if (substitutionEntity.Status!.Value == SubstitutionStatus.Terminated && substitution.Status.Value == SubstitutionStatus.Archived)
                substitutionEntity.Status = substitution.Status;

            _dbContext.DriverSubstitutions.Update(substitutionEntity);
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
                            ItemType = ItemTypes.Driver,
                            MemoLine = null,
                            OperationType = OperationTypes.Deleting
                        },
                        Data = entityOld
                    }
                }.JsonSerialize(false),
                Type = TaxiDriverEventTypes.TaxiDriverAudit
            });
        }

        async Task ITaxiDriversService.DeleteSubstitution(long licenseeId, long substitutionId)
        {
            var substitutionEntity = await _dbContext.DriverSubstitutions
                .Include(ds => ds.DriverTo)
                .FirstOrDefaultAsync(ds => ds.AuthorityId == _userContext.AuthorityId && ds.LicenseeId == licenseeId && ds.Id == substitutionId)
                ?? throw new BusinessLogicValidationException(Errors.SubstitutionNotFound);

            if (await _dbContext.Licensees
                .AsNoTracking()
                .AnyAsync(l => l.AuthorityId == _userContext.AuthorityId && l.Id == licenseeId && l.Status == LicenseeStatus.Terminated))
                throw new BusinessLogicValidationException(Errors.LicenseeUseStateError);

            _dbContext.DriverSubstitutions.Remove(substitutionEntity);
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
                            ItemId = substitutionId,
                            ItemType = ItemTypes.Driver,
                            MemoLine = null,
                            OperationType = OperationTypes.Deleting
                        },
                        Data = GetDriverSubstitutionEntityToSerialize(substitutionEntity)
                    }
                }.JsonSerialize(false),
                Type = TaxiDriverEventTypes.TaxiDriverAudit
            });
        }

        async Task<FilterResult<SubstitutionInfo>> ITaxiDriversService.SearchSubstitutions(long licenseeId, SubstitutionFilterCriteria criteria)
        {
            var fc = criteria ?? new();
            var query = _dbContext.DriverSubstitutions
                                    .Include(ds => ds.DriverTo)
                                        .ThenInclude(dt => dt.LicenseesTaxiDrivers)
                                    .AsNoTracking()
                                    .Where(ds => ds.AuthorityId == _userContext.AuthorityId && ds.LicenseeId == licenseeId);

            query = fc.Id.HasValue ? query.Where(q => q.Id == fc.Id) : query;
            query = fc.EndDate.HasValue ? query.Where(q => q.EndDate <= fc.EndDate) : query;
            query = fc.StartDate.HasValue ? query.Where(q => q.StartDate >= fc.StartDate) : query;
            query = fc.SubstituteDriverId.HasValue ? query.Where(q => q.DriverToId == fc.SubstituteDriverId) : query;
            query = fc.SubstitutionStatus.HasValue ? query.Where(q => q.Status == fc.SubstitutionStatus) : query;

            var result = await query.OrderAndPageAsync(fc.ToTypedCriteria(fc.MapSortCriteria()));
            return result.MapFilterResult(s => s.Map());
        }

        #endregion

        #region private methods

        async Task CheckDriverInLicensees(LicenseeEntity licensee, TaxiDriverEntity entity)
        {
            if (licensee.Status == LicenseeStatus.Terminated || licensee.Status == LicenseeStatus.Suspended)
                throw new BusinessLogicValidationException(Errors.LicenseeInvalidStatus);

            var licenseesForDriver = await _dbContext.LicenseesTaxiDrivers
                .AsNoTracking()
                .Include(a => a.Licensee)
                .Where(a => a.TaxiDriverId == entity.Id)
                .ToListAsync();

            if (!licenseesForDriver.Any(a => a.LicenseeId == licensee.Id) && licenseesForDriver.Any(a => a.TaxiDriverType == DriverTypes.FamilyCollaborator))
                throw new BusinessLogicValidationException(string.Format(Errors.LicenseeFamilyCollaborationNumTresholdExceeded, "1"));

            if (licensee.Type != LicenseeTypes.NCC_Auto)
            {
                if (!licenseesForDriver.Any(a => a.LicenseeId == licensee.Id) && licensee.LicenseesTaxiDrivers.Any(a => a.TaxiDriverType == DriverTypes.Master) && entity.LicenseesTaxiDrivers.Any(a => a.TaxiDriverType == DriverTypes.Master))
                    throw new BusinessLogicValidationException(string.Format(Errors.OwnerLicenseeNumTresholdExceeded, "1"));

                if (!licenseesForDriver.Any(a => a.LicenseeId == licensee.Id) && licensee.IsFamilyCollaboration && licensee.LicenseesTaxiDrivers.Any(a => a.TaxiDriverType == DriverTypes.FamilyCollaborator) && entity.LicenseesTaxiDrivers.Any(a => a.TaxiDriverType == DriverTypes.FamilyCollaborator))
                    throw new BusinessLogicValidationException(string.Format(Errors.LicenseeFamilyCollaborationNumTresholdExceeded, "1"));

                if (!licenseesForDriver.Any(a => a.LicenseeId == licensee.Id) && licenseesForDriver.Any(a => a.TaxiDriverType == DriverTypes.Master))
                    throw new BusinessLogicValidationException(string.Format(Errors.OwnerLicenseeNumTresholdExceeded, "1"));
            }
            else
            {
                if (!licenseesForDriver.Any(a => a.LicenseeId == licensee.Id) && licenseesForDriver.Any(a => !a.IsFinancialAdministration && a.TaxiDriverType == DriverTypes.Master))
                    throw new BusinessLogicValidationException(string.Format(Errors.InvalidNccOwner, licenseesForDriver?.FirstOrDefault(a => !a.IsFinancialAdministration)?.Licensee?.Number ?? string.Empty));

                if (!licenseesForDriver.Any(a => a.LicenseeId == licensee.Id) && licenseesForDriver.Where(a => a.TaxiDriverType == DriverTypes.Master).Count() > 6)
                    throw new BusinessLogicValidationException(string.Format(Errors.OwnerLicenseeNumTresholdExceeded, "7"));

                if (!licenseesForDriver.Any(a => a.LicenseeId == licensee.Id) && licenseesForDriver.Any(a => a.TaxiDriverType == DriverTypes.FinancialAdministration))
                    throw new BusinessLogicValidationException(string.Format(Errors.FALicenseeNumTresholdExceeded, "1"));
            }
        }

        static TaxiDriverEntity GetDriverEntityToSerialize(TaxiDriverEntity driver) =>
            new()
            {
                AuthorityId = driver.AuthorityId,
                SysStartTime = driver.SysStartTime,
                Id = driver.Id,
                ShiftStartHour = driver.ShiftStartHour,
                ShiftStartMinutes = driver.ShiftStartMinutes,
                CollaborationType = driver.CollaborationType,

                Address = driver.Address,
                BirthCity = driver.BirthCity,
                BirthProvince = driver.BirthProvince,
                BirthDate = driver.BirthDate,
                EmailOrPEC = driver.EmailOrPEC,
                FirstName = driver.FirstName,
                FiscalCode = driver.FiscalCode,
                LastName = driver.LastName,
                PhoneNumber = driver.PhoneNumber,
                ResidentCity = driver.ResidentCity,
                ResidentProvince = driver.ResidentProvince,
                ZipCode = driver.ZipCode
            };

        static TaxiDriverSubstitutionEntity GetDriverSubstitutionEntityToSerialize(TaxiDriverSubstitutionEntity substitution) =>
            new()
            {
                AuthorityId = substitution.AuthorityId,

                DriverTo = new TaxiDriverEntity
                {
                    AuthorityId = substitution!.DriverTo!.AuthorityId,
                    SysStartTime = substitution.DriverTo.SysStartTime,
                    Id = substitution.DriverTo.Id,
                    ShiftStartHour = substitution.DriverTo.ShiftStartHour,
                    ShiftStartMinutes = substitution.DriverTo.ShiftStartMinutes,

                    Address = substitution.DriverTo.Address,
                    BirthCity = substitution.DriverTo.BirthCity,
                    BirthProvince = substitution.DriverTo.BirthProvince,
                    BirthDate = substitution.DriverTo.BirthDate,
                    EmailOrPEC = substitution.DriverTo.EmailOrPEC,
                    FirstName = substitution.DriverTo.FirstName,
                    FiscalCode = substitution.DriverTo.FiscalCode,
                    LastName = substitution.DriverTo.LastName,
                    PhoneNumber = substitution.DriverTo.PhoneNumber,
                    ResidentCity = substitution.DriverTo.ResidentCity,
                    ResidentProvince = substitution.DriverTo.ResidentProvince,
                    ZipCode = substitution.DriverTo.ZipCode
                },
                DriverToId = substitution.DriverToId,
                Id = substitution.Id,
                LicenseeId = substitution.LicenseeId,
                Note = substitution.Note,
                EndDate = substitution.EndDate,
                StartDate = substitution.StartDate
            };

        async Task CheckDocuments(List<Document> documents)
        {
            await Task.CompletedTask;
            if (documents != default)
            {
                if (documents.Any(a => string.IsNullOrWhiteSpace(a.Number) || string.IsNullOrWhiteSpace(a.ReleasedBy)))
                    throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.Validation, "Numero Documento e ente rilascio obbligatori");

                if (documents.GroupBy(a => a.Type).Select(a => a.Count() > 1).Any(a => a))
                    throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.Validation, "i sono più Documenti dello stesso tipo");
            }
        }

        #endregion
    }
}