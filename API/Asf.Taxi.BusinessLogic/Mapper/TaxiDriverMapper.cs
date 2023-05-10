using Asf.Taxi.BusinessLogic.Extensions;
using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.BusinessLogic.Models.Drivers;
using Asf.Taxi.DAL.Entities;
using SmartTech.Common.Enums;
using SmartTech.Infrastructure.Extensions;
using System.Data;
using System.Linq.Expressions;

namespace Asf.Taxi.BusinessLogic.Mapper
{
    static class TaxiDriverMapper
    {
        public static void Map(this TaxiDriverEntity entity, TaxiDriverWrite write, long authorityId)
        {
            entity.CollaborationType = write.CollaborationType;
            entity.ShiftStartHour = !string.IsNullOrWhiteSpace(write.ShiftStarts) ? byte.TryParse(write.ShiftStarts.Split(':', StringSplitOptions.RemoveEmptyEntries)[0], out byte ssh) ? ssh : default(byte?) : default;
            entity.ShiftStartMinutes = !string.IsNullOrWhiteSpace(write.ShiftStarts) ? byte.TryParse(write.ShiftStarts.Split(':', StringSplitOptions.RemoveEmptyEntries)[1], out byte ssm) ? ssm : default(byte?) : default;
            entity.Documents = write.Documents != default ? write.Documents.Select(a => new DocumentEntity
            {
                AuthorityId = authorityId,
                ValidityDate = a.ValidityDate.GetValueOrDefault(),
                Number = a.Number,
                ReleasedBy = a.ReleasedBy,
                Type = a.Type ?? default,
            }).ToArray() : default;
        }

        public static TaxiDriverInfo Map(this TaxiDriverEntity driverEntity, bool? checkAllDocuments = null) =>
            new()
            {
                PersonDisplayName = driverEntity.DisplayName,
                ExtendedPersonDisplayName = driverEntity.ExtendedDisplayName,
                Address = driverEntity.Address,
                BirthCity = driverEntity.BirthCity,
                BirthProvince = driverEntity.BirthProvince,
                BirthDate = driverEntity.BirthDate,
                EmailOrPEC = driverEntity.EmailOrPEC,
                FirstName = driverEntity.FirstName,
                LastName = driverEntity.LastName,
                FiscalCode = driverEntity.FiscalCode,
                PhoneNumber = driverEntity.PhoneNumber,
                ResidentCity = driverEntity.ResidentCity,
                ResidentProvince = driverEntity.ResidentProvince,
                ZipCode = driverEntity.ZipCode,
                ShiftStarts = TimeToString(driverEntity.ShiftStartHour, driverEntity.ShiftStartMinutes),
                StartDate = driverEntity.SysStartTime,
                Id = driverEntity.Id,
                PersonId = driverEntity.Id,
                CollaborationType = driverEntity.CollaborationType,
                Documents = driverEntity.Documents != default ? driverEntity.Documents.Select(d => new Document
                {
                    Number = d.Number,
                    ReleasedBy = d.ReleasedBy,
                    ValidityDate = d.ValidityDate,
                    Id = d.Id,
                    Type = d.Type
                }).ToList() : new List<Document>(),
                AllDocuments = checkAllDocuments,
                Type = driverEntity.Type
            };

        public static T Map<T>(this LicenseeHistoryEntity driver) where T : TaxiDriverInfo =>
            typeof(T).Equals(typeof(TaxiDriverHistory)) ?
            new TaxiDriverHistory
            {
                PersonDisplayName = $"{driver.TaxiDriverLastName} {driver.TaxiDriverFirstName}",
                FirstName = driver.TaxiDriverLastName,
                LastName = driver.TaxiDriverFirstName,
                FiscalCode = driver.TaxiDriverFiscalCode,
                Note = driver.Note,
                StartDate = driver.SysStartTime,
                Id = driver.Id,
                TaxiDriverId = driver.TaxiDriverId.GetValueOrDefault(),
                EndDate = driver.SysEndTime,
                CollaborationType = driver.TaxiDriverCollaborationType,
                ShiftStarts = TimeToString(driver.TaxiDriverShiftStartHour, driver.TaxiDriverShiftStartMinutes),
                Type = driver.TaxiDriverPersonType
            } as T :
            new TaxiDriverInfo
            {
                PersonDisplayName = $"{driver.TaxiDriverLastName} {driver.TaxiDriverFirstName}",
                FirstName = driver.TaxiDriverLastName,
                LastName = driver.TaxiDriverFirstName,
                FiscalCode = driver.TaxiDriverFiscalCode,
                Note = driver.Note,
                StartDate = driver.SysStartTime,
                CollaborationType = driver.TaxiDriverCollaborationType,
                Id = driver.TaxiDriverId.GetValueOrDefault(),
                ShiftStarts = TimeToString(driver.TaxiDriverShiftStartHour, driver.TaxiDriverShiftStartMinutes),
                Type = driver.TaxiDriverPersonType
            } as T;

