using Asf.Taxi.BusinessLogic.Models.Licensees;
using Asf.Taxi.BusinessLogic.Models.LicenseesIssuingOffice;
using Asf.Taxi.DAL.Entities;
using Asf.Taxi.DAL.Enums;
using SmartTech.Infrastructure.Extensions;
using System.Linq.Expressions;

namespace Asf.Taxi.BusinessLogic.Mapper
{
    public static class LicenseesMapper
    {
        public static T Map<T>(this LicenseeEntity licensee, bool ownerCheck, bool collaboratorCheck) where T : LicenseeBase =>
            typeof(T) == typeof(LicenseeNCCDetail) ?
            new LicenseeNCCDetail
            {
                Acronym = licensee.Acronym,
                TaxiDriverAssociationId = licensee.TaxiDriverAssociationId,
                TaxiDriverAssociationName = licensee.TaxiDriverAssociation?.Name,
                EndDate = licensee.EndDate,
                ActivityExpiredOnCause = licensee.ExpireActivityCause,
                Id = licensee.Id,
                Note = licensee.Note,
                Number = licensee.Number,
                ReleaseDate = licensee.ReleaseDate,
                LicenseesIssuingOfficeId = licensee.LicenseesIssuingOfficeId,
                Status = licensee.Status,
                Type = licensee.Type,
                VehicleId = licensee.Vehicle?.Id,
                GarageAddress = licensee.GarageAddress,
                IsFinancialAdministration = licensee.IsFinancialAdministration,
                IsFamilyCollaboration = licensee.IsFamilyCollaboration,
                LicenseesIssuingOfficeDescription = licensee.LicenseesIssuingOffice?.Description ?? string.Empty,
            } as T :
            typeof(T) == typeof(LicenseeTaxiDetail) ?
            new LicenseeTaxiDetail
            {
                Acronym = licensee.Acronym,
                TaxiDriverAssociationId = licensee.TaxiDriverAssociationId,
                TaxiDriverAssociationName = licensee.TaxiDriverAssociation?.Name,
                EndDate = licensee.EndDate,
                ActivityExpiredOnCause = licensee.ExpireActivityCause,
                Id = licensee.Id,
                Note = licensee.Note,
                Number = licensee.Number,
                ReleaseDate = licensee.ReleaseDate,
                LicenseesIssuingOfficeId = licensee.LicenseesIssuingOfficeId,
                ShiftId = licensee.ShiftId,
                ShiftName = licensee.Shift?.Name,
                Status = licensee.Status,
                SubShiftId = licensee.SubShiftId,
                SubShiftName = licensee.SubShift?.Name,
                Type = licensee.Type,
                VehicleId = licensee.Vehicle?.Id,
                IsFamilyCollaboration = licensee.IsFamilyCollaboration,
                LicenseesIssuingOfficeDescription = licensee.LicenseesIssuingOffice?.Description ?? string.Empty
            } as T : typeof(T) == typeof(LicenseeInfo) ?
            new LicenseeInfo
            {
                CarFuelType = licensee.Vehicle?.CarFuelType,
                Id = licensee.Id,
                Number = licensee.Number,
                ReleaseDate = licensee.ReleaseDate,
                DriverDisplayName = licensee.LicenseesTaxiDrivers?.FirstOrDefault(a => a.TaxiDriverType == DriverTypes.Master)?.TaxiDriver.DisplayName ?? "",
                IsExpiring = licensee.EndDate <= new DateTime(DateTime.UtcNow.Year, 12, 31),
                ShiftName = licensee.Shift?.Name,
                Status = licensee.Status,
                SubShiftName = licensee.SubShift?.Name,
                TaxiDriverAssociationName = licensee.TaxiDriverAssociation?.Name,
                Type = licensee.Type,
                VehiclePlate = licensee.Vehicle?.LicensePlate,
                CollaboratorAllDocuments = collaboratorCheck ? false : true,
                OwnerAllDocuments = ownerCheck ? false : true
            } as T :
            typeof(T) == typeof(LicenseeBase) ?
            new LicenseeBase
            {
                Number = licensee.Number,
                Status = licensee.Status,
                Type = licensee.Type
            } as T : default;

