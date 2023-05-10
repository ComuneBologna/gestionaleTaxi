using Asf.Taxi.DAL.Entities;
using Asf.Taxi.DAL.Enums;

namespace Asf.Taxi.BusinessLogic.Mapper
{
    public static class VariationMapper
    {
        public static LicenseeHistoryEntity Map(this LicenseeEntity lh, string variationNote, DateTime sysEndTime)
            => new()
            {
                Acronym = lh.Acronym,
                LicenseeId = lh.Id,
                LicenseeNote = lh.Note,
                Note = variationNote,
                SysEndTime = sysEndTime,
                SysStartTime = lh.SysStartTime,
                Status = lh.Status,
                Number = lh.Number,
                Type = lh.Type,
                TaxiDriverAssociationName = lh.TaxiDriverAssociation?.Name ?? string.Empty,
                GarageAddress = lh.GarageAddress,
                IsFinancialAdministration = lh.IsFinancialAdministration,
                LicenseesIssuingOfficeDescription = lh.LicenseesIssuingOffice?.Description ?? string.Empty,
                LicenseesIssuingOfficeId = lh.LicenseesIssuingOfficeId,
                IsFamilyCollaboration = lh.IsFamilyCollaboration,
                AuthorityId = lh.AuthorityId,
                EndDate = lh.EndDate,
                ExpireActivityCause = lh.ExpireActivityCause,
                FinancialAdministrationId = lh.FinancialAdministration?.Id ?? null,
                ReleaseDate = lh.ReleaseDate,
                ShiftId = lh.ShiftId,
                SubShiftId = lh.SubShiftId,
                TaxiDriverAssociationFiscalCode = lh.TaxiDriverAssociation?.FiscalCode ?? string.Empty,
                TaxiDriverAssociationId = lh.TaxiDriverAssociationId,
                VariationType = VariationTypes.LicenseesVariation,
                VehicleCarFuelType = lh.Vehicle?.CarFuelType ?? null,
                VehicleId = lh.Vehicle?.Id ?? null,
                VehicleInUseSince = lh.Vehicle?.InUseSince ?? null,
                VehicleLicensePlate = lh.Vehicle?.LicensePlate ?? string.Empty,
                VehicleModel = lh.Vehicle?.Model ?? string.Empty,
                VehicleRegistrationDate = lh.Vehicle?.RegistrationDate ?? null,
                FolderId = lh.FolderId ?? null,
            };

        public static LicenseeHistoryEntity Map(this VehicleEntity vh, string variationNote, DateTime sysEndTime)
        {
            return new()
            {
                Acronym = vh.Licensee?.Acronym ?? string.Empty,
                LicenseeId = vh.LicenseeId,
                LicenseeNote = vh.Licensee?.Note ?? string.Empty,
                Note = variationNote,
                SysEndTime = sysEndTime,
                SysStartTime = vh.SysStartTime,
                Status = vh.Licensee?.Status ?? null,
                Number = vh.Licensee?.Number ?? string.Empty,
                Type = vh.Licensee?.Type ?? null,
                TaxiDriverAssociationName = vh.Licensee?.TaxiDriverAssociation?.Name ?? string.Empty,
                GarageAddress = vh.Licensee?.GarageAddress ?? string.Empty,
                IsFinancialAdministration = vh.Licensee?.IsFinancialAdministration ?? null,
                LicenseesIssuingOfficeDescription = vh.Licensee?.LicenseesIssuingOffice?.Description ?? string.Empty,
                LicenseesIssuingOfficeId = vh.Licensee?.LicenseesIssuingOfficeId ?? null,
                IsFamilyCollaboration = vh.Licensee?.IsFamilyCollaboration ?? null,
                AuthorityId = vh.AuthorityId,
                EndDate = vh.Licensee?.EndDate ?? null,
                ExpireActivityCause = vh.Licensee?.ExpireActivityCause ?? string.Empty,
                FinancialAdministrationId = vh.Licensee?.FinancialAdministration?.Id ?? null,
                ReleaseDate = vh.Licensee?.ReleaseDate ?? null,
                ShiftId = vh.Licensee?.ShiftId ?? null,
                SubShiftId = vh.Licensee?.SubShiftId ?? null,
                TaxiDriverAssociationFiscalCode = vh.Licensee?.TaxiDriverAssociation?.FiscalCode ?? string.Empty,
                TaxiDriverAssociationId = vh.Licensee?.TaxiDriverAssociationId ?? null,
                VariationType = VariationTypes.VehiclesVariation,
                VehicleCarFuelType = vh.CarFuelType,
                VehicleId = vh.Id,
                VehicleInUseSince = vh.InUseSince,
                VehicleLicensePlate = vh.LicensePlate,
                VehicleModel = vh.Model ?? string.Empty,
                VehicleRegistrationDate = vh.RegistrationDate
            };
        }

