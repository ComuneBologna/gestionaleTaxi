using Asf.Taxi.API.Models;
using Asf.Taxi.BusinessLogic;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Models.Requests;
using Asf.Taxi.BusinessLogic.Models.Drivers;
using Asf.Taxi.BusinessLogic.Models.FinancialAdministrations;
using Asf.Taxi.BusinessLogic.Models.Licensees;
using Asf.Taxi.BusinessLogic.Models.LicenseesIssuingOffice;
using Asf.Taxi.BusinessLogic.Models.Vehicles;
using Asf.Taxi.BusinessLogic.Services;
using Asf.Taxi.DAL.Enums;
using Microsoft.AspNetCore.Mvc;
using SmartTech.Common.API;
using SmartTech.Common.Web.Security;
using SmartTech.Infrastructure.Search;
using SmartTech.Infrastructure.Extensions;
using Asf.Taxi.BusinessLogic.Localization;

namespace Asf.Taxi.API.Controllers
{
    public class LicenseesController : APIControllerBase
    {
        readonly IShiftsService _shiftService;
        readonly IExportService _exportService;
        readonly IVehiclesService _vehiclesService;
        readonly ITemplateService _templateService;
        readonly ITaxiDriversService _driverService;
        readonly ILicenseesService _licenseesService;
        readonly IFinancialAdministrationService _financialAdministrationService;

        public LicenseesController(ILicenseesService licenseesService, IVehiclesService vehiclesService, ITaxiDriversService driverService,
            IShiftsService shiftService, IFinancialAdministrationService financialAdministrationService, IExportService exportService,
            ITemplateService templateService)
        {
            _shiftService = shiftService;
            _exportService = exportService;
            _driverService = driverService;
            _vehiclesService = vehiclesService;
            _templateService = templateService;
            _licenseesService = licenseesService;
            _financialAdministrationService = financialAdministrationService;
        }

        #region Licensee