        public static SubstitutionInfo Map(this TaxiDriverSubstitutionEntity substitutionEntity) =>
            new()
            {
                EndDate = substitutionEntity.EndDate,
                Id = substitutionEntity.Id,
                Note = substitutionEntity.Note,
                StartDate = substitutionEntity.StartDate,
                Status = substitutionEntity.Status!.Value,
                SubstituteDriverId = substitutionEntity.DriverToId,
                SubstituteDriver = new TaxiDriverSubstitution
                {
                    DriverId = substitutionEntity.DriverToId,
                    PersonDisplayName = substitutionEntity.DriverTo!.DisplayName
                },
                IsExpiring = DateTime.UtcNow.Date <= substitutionEntity.EndDate && substitutionEntity.EndDate <= DateTime.UtcNow.Date.AddDays(5) && DateTime.UtcNow.Date >= substitutionEntity.StartDate
            };

        public static TaxiDriverSubstitutionEntity Map(this SubstitutionWrite substitution, long authorityId, long licenseeId) =>
            new()
            {
                AuthorityId = authorityId,
                DriverToId = substitution!.SubstituteDriverId!.Value,
                LicenseeId = licenseeId,
                Note = substitution.Note,
                EndDate = substitution!.EndDate!.Value,
                StartDate = substitution!.StartDate!.Value,
                Status = DateTime.UtcNow.Date.CalculateSubstitutionStatus(substitution.StartDate.Value.Date, substitution.EndDate.Value.Date)
            };

        public static void Map(this TaxiDriverEntity driverEntity, TaxiDriverWrite driverWrite)
        {
            var ssw = !string.IsNullOrWhiteSpace(driverWrite.ShiftStarts) ? driverWrite.ShiftStarts.Split(':', StringSplitOptions.RemoveEmptyEntries) : default;
            driverEntity.ShiftStartHour = ssw != default ? byte.TryParse(ssw[0], out byte ssh) ? ssh : default(byte?) : default;
            driverEntity.ShiftStartMinutes = ssw != default ? byte.TryParse(ssw[1], out byte ssm) ? ssm : default(byte?) : default;
            driverEntity.CollaborationType = driverWrite.CollaborationType;
        }

        internal static Expression<Func<TaxiDriverSubstitutionEntity, dynamic>> MapSortCriteria(this SubstitutionFilterCriteria sfc) =>
            sfc.KeySelector.FirstCharToUpper() switch
            {
                nameof(SubstitutionInfo.EndDate) => x => x.EndDate,
                nameof(SubstitutionInfo.Note) => x => x.Note,
                nameof(SubstitutionInfo.StartDate) => x => x.StartDate,
                nameof(SubstitutionInfo.Status) => x => x.Status,
                _ => x => x.Id
            };

        internal static Expression<Func<LicenseeHistoryEntity, dynamic>> MapSortCriteria(this TaxiDriverVariationFilterCriteria ofc) =>
            ofc.KeySelector?.FirstCharToUpper() switch
            {
                nameof(TaxiDriverHistory.TaxiDriverId) => x => x.TaxiDriverId,
                nameof(TaxiDriverHistory.PersonDisplayName) => x => $"{x.TaxiDriverLastName} {x.TaxiDriverFirstName}",
                nameof(TaxiDriverHistory.CollaborationType) => x => x.TaxiDriverCollaborationType,
                nameof(TaxiDriverHistory.FirstName) => x => x.TaxiDriverFirstName,
                nameof(TaxiDriverHistory.FiscalCode) => x => x.TaxiDriverFiscalCode,
                nameof(TaxiDriverHistory.LastName) => x => x.TaxiDriverLastName,
                nameof(TaxiDriverHistory.Note) => x => x.Note,
                nameof(TaxiDriverHistory.StartDate) => x => x.SysStartTime,
                nameof(TaxiDriverHistory.ShiftStarts) => x => x.Status,
                nameof(TaxiDriverHistory.EndDate) => x => x.SysEndTime,
                nameof(TaxiDriverHistory.Type) => x => x.TaxiDriverPersonType,
                _ => x => x.Id
            };