        public static LicenseeHistoryEntity Map(this TaxiDriverEntity tdh, string variationNote, DateTime sysEndTime)
        {
            var licensee = tdh?.LicenseesTaxiDrivers?.FirstOrDefault()?.Licensee ?? new LicenseeEntity();

            return new()
            {
                Acronym = licensee?.Acronym ?? string.Empty,
                LicenseeId = licensee?.Id ?? 0,
                LicenseeNote = licensee?.Note ?? string.Empty,
                Note = variationNote,
                SysEndTime = sysEndTime,
                SysStartTime = tdh.SysStartTime,
                Status = licensee?.Status ?? null,
                Number = licensee?.Number ?? string.Empty,
                Type = licensee?.Type ?? null,
                TaxiDriverAssociationName = licensee?.TaxiDriverAssociation?.Name ?? string.Empty,
                GarageAddress = licensee?.GarageAddress ?? string.Empty,
                IsFinancialAdministration = licensee?.IsFinancialAdministration ?? null,
                LicenseesIssuingOfficeDescription = licensee?.LicenseesIssuingOffice?.Description ?? string.Empty,
                LicenseesIssuingOfficeId = licensee?.LicenseesIssuingOfficeId ?? null,
                IsFamilyCollaboration = licensee?.IsFamilyCollaboration ?? null,
                AuthorityId = tdh.AuthorityId,
                EndDate = licensee?.EndDate ?? null,
                ExpireActivityCause = licensee?.ExpireActivityCause ?? string.Empty,
                FinancialAdministrationId = licensee?.FinancialAdministration?.Id ?? null,
                ReleaseDate = licensee?.ReleaseDate ?? null,
                ShiftId = licensee?.ShiftId ?? null,
                SubShiftId = licensee?.SubShiftId ?? null,
                TaxiDriverAssociationFiscalCode = licensee?.TaxiDriverAssociation?.FiscalCode ?? string.Empty,
                TaxiDriverAssociationId = licensee?.TaxiDriverAssociationId ?? null,
                VariationType = VariationTypes.TaxiDriversVariation,
                VehicleCarFuelType = licensee?.Vehicle?.CarFuelType ?? null,
                VehicleId = licensee?.Vehicle?.Id ?? null,
                VehicleInUseSince = licensee?.Vehicle?.InUseSince ?? null,
                VehicleLicensePlate = licensee?.Vehicle?.LicensePlate ?? string.Empty,
                VehicleModel = licensee?.Vehicle?.Model ?? string.Empty,
                VehicleRegistrationDate = licensee?.Vehicle?.RegistrationDate ?? null,
                TaxiDriverCollaborationType = tdh.CollaborationType,
                TaxiDriverFirstName = tdh.FirstName,
                TaxiDriverFiscalCode = tdh.FiscalCode,
                TaxiDriverId = tdh.Id,
                TaxiDriverLastName = tdh.LastName,
                TaxiDriverShiftStartHour = tdh.ShiftStartHour,
                TaxiDriverShiftStartMinutes = tdh.ShiftStartMinutes,
                TaxiDriverType = tdh?.LicenseesTaxiDrivers?.FirstOrDefault()?.TaxiDriverType ?? null,
                TaxiDriverPersonType = tdh?.Type
            };
        }
    }
}