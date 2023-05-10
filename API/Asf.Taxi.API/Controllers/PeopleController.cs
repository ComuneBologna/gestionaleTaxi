using Asf.Taxi.API.Mappers;
using Asf.Taxi.API.Models;
using Asf.Taxi.BusinessLogic;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using SmartTech.Common.API;
using SmartTech.Common.Enums;
using SmartTech.Common.Web.Security;
using SmartTech.Infrastructure.Search;

namespace Asf.Taxi.API.Controllers
{
    public class PeopleController : APIControllerBase
    {
        readonly IExportService _exportService;
        readonly ITaxiDriversService _peopleService;

        public PeopleController(ITaxiDriversService peopleService, IExportService exportService)
        {
            _peopleService = peopleService;
            _exportService = exportService;
        }

        [HttpGet("autocomplete")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<IEnumerable<PersonAutocompleteBase>> Autocomplete([FromQuery] string fullTextSearch, [FromQuery] PersonType? type) =>
            (await _peopleService.SearchDrivers(new TaxiDriversFilterCriteria
            {
                Ascending = true,
                ItemsPerPage = int.MaxValue,
                PageNumber = 1,
                KeySelector = nameof(TaxiDriverInfo.PersonDisplayName),
                PersonDescription = fullTextSearch,
                PersonType = type
            }, isAutoComplete: true)).Items.Select(a => new PersonAutocomplete
            {
                DisplayName = a.ExtendedPersonDisplayName,
                Id = a.Id
            });

        [HttpGet()]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<FilterResult<PersonInfo>> Search([FromQuery] TaxiDriversFilterCriteria search)
        {
            var res = await _peopleService.SearchDrivers(search);

            return new FilterResult<PersonInfo>
            {
                Items = res.Items.Select(a => new PersonInfo
                {
                    Address = a.Address,
                    DisplayName = a.PersonDisplayName,
                    EmailOrPEC = a.EmailOrPEC,
                    FiscalCode = a.FiscalCode,
                    Id = a.Id,
                    ResidentCity = a.ResidentCity,
                    ZipCode = a.ZipCode,
                    ResidentProvince = a.ResidentProvince,
                    Type = a.Type
                }),
                TotalCount = res.TotalCount
            };
        }

        [HttpPost("{type}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<long> PostIndividual([FromBody] PersonWriteAPI individual, PersonType type)
        {
            if (type == PersonType.Physical)
                return (await _peopleService.AddPerson(individual.MapPhysical(), type)).Id;
            else
                return (await _peopleService.AddPerson(individual.MapLegal(), type)).Id;
        }

        [HttpPut("{id}/{type}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task Put(long id, [FromBody] PersonWriteAPI person, PersonType type)
        {
            if (type == PersonType.Physical)
                await _peopleService.EditPerson(id, person.MapPhysical(), type);
            else
                await _peopleService.EditPerson(id, person.MapLegal(), type);
        }

        [HttpGet("{id}")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<Person> Get(long id)
        {
            var a = (await _peopleService.SearchDrivers(new TaxiDriversFilterCriteria { Id = id })).Items.FirstOrDefault();

            return new Person
            {
                Address = a.Address,
                EmailOrPEC = a.EmailOrPEC,
                FiscalCode = a.FiscalCode,
                Id = a.Id,
                PhoneNumber = a.PhoneNumber,
                ResidentCity = a.ResidentCity,
                ZipCode = a.ZipCode,
                BirthCity = a.BirthCity,
                BirthDate = a.BirthDate,
                FirstName = a.FirstName,
                LastName = a.LastName,
                BirthProvince = a.BirthProvince,
                ResidentProvince = a.ResidentProvince,
                Type = a.Type
            };
        }

        #region Export

        [HttpGet("export")]
        [PermissionAuthorize(TaxiPermissions.Taxi_Executive, TaxiPermissions.Taxi_Admin, TaxiPermissions.Taxi_Operator)]
        public async Task<string> PeopleExport([FromQuery] TaxiDriversFilterCriteria search)
        {

            var sc = search ?? new TaxiDriversFilterCriteria();
            sc.PageNumber = 1;
            sc.ItemsPerPage = int.MaxValue;

            var renderList = (await _peopleService.SearchDrivers(sc)).Items.Select(a => new
            {
                Nome = a.FirstName,
                Cognome = a.LastName,
                CodiceFiscale = a.FiscalCode,
                DataNascita = a.BirthDate,
                CittàNascita = a.BirthCity,
                ProvinciaNascita = a.BirthProvince,
                CittàResidenza = a.ResidentCity,
                ProvinciaResidenza = a.ResidentProvince,
                CAP = a.ZipCode,
                Indirizzo = a.Address,
                Email = a.EmailOrPEC,
                NumeroTelefono = a.PhoneNumber
            });

            return await _exportService.GetExcelExport(renderList, "export_anagrafica");
        }

        #endregion
    }
}