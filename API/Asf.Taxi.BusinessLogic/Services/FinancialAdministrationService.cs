using Asf.Taxi.BusinessLogic.Localization;
using Asf.Taxi.BusinessLogic.Mapper;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Models.FinancialAdministrations;
using Asf.Taxi.DAL;
using Asf.Taxi.DAL.Entities;
using Asf.Taxi.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using SmartTech.Common.Enums;
using SmartTech.Infrastructure;
using SmartTech.Infrastructure.DataAccessLayer.EFCore;
using SmartTech.Infrastructure.Exceptions;
using SmartTech.Infrastructure.Extensions;
using SmartTech.Infrastructure.Search;
using SmartTech.Infrastructure.Validations;

namespace Asf.Taxi.BusinessLogic.Services
{
    public class FinancialAdministrationService : IFinancialAdministrationService
    {
        readonly ITaxiUserContext _userContext;
        readonly TaxiDriverDBContext _dbContext;
        readonly ITaxiDriversService _driversService;

        public FinancialAdministrationService(TaxiDriverDBContext dbContext, ITaxiUserContext userContext, ITaxiDriversService driversService)
        {
            _dbContext = dbContext;
            _userContext = userContext;
            _driversService = driversService;
        }

        async Task<long> IFinancialAdministrationService.AddFinancialAdministration(long licenseeId, FinancialAdministrationWrite write)
        {
            write?.Validate();

            if (licenseeId <= 0)
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            if (!await _dbContext.Licensees
                .AsNoTracking()
                .AnyAsync(a => a.AuthorityId == _userContext.AuthorityId && a.Id == licenseeId))
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            if (await _dbContext.FinancialAdministrations
                    .AsNoTracking()
                    .AnyAsync(f => f.AuthorityId == _userContext.AuthorityId && f.LicenseeId == licenseeId && !f.Deleted))
                throw new BusinessLogicValidationException(Errors.FinancialAdministrationNotAdded);

            var legalPerson = await _dbContext.Drivers
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.AuthorityId == _userContext.AuthorityId && d.Id == write!.LegalPersonId!.Value)
                ?? throw new BusinessLogicValidationException(Errors.LegalPersonNotFound);

            if (legalPerson.Type != PersonType.Legal)
                throw new BusinessLogicValidationException(Errors.FinancialAdministrationMustBeLegalPerson);

            var drivers = await _dbContext.Drivers
                .AsNoTracking()
                .Where(a => a.AuthorityId == _userContext.AuthorityId && write!.Drivers.Select(d => d.Id).Contains(a.Id))
                .ToListAsync();

            if (drivers.Any(d => d.Type == PersonType.Legal))
                throw new BusinessLogicValidationException(Errors.FinancialAdministrationDriversMustBeTaxi);

            var entity = write!.Map(_userContext.AuthorityId, licenseeId);

            await _dbContext.FinancialAdministrations.AddAsync(entity);

            var driverEntities = await AddDrivers(licenseeId, write!.Drivers);

            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        async Task IFinancialAdministrationService.DeleteFinancialAdministration(long licenseeId)
        {
            if (licenseeId <= 0)
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            if (!await _dbContext.Licensees
                .AsNoTracking()
                .AnyAsync(a => a.AuthorityId == _userContext.AuthorityId && a.Id == licenseeId))
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            var entity = await _dbContext.FinancialAdministrations
                .FirstOrDefaultAsync(f => f.AuthorityId == _userContext.AuthorityId && f.LicenseeId == licenseeId && !f.Deleted)
                ?? throw new BusinessLogicValidationException(Errors.FinancialAdministrationNotFound);

            entity.Deleted = true;
            _dbContext.FinancialAdministrations.Update(entity);

            var toDelete = await _dbContext.LicenseesTaxiDrivers
                .Where(a => a.AuthorityId == _userContext.AuthorityId && a.LicenseeId == licenseeId
                                    && a.TaxiDriverType == DriverTypes.FinancialAdministration)
                .ToListAsync();

            _dbContext.LicenseesTaxiDrivers.RemoveRange(toDelete);

            await _dbContext.SaveChangesAsync();
        }

        async Task IFinancialAdministrationService.EditFinancialAdministration(long licenseeId, FinancialAdministrationWrite write)
        {
            write?.Validate();

            if (licenseeId <= 0)
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            if (!await _dbContext.Licensees
                .AsNoTracking()
                .AnyAsync(a => a.AuthorityId == _userContext.AuthorityId && a.Id == licenseeId))
                throw new BusinessLogicValidationException(Errors.LicenseeNotFound);

            var entity = await _dbContext.FinancialAdministrations
                 .FirstOrDefaultAsync(f => f.AuthorityId == _userContext.AuthorityId && f.LicenseeId == licenseeId && !f.Deleted)
                 ?? throw new BusinessLogicValidationException(Errors.FinancialAdministrationNotFound);

            if (entity.LegalPersonId != write!.LegalPersonId)
            {
                var legalPerson = await _dbContext.Drivers
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.AuthorityId == _userContext.AuthorityId && d.Id == write!.LegalPersonId!.Value)
                ?? throw new BusinessLogicValidationException(Errors.LegalPersonNotFound);

                if (legalPerson.Type != PersonType.Legal)
                    throw new BusinessLogicValidationException(Errors.FinancialAdministrationMustBeLegalPerson);

                entity.LegalPersonId = write.LegalPersonId!.Value;
            }

            var drivers = await _dbContext.Drivers
                .AsNoTracking()
                .Where(a => a.AuthorityId == _userContext.AuthorityId && write!.Drivers.Select(d => d.Id).Contains(a.Id))
                .ToListAsync();