        public static LicenseeEntity Map<T>(this T licenseeWrite, long authorityId) where T : LicenseeWrite =>
            (licenseeWrite is LicenseeNCCWrite ncc) ?
                new()
                {
                    Acronym = ncc.Acronym,
                    TaxiDriverAssociationId = ncc.TaxiDriverAssociationId,
                    EndDate = new DateTime(ncc.ReleaseDate!.Value.Year + 5, 12, 31),
                    ExpireActivityCause = ncc.ActivityExpiredOnCause,
                    Note = ncc.Note,
                    Number = ncc.Number,
                    ReleaseDate = ncc.ReleaseDate.Value.Date,
                    LicenseesIssuingOfficeId = ncc.LicenseesIssuingOfficeId,
                    Status = ncc.Status.Value,
                    SysStartTime = DateTime.UtcNow,
                    Type = ncc.Type.Value,
                    AuthorityId = authorityId,
                    GarageAddress = ncc.GarageAddress,
                    IsFinancialAdministration = ncc.IsFinancialAdministration.Value,
                    IsFamilyCollaboration = ncc.IsFamilyCollaboration.Value,
                } : (licenseeWrite is LicenseeTaxiWrite taxi) ?
                new()
                {
                    Acronym = taxi.Acronym,
                    TaxiDriverAssociationId = taxi.TaxiDriverAssociationId,
                    EndDate = new DateTime(taxi.ReleaseDate!.Value.Year + 5, 12, 31),
                    ExpireActivityCause = taxi.ActivityExpiredOnCause,
                    Note = taxi.Note,
                    Number = taxi.Number,
                    ReleaseDate = taxi.ReleaseDate.Value.Date,
                    LicenseesIssuingOfficeId = taxi.LicenseesIssuingOfficeId,
                    Status = taxi.Status.Value,
                    SysStartTime = DateTime.UtcNow,
                    Type = taxi.Type.Value,
                    AuthorityId = authorityId,
                    IsFamilyCollaboration = taxi.IsFamilyCollaboration.Value,
                    ShiftId = taxi.ShiftId,
                    SubShiftId = taxi.SubShiftId
                } : default;

        public static void Map(this LicenseeEntity licenseeEntity, LicenseeWrite licenseeWrite)
        {
            if (licenseeEntity.ReleaseDate != licenseeWrite.ReleaseDate)
                licenseeEntity.EndDate = new DateTime(licenseeWrite.ReleaseDate!.Value.Year + 5, 12, 31);
            licenseeEntity.ReleaseDate = licenseeWrite.ReleaseDate!.Value.Date;
            licenseeEntity.Acronym = licenseeWrite.Acronym;
            licenseeEntity.TaxiDriverAssociationId = licenseeWrite.TaxiDriverAssociationId;
            licenseeEntity.ExpireActivityCause = licenseeWrite.ActivityExpiredOnCause;
            licenseeEntity.Note = licenseeWrite.Note;
            licenseeEntity.Number = licenseeWrite.Number;
            licenseeEntity.LicenseesIssuingOfficeId = licenseeWrite.LicenseesIssuingOfficeId;
            licenseeEntity.Type = licenseeWrite.Type!.Value;
            licenseeEntity.IsFamilyCollaboration = licenseeWrite.IsFamilyCollaboration!.Value;
            licenseeEntity.Status = licenseeWrite.Status ?? licenseeEntity.Status;

            if (licenseeWrite is LicenseeNCCWrite nccWrite)
            {
                licenseeEntity.GarageAddress = nccWrite.GarageAddress;
                licenseeEntity.IsFinancialAdministration = nccWrite.IsFinancialAdministration!.Value;
            }
            else if (licenseeWrite is LicenseeTaxiWrite taxiWrite)
            {
                //TODO: I turni possono essere modificati? In caso affermantivo cosa accade alle logiche che dipendono da questo?
                licenseeEntity.ShiftId = taxiWrite.ShiftId;
                licenseeEntity.SubShiftId = taxiWrite.SubShiftId;
            }
        }