        [HttpGet("autocomplete")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<List<LicenseeInfo>> GetLicenseeAutocomplete(string number) =>
            (await _licenseesService.SearchLicensees<LicenseeInfo>(new LicenseesFilterCriteria
            {
                PageNumber = 1,
                ItemsPerPage = int.MaxValue,
                Number = number
            })).Items.ToList();

        [HttpGet("{licenseeId}/recipients/autocomplete")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<List<string?>> GetRecipientsAutocomplete(long licenseeId, [FromQuery] string mail) =>
            await _licenseesService.GetRecipientsAutocomplete(licenseeId, mail);

        [HttpGet("{licenseeId}/recipients")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<List<string?>> GetRecipients(long licenseeId) =>
            await _licenseesService.GetRecipients(licenseeId);

        [HttpPost]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<long> Post([FromBody] LicenseeTaxiWrite licensee) =>
            await _licenseesService.AddLicensee(licensee);

        [HttpPost("ncc")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<long> Post([FromBody] LicenseeNCCWrite licensee) =>
            await _licenseesService.AddLicensee(licensee);

        [HttpPut("{licenseeId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task Put(long licenseeId, [FromBody] LicenseeTaxiWrite licensee) =>
            await _licenseesService.UpdateLicensee(licenseeId, licensee);

        [HttpPut("{licenseeId}/renew")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task Renewlicensee(long licenseeId) => await _licenseesService.RenewLicensee(licenseeId);

        [HttpPut("ncc/{licenseeId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task Put(long licenseeId, [FromBody] LicenseeNCCWrite licensee) =>
            await _licenseesService.UpdateLicensee(licenseeId, licensee);

        [HttpPut("{licenseeId}/variation")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task Put(long licenseeId, [FromBody] LicenseeTaxiVariation licenseeVariaton) =>
            await _licenseesService.UpdateLicensee(licenseeId, licenseeVariaton);

        [HttpPut("ncc/{licenseeId}/variation")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task Put(long licenseeId, [FromBody] LicenseeNCCVariation licenseeVariaton) =>
            await _licenseesService.UpdateLicensee(licenseeId, licenseeVariaton);

        [HttpGet("{licenseeId}/variations")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<FilterResult<LicenseeVariation>> GetStateVariations(long licenseeId, [FromQuery] LicenseeVariationFilterCriteria criteria)
        {
            var result = await _licenseesService.SearchLicenseeVariations(new LicenseeVariationFilterCriteria()
            {
                Id = licenseeId,
                Ascending = criteria.Ascending,
                ItemsPerPage = criteria.ItemsPerPage,
                KeySelector = criteria.KeySelector,
                PageNumber = criteria.PageNumber,
                VariationDateFrom = criteria.VariationDateFrom,
                VariationDateTo = criteria.VariationDateTo,
                EndFrom = criteria.EndFrom,
                EndTo = criteria.EndTo,
                Ids = criteria.Ids,
                VariationNote = criteria.VariationNote,
                Note = criteria.Note,
                Number = criteria.Number,
                ReleaseDateFrom = criteria.ReleaseDateFrom,
                ReleaseDateTo = criteria.ReleaseDateTo,
                ShiftId = criteria.ShiftId,
                ShiftName = criteria.ShiftName,
                Status = criteria.Status,
                SubShiftId = criteria.SubShiftId,
                SubShiftName = criteria.SubShiftName,
                CarFuelType = criteria.CarFuelType,
                TaxiDriverAssociationId = criteria.TaxiDriverAssociationId,
                Type = criteria.Type,
                VehiclePlate = criteria.VehiclePlate,
                Acronym = criteria.Acronym,
                GarageAddress = criteria.GarageAddress,
                IsFamilyCollaboration = criteria.IsFamilyCollaboration,
                IsFinancialAdministration = criteria.IsFinancialAdministration,
                IsSubstitution = criteria.IsSubstitution,
                HasActiveSubstitution = criteria.HasActiveSubstitution,
                TaxiDriverLastName = criteria.TaxiDriverLastName
            });

            return new FilterResult<LicenseeVariation>
            {
                Items = result.Items.Select(i => new LicenseeVariation
                {
                    Id = i.LicenseeId,
                    Date = i.StartDate,
                    Note = i.Note,
                    Status = i.Status?.ToString(),
                    TaxiDriverAssociationName = i.TaxiDriverAssociationName,
                    Acronym = i.Acronym,
                    ActivityExpiredOnCause = i.ActivityExpiredOnCause,
                    IsFamilyCollaboration = i.IsFamilyCollaboration,
                    LicenseeNote = i.LicenseeNote,
                    ReleaseDate = i.ReleaseDate,
                    ShiftName = i.ShiftName,
                    SubShiftName = i.SubShiftName,
                    GarageAddress = i.GarageAddress,
                    IsFinancialAdministration = i.IsFinancialAdministration
                }),
                TotalCount = result.TotalCount
            };
        }

        [HttpGet]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<FilterResult<LicenseeInfo>> Get([FromQuery] LicenseesFilterCriteria search)
        {
            if (search.KeySelector == default)
            {
                search.KeySelector = nameof(LicenseeInfo.ReleaseDate);
                search.Ascending = false;
            }
            return await _licenseesService.SearchLicensees<LicenseeInfo>(search);
        }

        [HttpGet("{licenseeId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<LicenseeTaxiDetail?> GetTaxiLicensees(long licenseeId) =>
            (await _licenseesService.SearchLicensees<LicenseeTaxiDetail>(new LicenseesFilterCriteria
            {
                Id = licenseeId,
                Type = LicenseeTypes.Taxi
            })).Items.FirstOrDefault();

        [HttpGet("ncc/{licenseeId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<LicenseeNCCDetail?> GetNccLicencees(long licenseeId) =>
            (await _licenseesService.SearchLicensees<LicenseeNCCDetail>(new LicenseesFilterCriteria
            {
                Id = licenseeId,
                Type = LicenseeTypes.NCC_Auto
            })).Items.FirstOrDefault();

        #endregion

        #region Requests

        [HttpGet("{licenseeId}/requestsregisters")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<List<RequestRegister>> GetRequests(long licenseeId) =>
            (await _templateService.SearchRegisters<RequestRegister>(new RequestsRegisterFilterCriteria
            {
                Ascending = false,
                KeySelector = nameof(RequestRegister.LastUpdate),
                LicenseeId = licenseeId
            })).Items.ToList();

        [HttpGet("{licenseeId}/requestsregisters/autocomplete")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<List<RequestRegisterBase>> GetRequestsToSign(long licenseeId)
            => (await _templateService.SearchRegisters<RequestRegisterBase>(new RequestsRegisterFilterCriteria
            {
                ItemsPerPage = int.MaxValue,
                PageNumber = 1,
                Ascending = true,
                ExecutiveDigitalSignStatus = ExecutiveDigitalSignStatus.Required,
                LicenseeId = licenseeId
            })).Items.ToList();

        [HttpPost("{licenseeId}/requestsregisters/send")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task SendRequestsToSign(long licenseeId, RequestRegisterSend requestsToSend)
            => await _templateService.SendRequests(licenseeId, requestsToSend);

        [HttpPut("{licenseeId}/requestsregisters/{requestRegisterId}/tosign")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task PutRequestsToSign(long licenseeId, long requestRegisterId)
            => await _templateService.UpdateRequest(licenseeId, requestRegisterId);

        [HttpDelete("{licenseeId}/requestsregisters/{requestRegisterId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task DeleteRequest(long licenseeId, long requestRegisterId)
            => await _templateService.DeleteRequest(licenseeId, requestRegisterId);
        #endregion

        #region FinancialAdministrations

        [HttpPost("{licenseeId}/financialadministrations")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<long> Post(long licenseeId, [FromBody] FinancialAdministrationWrite financialAdministration) =>
            await _financialAdministrationService.AddFinancialAdministration(licenseeId, financialAdministration);

        [HttpPut("{licenseeId}/financialadministrations")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task Put(long licenseeId, [FromBody] FinancialAdministrationWrite financialAdministration) =>
            await _financialAdministrationService.EditFinancialAdministration(licenseeId, financialAdministration);

        [HttpGet("{licenseeId}/financialadministrations")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<FinancialAdministration?> Get(long licenseeId, [FromQuery] FinancialAdministrationFilterCriteria sc) =>
            (await _financialAdministrationService.SearchFinancialAdministrations(licenseeId, sc)).Items.FirstOrDefault();

        [HttpDelete("{licenseeId}/financialadministrations")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task DeleteFinancialAdministration(long licenseeId) =>
                await _financialAdministrationService.DeleteFinancialAdministration(licenseeId);

        #endregion

        #region TaxiDrivers

        [HttpGet("{licenseeId}/autocomplete")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<IEnumerable<PersonAutocompleteBase>> Autocomplete(string fullTextSearch, long licenseeId)
            => (await _driverService.SearchDrivers(new TaxiDriversFilterCriteria
            {
                Ascending = true,
                ItemsPerPage = int.MaxValue,
                PageNumber = 1,
                KeySelector = nameof(TaxiDriverInfo.PersonDisplayName),
                PersonDescription = fullTextSearch,
                LicenseeId = licenseeId
            }, isAutoComplete: true)).Items.Select(a => new PersonAutocompleteBase
            {
                DisplayName = a.ExtendedPersonDisplayName,
                Id = a.Id
            });

        [HttpGet("{licenseeId}/drivers")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<IEnumerable<PersonAutocomplete>> GetDriversByLicenseeId(long licenseeId)
            => (await _driverService.SearchDrivers(new TaxiDriversFilterCriteria
            {
                Ascending = true,
                ItemsPerPage = int.MaxValue,
                PageNumber = 1,
                KeySelector = nameof(PersonAutocomplete.DisplayName),
                LicenseeId = licenseeId
            })).Items.ToList().Select(a => new PersonAutocomplete
            {
                DisplayName = a.ExtendedPersonDisplayName,
                Id = a.Id,
                Documents = a.Documents
            });

        [HttpGet("{licenseeId}/owner")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<TaxiDriverInfo?> GetOwner(long licenseeId)
            => (await _driverService.SearchDrivers(
                new TaxiDriversFilterCriteria
                {
                    LicenseeId = licenseeId
                }, DriverTypes.Master)).Items.FirstOrDefault();

        [HttpGet("{licenseeId}/owner/variations")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<IEnumerable<TaxiDriverInfo>> GetOwnerVariations(long licenseeId)
            => (await _driverService.SearchDriverVariations<TaxiDriverInfo>(
                new TaxiDriverVariationFilterCriteria
                {
                    LicenseeId = licenseeId
                }, DriverTypes.Master)).Items;

        [HttpPost("{licenseeId}/owner")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<TaxiDriverInfo> OwnerPost(long licenseeId, [FromBody] TaxiDriverWrite ownerWrite) =>
            await _driverService.AddDriver(licenseeId, ownerWrite, DriverTypes.Master);

        [HttpPut("{licenseeId}/owner")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task OwnerPut(long licenseeId, [FromBody] TaxiDriverWrite ownerWrite) =>
            await _driverService.UpsertDriver(licenseeId, ownerWrite, DriverTypes.Master);

        [HttpPut("{licenseeId}/owner/variations")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task Put(long licenseeId, [FromBody] TaxiDriverVariation ownerWrite) =>
            await _driverService.UpsertDriver(licenseeId, ownerWrite, DriverTypes.Master);

        [HttpGet("{licenseeId}/documents/{driverId}/exists")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<bool> CheckOwnerDocument(long licenseeId, long driverId) =>
            await _driverService.CheckAllDocumentsExists(licenseeId, driverId);

        #endregion

        #region Substitution

        [HttpGet("{licenseeId}/substitutions")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<FilterResult<SubstitutionInfo>> GetSubstitutions(long licenseeId, [FromQuery] SubstitutionFilterCriteria criteria)
        {
            var result = await _driverService.SearchSubstitutions(licenseeId, criteria);

            if (string.IsNullOrWhiteSpace(criteria?.KeySelector) ||
                (criteria?.KeySelector ?? string.Empty) == nameof(SubstitutionInfo.Status))
                result.Items = result.Items.OrderBy(i => (byte)i.Status);

            return result;
        }

        [HttpGet("{licenseeId}/substitutions/{substitutionId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<SubstitutionInfo?> GetSubstitution(long licenseeId, long substitutionId) =>
            (await _driverService.SearchSubstitutions(licenseeId, new SubstitutionFilterCriteria
            {
                Id = substitutionId
            })).Items.FirstOrDefault();

        [HttpPost("{licenseeId}/substitutions")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<long> Add(long licenseeId, [FromBody] SubstitutionWrite substitution) =>
            await _driverService.AddSubstitution(licenseeId, substitution);

        [HttpPut("{licenseeId}/substitutions/{substitutionId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task Update(long licenseeId, long substitutionId, [FromBody] SubstitutionEdit substitution) =>
            await _driverService.EditSubstitution(licenseeId, substitutionId, substitution);

        [HttpDelete("{licenseeId}/substitutions/{substitutionId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task DeleteSubstitution(long licenseeId, long substitutionId) =>
            await _driverService.DeleteSubstitution(licenseeId, substitutionId);

        #endregion Substitution

        #region FamilyCollaborators

        [HttpGet("{licenseeId}/familycollaborators")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<TaxiDriverInfo> GetFamilyCollaborators(long licenseeId) =>
            (await _driverService.SearchDrivers(new TaxiDriversFilterCriteria
            {
                LicenseeId = licenseeId
            }, DriverTypes.FamilyCollaborator)).Items.FirstOrDefault();

        [HttpGet("{licenseeId}/familycollaborators/variations")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<IEnumerable<TaxiDriverInfo>> GetFamilyCollaboratorVariations(long licenseeId) =>
            (await _driverService.SearchDriverVariations<TaxiDriverInfo>(new TaxiDriverVariationFilterCriteria
            {
                LicenseeId = licenseeId
            }, DriverTypes.FamilyCollaborator)).Items;

        [HttpPost("{licenseeId}/familycollaborators")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<TaxiDriverInfo> PostFamilyCollaborator(long licenseeId, [FromBody] TaxiDriverWrite write) =>
            await _driverService.AddDriver(licenseeId, write, DriverTypes.FamilyCollaborator);

        [HttpPut("{licenseeId}/familycollaborators")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task PutFamilyCollaborator(long licenseeId, [FromBody] TaxiDriverWrite write) =>
            await _driverService.UpsertDriver(licenseeId, write, DriverTypes.FamilyCollaborator);

        [HttpPut("{licenseeId}/familycollaborators/variation")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task PutFamilyCollaboratorVariation(long licenseeId, [FromBody] TaxiDriverVariation write) =>
            await _driverService.UpsertDriver(licenseeId, write, DriverTypes.FamilyCollaborator);

        #endregion

        #region Shifts

        [HttpGet("shifts/{shiftId}/subshifts")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<IEnumerable<SubShiftAPIModel>> GetSubShiftsByShiftId(long shiftId) =>
            ((await _shiftService.SearchShifts<Shift>(new ShiftsSearchCriteria
            {
                Id = shiftId
            })).Items.FirstOrDefault()?.SubShifts ?? new List<SubShift>()).Select(ss => new SubShiftAPIModel
            {
                Id = ss.Id,
                Name = ss.Name,
                ShiftId = shiftId
            });

        [HttpGet("shifts")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<IEnumerable<ShiftAPIModel>> GetShifts() =>
            (await _shiftService.SearchShifts<Shift>(new ShiftsSearchCriteria
            {
                PageNumber = 1,
                ItemsPerPage = int.MaxValue
            })).Items.Select(s => new ShiftAPIModel
            {
                Id = s.Id,
                Name = s.Name
            });

        #endregion

        #region Vehicle

        [HttpPost("{licenseeId}/vehicle")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<long> Post(long licenseeId, [FromBody] VehicleWrite vehicleWrite) =>
            await _vehiclesService.AddVehicle(licenseeId, vehicleWrite);

        [HttpPut("{licenseeId}/vehicle")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task Put(long licenseeId, [FromBody] VehicleWrite vehicle) =>
            await _vehiclesService.UpsertVehicle(licenseeId, vehicle);

        [HttpPut("{licenseeId}/vehicle/variations")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task Modify(long licenseeId, [FromBody] VehicleVariation vehicleVariation) =>
            await _vehiclesService.UpsertVehicle(licenseeId, vehicleVariation);

        [HttpGet("{licenseeId}/vehicle")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<VehicleInfo> GetVehicle(long licenseeId) =>
            await _vehiclesService.GetVehicle(licenseeId);

        [HttpGet("{licenseeId}/vehicle/variations")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<IEnumerable<VehicleInfo>> GetVehicleVariations(long licenseeId) =>
            (await _vehiclesService.SearchVehicleVariations<VehicleInfo>(new VehicleVariationFilterCriteria
            {
                LicenseeId = licenseeId
            })).Items;

        #endregion

        #region LicenseesIssuingOffices

        [HttpGet("licenseesissuingoffices")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<List<LicenseesIssuingOffice>> GetLicenseesIssuingOffices([FromQuery] LicenseesIssuingOfficesFilterCriteria search)
        {
            var sc = search ?? new LicenseesIssuingOfficesFilterCriteria();
            sc.ItemsPerPage = int.MaxValue;
            sc.PageNumber = 1;
            sc.KeySelector = nameof(LicenseesIssuingOffice.Description);
            sc.Ascending = true;

            return (await _licenseesService.SearchLicenseesIssuingOffices(sc)).Items.ToList();
        }

        [HttpGet("licenseesissuingoffices/search")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<FilterResult<LicenseesIssuingOffice>> SearchLicenseesIssuingOffices([FromQuery] LicenseesIssuingOfficesFilterCriteria search) =>
            await _licenseesService.SearchLicenseesIssuingOffices(search);

        [HttpGet("licenseesissuingoffices/{id}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<LicenseesIssuingOffice> GetLicenseesIssuingOffice(long id) =>
            (await _licenseesService.SearchLicenseesIssuingOffices(new LicenseesIssuingOfficesFilterCriteria() { Id = id })).Items.FirstOrDefault();

        [HttpPost("licenseesissuingoffices")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<long> PostLicenseesIssuingOffice([FromBody] LicenseesIssuingOfficeWrite office) =>
                await _licenseesService.AddLicenseesIssuingOffice(office);

        [HttpPut("licenseesissuingoffices/{licenseesIssuingOfficeId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task PutLicenseesIssuingOffice(long licenseesIssuingOfficeId, [FromBody] LicenseesIssuingOfficeWrite office) =>
            await _licenseesService.UpdateLicenseesIssuingOffice(licenseesIssuingOfficeId, office);

        [HttpDelete("licenseesissuingoffices/{licenseesIssuingOfficeId}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task DeleteLicenseesIssuingOffice(long licenseesIssuingOfficeId) =>
        await _licenseesService.DeleteLicenseesIssuingOffice(licenseesIssuingOfficeId);

        #endregion

        #region Export

        [HttpGet("export")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<string> LicenseesExport([FromQuery] LicenseesFilterCriteria search)
        {
            var sc = search ?? new LicenseesFilterCriteria();
            sc.PageNumber = 1;
            sc.ItemsPerPage = int.MaxValue;

            var renderList = (await _licenseesService.SearchLicensees<LicenseeInfo>(sc)).Items.Select(a => new
            {
                NumeroLicenza = a.Number,
                ProprietarioLicenza = a.DriverDisplayName,
                DataRilascio = a.ReleaseDate,
                TipoLicenza = a.Type,
                StatoLicenza = a.Status?.GetDescription<EnumDescriptions>(),
                NomeAssociazione = a.TaxiDriverAssociationName,
                DocumentiTuttiPresenti = a.OwnerAllDocuments ? "NO" : "SI",
                NomeTurno = a.ShiftName,
                NomeSottoTurno = a.SubShiftName,
                TargaVeicolo = a.VehiclePlate,
                TipoAlimentazione = a.CarFuelType?.GetDescription<EnumDescriptions>()
            });

            return await _exportService.GetExcelExport(renderList, "export_licenze");

        }

        [HttpGet("licenseesissuingoffices/search/export")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<string> LicenseesIssuingOfficesExport([FromQuery] LicenseesIssuingOfficesFilterCriteria search)
        {
            var sc = search ?? new LicenseesIssuingOfficesFilterCriteria();
            sc.PageNumber = 1;
            sc.ItemsPerPage = int.MaxValue;

            var renderList = (await _licenseesService.SearchLicenseesIssuingOffices(sc)).Items.Select(a => new
            {
                Descrizione = a.Description
            });

            return await _exportService.GetExcelExport(renderList, "export_enti_rilascio_licenze");
        }

        [HttpGet("{licenseeId}/substitutions/export")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<string> SubstitutionsExport(long licenseeId, [FromQuery] SubstitutionFilterCriteria criteria)
        {
            var sc = criteria ?? new SubstitutionFilterCriteria();
            sc.PageNumber = 1;
            sc.ItemsPerPage = int.MaxValue;

            var renderList = (await _driverService.SearchSubstitutions(licenseeId, sc)).Items.Select(a => new
            {
                Sostituto = a.SubstituteDriver.PersonDisplayName,
                StatoSostituzione = a.Status,
                InizioSostituzione = a.StartDate,
                FineSostituzione = a.EndDate,
                Note = a.Note
            });

            return await _exportService.GetExcelExport(renderList, "export_sostituzioni");
        }

        #endregion
    }
}