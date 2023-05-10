using Asf.Taxi.BusinessLogic.Models.LicenseesIssuingOffice;
using Asf.Taxi.DAL.Entities;

namespace Asf.Taxi.BusinessLogic.Mapper
{
	public static class LicenseesIssuingOfficeMapper
	{
		public static LicenseesIssuingOfficeEntity Map(this LicenseesIssuingOfficeWrite write, long authorityId) =>
			new()
			{
				AuthorityId = authorityId,
				Description = write.Description
			};
		
		public static void Map(this LicenseesIssuingOfficeEntity entity, LicenseesIssuingOfficeWrite write, long authorityId)
		{

			entity.AuthorityId = authorityId;
			entity.Description = write.Description;
		}
	}
}
