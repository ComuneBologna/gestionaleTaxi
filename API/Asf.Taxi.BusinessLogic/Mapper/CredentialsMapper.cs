using Asf.Taxi.BusinessLogic.Models;
using Asf.Taxi.DAL.Entities;
using SmartTech.Common.Models;
using SmartTech.Infrastructure.Extensions;
using System.Linq.Expressions;

namespace Asf.Taxi.BusinessLogic.Mapper
{
    internal static class CredentialsMapper
    {
        internal static CredentialEntity Map(this DigitalSignCredential model, string firstName, string lastName, long authorityId, Guid userId, string encryptedPassword) =>
            new()
            {
                AuthorityId = authorityId,
                FirstName = firstName,
                Password = encryptedPassword,
                UserId = userId,
                LastName = lastName,
                Username = model.Username
            };

        internal static void Map(this CredentialEntity entity, string username, string encryptedPassword)
        {
            entity.Username = username;
            entity.Password = encryptedPassword;
        }

        internal static Expression<Func<CredentialEntity, dynamic>> MapSortCriteria(this CredentialUserFilterCriteria criteria) =>
             criteria.KeySelector?.FirstCharToUpper() switch
             {
                 nameof(CredentialUser.FirstName) => x => x.FirstName!,
                 nameof(CredentialUser.LastName) => x => x.LastName!,
                 _ => x => x.Id
             };

        internal static CredentialUser Map(this CredentialEntity entity) =>
            new()
            {
                FirstName = entity.FirstName,
                LastName = entity.LastName
            };
    }
}