            if (drivers.Any(d => d.Type == PersonType.Legal))
                throw new BusinessLogicValidationException(Errors.FinancialAdministrationDriversMustBeTaxi);


            _dbContext.FinancialAdministrations.Update(entity);

            var uiDrivers = write.Drivers
                .Select(a => new LicenseeTaxiDriverEntity
                {
                    TaxiDriverId = a.Id,
                    AuthorityId = _userContext.AuthorityId,
                    TaxiDriverType = DriverTypes.FinancialAdministration,
                    IsFinancialAdministration = true,
                    LicenseeId = licenseeId
                }).ToList();

            var dbDrivers = await _dbContext.LicenseesTaxiDrivers
                .AsNoTracking()
                .Where(a => a.AuthorityId == _userContext.AuthorityId && a.LicenseeId == licenseeId
                    && a.TaxiDriverType == DriverTypes.FinancialAdministration)
                .ToListAsync();

            var toDelete = dbDrivers.Except(uiDrivers, GenericComparer.Create((LicenseeTaxiDriverEntity td) => td.TaxiDriverId)).ToList();

            _dbContext.LicenseesTaxiDrivers.RemoveRange(toDelete);

            var toAdd = uiDrivers.Where(a => a.AuthorityId == _userContext.AuthorityId && a.LicenseeId == licenseeId &&
                                                a.TaxiDriverType == DriverTypes.FinancialAdministration &&
                                                !dbDrivers.Select(b => b.TaxiDriverId).Contains(a.TaxiDriverId))
                                                .ToList();

            await AddDrivers(licenseeId, toAdd.Select(a => new PersonAutocompleteBase
            {
                Id = a.TaxiDriverId
            }).ToList());

            var toEdit = uiDrivers.Except(toAdd, GenericComparer.Create((LicenseeTaxiDriverEntity td) => td.TaxiDriverId)).ToList();

            foreach (var driver in toEdit)
            {
                var driverwrite = new TaxiDriverWrite
                {
                    PersonId = driver.TaxiDriverId
                };
                await _driversService.UpsertDriver(licenseeId, driverwrite, DriverTypes.FinancialAdministration);
            }

            await _dbContext.SaveChangesAsync();
        }

        async Task<FilterResult<FinancialAdministration>> IFinancialAdministrationService.SearchFinancialAdministrations(long licenseeId, FinancialAdministrationFilterCriteria filterCriteria)
        {
            var fc = filterCriteria ?? new FinancialAdministrationFilterCriteria();

            var query = _dbContext.FinancialAdministrations
                .Include(a => a.Licensee)
                .Include(a => a.LegalPerson)
                .Where(f => f.AuthorityId == _userContext.AuthorityId && !f.Deleted && f.LicenseeId == licenseeId);

            var ltds = await _dbContext.LicenseesTaxiDrivers
                .Include(a => a.TaxiDriver)
                .Where(a => a.AuthorityId == _userContext.AuthorityId && a.LicenseeId == licenseeId
                    && a.TaxiDriverType == DriverTypes.FinancialAdministration)
                .ToListAsync()
                ?? new();

            query = fc.Id.HasValue ? query.Where(f => f.Id == fc.Id.Value) : query;
            query = fc.Ids.Any() ? query.Where(f => fc.Ids.Contains(f.Id)) : query;
            query = fc.LegalPersonId.HasValue ? query.Where(f => fc.LegalPersonId == f.LegalPersonId) : query;
            query = !string.IsNullOrWhiteSpace(fc.LegalPersonDisplayName) ?
                query.Where(f => f.LegalPerson!.FirstName!.Contains(fc.LegalPersonDisplayName)
                    || f.LegalPerson!.LastName!.Contains(fc.LegalPersonDisplayName)
                    || f.LegalPerson!.FiscalCode!.Contains(fc.LegalPersonDisplayName)) : query;

            var result = await query.OrderAndPageAsync(fc.ToTypedCriteria(fc.MapSortCriteria()));

            return result.MapFilterResult(m => m.MapFinancialDetail(ltds.Select(a => a.TaxiDriver)!));
        }

        #region privateMethods

        async Task<IEnumerable<TaxiDriverEntity>> AddDrivers(long licenseeId, List<PersonAutocompleteBase> write)
        {
            List<TaxiDriverInfo> drivers = new();

            foreach (var driver in write)
            {
                var driverwrite = new TaxiDriverWrite
                {
                    PersonId = driver.Id
                };
                drivers.Add(await _driversService.AddDriver(licenseeId, driverwrite, DriverTypes.FinancialAdministration));
            }

            return drivers.Select(f => new TaxiDriverEntity
            {
                Address = f.Address,
                BirthCity = f.BirthCity,
                BirthDate = f.BirthDate,
                CollaborationType = f.CollaborationType,
                EmailOrPEC = f.EmailOrPEC,
                Id = f.Id,
                FirstName = f.FirstName,
                FiscalCode = f.FiscalCode,
                LastName = f.LastName,
                PhoneNumber = f.PhoneNumber,
                ResidentCity = f.ResidentCity,
                ShiftStartHour = !string.IsNullOrWhiteSpace(f.ShiftStarts) ? byte.TryParse(f.ShiftStarts.Split(':', StringSplitOptions.RemoveEmptyEntries)[0], out byte ssh) ? ssh : default(byte?) : default,
                ShiftStartMinutes = !string.IsNullOrWhiteSpace(f.ShiftStarts) ? byte.TryParse(f.ShiftStarts.Split(':', StringSplitOptions.RemoveEmptyEntries)[1], out byte ssm) ? ssm : default(byte?) : default,
                ZipCode = f.ZipCode,
                AuthorityId = _userContext.AuthorityId,
                SysStartTime = f.StartDate,
            });
        }

        #endregion
    }
}