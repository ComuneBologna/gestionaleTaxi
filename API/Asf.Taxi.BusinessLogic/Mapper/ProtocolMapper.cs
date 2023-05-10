using Asf.Taxi.BusinessLogic.Models.ProtocolEmails;
using Asf.Taxi.DAL.Entities;
using SmartTech.Infrastructure.Extensions;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Asf.Taxi.BusinessLogic.Mapper
{
    public static class ProtocolMapper
    {
        #region ProtocolEmails
        public static bool CheckValidMail(this string mail) =>
            new Regex(@"^([0-9a-zA-Z]([-\.\'\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z\.])+[.a-zA-Z]{2,9})$", RegexOptions.IgnoreCase).IsMatch(mail);

        public static Expression<Func<EmailEntity, dynamic>> MapProtocolEmailSortCriteria(this ProtocolEmailSearchCriteria sc)
            => sc.KeySelector?.FirstCharToUpper() switch
            {
                nameof(ProtocolEmail.Description) => x => x.Description,
                nameof(ProtocolEmail.Email) => x => x.Email!,
                nameof(ProtocolEmail.Active) => x => x.Active,
                _ => x => x.Id
            };

        public static EmailEntity MapPostProtocolEmail(this ProtocolEmailWrite model, long auhtorityId)
            => new()
            {
                AuthorityId = auhtorityId,
                Description = model.Description ?? string.Empty,
                Email = model.Email,
                Active = true
            };

        public static void MapUpdateProtocolEmail(this EmailEntity entity, ProtocolEmailUpdate model)
        {
            entity.Email = model.Email;
            entity.Active = model.Active!.Value;
            entity.Description = model!.Description ?? string.Empty;
        }

        public static T? MapGetProtocolEmail<T>(this EmailEntity entity) where T : ProtocolEmailBase
            => typeof(T).Equals(typeof(ProtocolEmail)) ?
            new ProtocolEmail
            {
                Active = entity.Active,
                Description = entity.Description ?? string.Empty,
                Email = entity.Email,
                Id = entity.Id,
            } as T : typeof(T).Equals(typeof(ProtocolEmailInfo)) ?
            new ProtocolEmailInfo
            {
                Email = entity.Email,
                Id = entity.Id
            } as T : new ProtocolEmailBase
            {
                Email = entity.Email
            } as T;
        #endregion
    }
}
