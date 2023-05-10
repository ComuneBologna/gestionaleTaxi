using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.DAL.Entities;

namespace Asf.Taxi.BusinessLogic.Mapper
{
	public static class AuditsMapper
	{
		public static AuditEntity Map(this AuditWrite audit, long authorityId, Guid userId, string username) =>
			new()
			{
				AuthorityId = authorityId,
				CreatedAt = DateTime.UtcNow,
				ItemId = audit.ItemId.Value,
				ItemType = audit.ItemType.Value,
				MemoLine = audit.MemoLine,
				OldItemPath = audit.OldPathItem,
				OperationType = audit.OperationType.Value,
				UserId = userId,
				Username = username
			};

		public static AuditInfo Map(this AuditEntity audit) =>
			new()
			{
				CreatedAt = audit.CreatedAt,
				ItemId = audit.ItemId,
				ItemType = audit.ItemType,
				MemoLine = audit.MemoLine,
				OperationType = audit.OperationType,
				PathOldItem = audit.OldItemPath,
				Username = audit.Username
			};
	}
}