        internal static Expression<Func<TaxiDriverEntity, dynamic>> MapSortCriteria(this TaxiDriversFilterCriteria dfc) =>
            dfc.KeySelector?.FirstCharToUpper() switch
            {
                nameof(TaxiDriverInfo.PersonDisplayName) => x => x.LastName,
                nameof(TaxiDriverInfo.ExtendedPersonDisplayName) => x => x.LastName,
                nameof(TaxiDriverInfo.Address) => x => x.Address,
                nameof(TaxiDriverInfo.BirthCity) => x => x.BirthCity,
                nameof(TaxiDriverInfo.BirthProvince) => x => x.BirthProvince,
                nameof(TaxiDriverInfo.BirthDate) => x => x.BirthDate,
                nameof(TaxiDriverInfo.CollaborationType) => x => x.CollaborationType,
                nameof(TaxiDriverInfo.EmailOrPEC) => x => x.EmailOrPEC,
                nameof(TaxiDriverInfo.FirstName) => x => x.FirstName,
                nameof(TaxiDriverInfo.FiscalCode) => x => x.FiscalCode,
                nameof(TaxiDriverInfo.LastName) => x => x.LastName,
                nameof(TaxiDriverInfo.PhoneNumber) => x => x.PhoneNumber,
                nameof(TaxiDriverInfo.ResidentCity) => x => x.ResidentCity,
                nameof(TaxiDriverInfo.ResidentProvince) => x => x.ResidentProvince,
                nameof(TaxiDriverInfo.ZipCode) => x => x.ZipCode,
                nameof(TaxiDriverInfo.ShiftStarts) => x => $"{x.ShiftStartHour}:{x.ShiftStartMinutes}",
                nameof(TaxiDriverInfo.StartDate) => x => x.SysStartTime,
                nameof(TaxiDriverInfo.Type) => x => x.Type,
                _ => x => x.Id
            };

        static string TimeToString(byte? hours, byte? minutes)
        {
            if (!hours.HasValue || !minutes.HasValue)
                return string.Empty;

            var h = hours.ToString();
            var m = minutes.ToString();

            return $"{(h.Length < 2 ? $"0{h}" : h)}:{(m.Length < 2 ? $"0{m}" : m)}";
        }

        public static TaxiDriverEntity Map<T>(this T person, long authorityId) where T : PersonWriteBase
        {
            if (typeof(T).Equals(typeof(PhysicalPersonWrite)))
            {
                var physical = person as PhysicalPersonWrite;
                return new TaxiDriverEntity
                {
                    AuthorityId = authorityId,
                    FiscalCode = physical!.FiscalCode,
                    Address = physical.Address,
                    EmailOrPEC = physical.EmailOrPEC,
                    PhoneNumber = physical.PhoneNumber,
                    BirthDate = physical.BirthDate!.Value,
                    BirthCity = physical.BirthCity,
                    BirthProvince = physical.BirthProvince,
                    ResidentCity = physical.ResidentCity,
                    ResidentProvince = physical.ResidentProvince,
                    ZipCode = physical.ZipCode,
                    FirstName = physical.FirstName,
                    LastName = physical.LastName,
                    SysStartTime = DateTime.UtcNow,
                    Type = PersonType.Physical
                };
            }
            else
            {
                var legal = person as LegalPersonWrite;
                return new TaxiDriverEntity
                {
                    AuthorityId = authorityId,
                    FiscalCode = legal!.VATNumber,
                    Address = legal.Address,
                    EmailOrPEC = legal.EmailOrPEC,
                    PhoneNumber = legal.PhoneNumber,
                    LastName = legal.Nominative,
                    SysStartTime = DateTime.UtcNow,
                    Type = PersonType.Legal
                };
            }
        }

        public static void Map<T>(this TaxiDriverEntity entity, T person) where T : PersonWriteBase
        {
            if (typeof(T).Equals(typeof(PhysicalPersonWrite)))
            {
                var physical = person as PhysicalPersonWrite;
                entity.Address = physical!.Address;
                entity.EmailOrPEC = physical.EmailOrPEC;
                entity.PhoneNumber = physical.PhoneNumber;
                entity.ResidentCity = physical.ResidentCity;
                entity.ResidentProvince = physical.ResidentProvince;
                entity.ZipCode = physical.ZipCode;
                entity.BirthDate = physical.BirthDate!.Value;
                entity.BirthCity = physical.BirthCity;
                entity.BirthProvince = physical.BirthProvince;
                entity.FirstName = physical.FirstName;
                entity.FiscalCode = physical.FiscalCode;
                entity.LastName = physical.LastName;
            }
            else
            {
                var legal = person as LegalPersonWrite;
                entity.Address = legal!.Address;
                entity.EmailOrPEC = legal.EmailOrPEC;
                entity.PhoneNumber = legal.PhoneNumber;
                entity.LastName = legal.Nominative;
                entity.FiscalCode = legal.VATNumber;
            }

        }
    }
}