        public static Expression<Func<LicenseeEntity, dynamic>> MapSortCriteria(this LicenseesFilterCriteria lfc) =>
            lfc.KeySelector.FirstCharToUpper() switch
            {
                nameof(LicenseeInfo.CarFuelType) => x => x.Vehicle != default ? x.Vehicle.CarFuelType : default,
                nameof(LicenseeInfo.ReleaseDate) => x => x.ReleaseDate,
                nameof(LicenseeInfo.ShiftName) => x => x.Shift != default ? x.Shift.Name : string.Empty,
                nameof(LicenseeInfo.SubShiftName) => x => x.SubShift != default ? x.SubShift.Name : string.Empty,
                nameof(LicenseeInfo.Number) => x => "0000" + x.Number.Length.ToString() + x.Number,
                nameof(LicenseeInfo.Status) => x => x.Status,
                nameof(LicenseeInfo.TaxiDriverAssociationName) =>
                    x => x.TaxiDriverAssociation != default ? x.TaxiDriverAssociation.Name : string.Empty,
                nameof(LicenseeInfo.Type) => x => x.Type,
                nameof(LicenseeInfo.DriverDisplayName) => x => x.LicenseesTaxiDrivers.Where(a => a.TaxiDriverType == DriverTypes.Master).Select(a => a.TaxiDriver.LastName).FirstOrDefault(),
                nameof(LicenseeInfo.VehiclePlate) =>
                    x => x.Vehicle != default ? x.Vehicle.LicensePlate : string.Empty,
                _ => x => x.Id
            };

        public static Expression<Func<LicenseeHistoryEntity, dynamic>> MapSortCriteria(this LicenseeVariationFilterCriteria lfc) =>
            lfc.KeySelector.FirstCharToUpper() switch
            {
                nameof(LicenseeHistory.Acronym) => x => x.Acronym,
                nameof(LicenseeHistory.LicenseeNote) => x => x.LicenseeNote,
                nameof(LicenseeHistory.CarFuelType) => x => x.VehicleCarFuelType,
                nameof(LicenseeHistory.Number) => x => x.Number,
                nameof(LicenseeHistory.Status) => x => x.Status,
                nameof(LicenseeHistory.TaxiDriverAssociationName) => x => x.TaxiDriverAssociationName,
                nameof(LicenseeHistory.Type) => x => x.Type,
                nameof(LicenseeHistory.VariationEndDate) => x => x.SysEndTime,
                nameof(LicenseeHistory.Note) => x => x.Note,
                nameof(LicenseeHistory.StartDate) => x => x.SysStartTime,
                nameof(LicenseeHistory.LicenseeId) => x => x.LicenseeId,
                _ => x => x.Id
            };

        public static LicenseeHistory Map(this LicenseeHistoryEntity lh, Dictionary<long, string> subShifts, Dictionary<long, string> shifts)
        {
            return new()
            {
                Acronym = lh.Acronym,
                LicenseeId = lh.LicenseeId,
                LicenseeNote = lh.LicenseeNote,
                Note = lh.Note,
                VariationEndDate = lh.SysEndTime,
                StartDate = lh.SysStartTime,
                Status = lh.Status,
                Id = lh.Id,
                Number = lh.Number,
                Type = lh.Type,
                TaxiDriverAssociationName = lh.TaxiDriverAssociationName,
                GarageAddress = lh.GarageAddress,
                IsFinancialAdministration = lh.IsFinancialAdministration.GetValueOrDefault(),
                LicenseesIssuingOfficeDescription = lh.LicenseesIssuingOfficeDescription,
                LicenseesIssuingOfficeId = lh.LicenseesIssuingOfficeId,
                FinancialAdministrationId = lh.FinancialAdministrationId,
                IsFamilyCollaboration = lh.IsFamilyCollaboration.GetValueOrDefault(),
                ActivityExpiredOnCause = lh.ExpireActivityCause,
                CarFuelType = lh.VehicleCarFuelType.GetValueOrDefault(),
                DriverDisplayName = $"{lh.TaxiDriverLastName} {lh.TaxiDriverFirstName}",
                ReleaseDate = lh.ReleaseDate,
                VehiclePlate = lh.VehicleLicensePlate,
                SubShiftName = lh.SubShiftId.HasValue && subShifts.ContainsKey(lh.SubShiftId.Value) ? subShifts[lh.SubShiftId.Value] : string.Empty,
                ShiftName = lh.ShiftId.HasValue && shifts.ContainsKey(lh.ShiftId.Value) ? shifts[lh.ShiftId.Value] : string.Empty,
            };
        }

        public static LicenseesIssuingOffice Map(this LicenseesIssuingOfficeEntity officeEntity) =>
            new()
            {
                Description = officeEntity.Description,
                Id = officeEntity.Id
            };

        public static Expression<Func<LicenseesIssuingOfficeEntity, dynamic>> MapSortCriteria(this LicenseesIssuingOfficesFilterCriteria lfc) =>
            lfc.KeySelector.FirstCharToUpper() switch
            {
                nameof(LicenseesIssuingOffice.Description) => x => x.Description,
                _ => x => x.Id
            };
    }
}