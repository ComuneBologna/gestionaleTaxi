using SmartTech.Common.ApplicationUsers.Entities;
using SmartTech.Common.ApplicationUsers.Models;
using SmartTech.Common.Models;
using SmartTech.Infrastructure.Extensions;
using System.Linq.Expressions;
using SmartTech.Common.ApplicationUsers;
using Asf.Taxi.DAL.Entities;
using Asf.Taxi.BusinessLogic.Models;

namespace Asf.Taxi.BusinessLogic.Mapper
{
	public class UsersMapper : IUsersMapper
	{
		public User Map<TWrite>(TWrite user)
					where TWrite : UserWrite
					=> new()
					{
						Email = user.Email!,
						Firstname = user.FirstName!,
						Surname = user.LastName!,
						FiscalCode = user.FiscalCode!,
						PhoneNumber = user.PhoneNumber
					};

		public TAuthorityUserEntity Map<TWrite, TAuthorityUserEntity>(TWrite user, Guid userId, long authorityId)
			where TWrite : UserWrite
			where TAuthorityUserEntity : AuthorityUserEntity
			=> new TaxiAuthorityUserEntity
			{
				AuthorityId = authorityId,
				DriverId = (user as BackofficeUserWrite).DriverId,
				IsEnabled = true,
				Permissions = new List<UserPermissionEntity>()
						{
							new UserPermissionEntity()
							{
								PermissionCode = user.PermissionCode!
							}
						},
				SmartPAUserId = userId
			} as TAuthorityUserEntity;

		public TResult? Map<TUserEntity, TResult>(TUserEntity? user, long authorityId)
			where TUserEntity : UserEntity
			where TResult : ApplicationUser
		{
			if (user == default)
				return default;

			var au = user.Authorities.FirstOrDefault(a => a.AuthorityId == authorityId);

			return new ApplicationUser
			{
				Id = user.SmartPAUserId,
				IsEnabled = au?.IsEnabled,
				PhoneNumber = user.PhoneNumber,
				Email = user.Email,
				FirstName = user.FirstName,
				LastName = user.LastName,
				FiscalCode = user.FiscalCode,
				PermissionCode = au?.Permissions?.Select(p => p.PermissionCode).FirstOrDefault()
			} as TResult;
		}

		public TResult MapResult<TAuthorityUserEntity, TResult>(TAuthorityUserEntity entity)
			where TAuthorityUserEntity : AuthorityUserEntity
			where TResult : UserBase
		{
			if (typeof(TResult).Equals(typeof(BackofficeUser)))
				return new BackofficeUser
				{
					Id = entity.SmartPAUserId,
					IsEnabled = entity.IsEnabled,
					PhoneNumber = entity.User.PhoneNumber,
					Email = entity.User.Email,
					FirstName = entity.User.FirstName,
					LastName = entity.User.LastName,
					FiscalCode = entity.User.FiscalCode,
					PermissionCode = entity.Permissions.Select(p => p.PermissionCode).FirstOrDefault()
				} as TResult;
			else
				return new UserBase
				{
					PhoneNumber = entity.User.PhoneNumber,
					Email = entity.User.Email,
					FirstName = entity.User.FirstName,
					LastName = entity.User.LastName,
					FiscalCode = entity.User.FiscalCode
				} as TResult;
		}

		public TProfileResult? MapResult<TUserEntity, TProfileResult>(TUserEntity entity, bool isSpidUser, string avatarUrl)
			where TUserEntity : UserEntity
			where TProfileResult : UserBase =>
			entity == default ? default :
			typeof(TProfileResult).Equals(typeof(Profile)) ?
				new Profile
				{
					Id = entity.SmartPAUserId,
					IsSpidUser = isSpidUser,
					PhoneNumber = entity.PhoneNumber,
					Email = entity.Email,
					FirstName = entity.FirstName,
					LastName = entity.LastName,
					FiscalCode = entity.FiscalCode,
					AvatarUrl = avatarUrl
				} as TProfileResult :
				new UserBase
				{
					PhoneNumber = entity.PhoneNumber,
					Email = entity.Email,
					FirstName = entity.FirstName,
					LastName = entity.LastName,
					FiscalCode = entity.FiscalCode
				} as TProfileResult;

		public TResult MapResult<T, TResult>(T user, string permissionCode)
			where T : UserInfo
			where TResult : ApplicationUser =>
			new ApplicationUser
			{
				Id = user.Id,
				IsEnabled = user.Enabled,
				Email = user.Email,
				FirstName = user.Name,
				LastName = user.Surname,
				FiscalCode = user.FiscalCode,
				PhoneNumber = user.PhoneNumber,
				PermissionCode = permissionCode,
			} as TResult;

		public Expression<Func<TAuthorityUserEntity, dynamic>> MapSortCriteria<TSearch, TAuthorityUserEntity>(TSearch searchCriteria)
			where TSearch : UsersSearchCriteria
			where TAuthorityUserEntity : AuthorityUserEntity =>
			searchCriteria.KeySelector.FirstCharToUpper() switch
			{
				nameof(ApplicationUser.Email) => x => x.User.Email,
				nameof(ApplicationUser.FirstName) => x => x.User.FirstName,
				nameof(ApplicationUser.FiscalCode) => x => x.User.FiscalCode,
				nameof(ApplicationUser.IsEnabled) => x => x.IsEnabled,
				nameof(ApplicationUser.LastName) => x => x.User.LastName,
				nameof(ApplicationUser.PhoneNumber) => x => x.User.PhoneNumber,
				nameof(ApplicationUser.PermissionCode) => x => x.Permissions.First(),
				_ => x => x.SmartPAUserId
			};

		public TUserEntity MapUpdate<TUpdate, TUserEntity>(TUserEntity entity, TUpdate user)
			where TUpdate : UserUpdate
			where TUserEntity : UserEntity
		{
			entity.Email = user.Email!;
			entity.FiscalCode = user.FiscalCode!;
			entity.LastName = user.LastName!;
			entity.FirstName = user.FirstName!;
			entity.PhoneNumber = user.PhoneNumber!;
			entity.Authorities.ForEach(au =>
			{
				((TaxiAuthorityUserEntity)au).DriverId = (user as BackofficeUserUpdate).DriverId;
				au.IsEnabled = user.IsEnabled!.Value;
				au.Permissions.ForEach(p => p.PermissionCode = user.PermissionCode!);
			});

			return entity;
		}

		public TUserEntity MapWrite<TWrite, TUserEntity>(TWrite user, Guid userId, Guid tenantId, long authorityId)
			where TWrite : UserWrite
			where TUserEntity : UserEntity =>
			new UserEntity
			{
				Authorities = new List<AuthorityUserEntity>
				{
					Map<TWrite, TaxiAuthorityUserEntity>(user, userId, authorityId)
				},
				Email = user.Email!,
				FirstName = user.FirstName!,
				FiscalCode = user.FiscalCode!,
				LastName = user.LastName!,
				PhoneNumber = user.PhoneNumber,
				SmartPAUserId = userId,
				TenantId = tenantId
			} as TUserEntity;
	}
}