using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTech.Common.API;
using SmartTech.Common.ApplicationUsers;
using SmartTech.Common.ApplicationUsers.API;
using SmartTech.Common.ApplicationUsers.Models;
using SmartTech.Common.Authentication;
using SmartTech.Common.Web.Security;
using SmartTech.Infrastructure.Exceptions;

namespace Asf.Taxi.API.Controllers
{
    public class ProfilesController : APIControllerBase
    {
        readonly IUserContext _userContext;
        readonly IUsersService _userService;
        readonly IAuthorityService _authorityService;

        public ProfilesController(IUserContext userContext, IUsersService usersService, IAuthorityService authorityService)
        {
            _userContext = userContext;
            _userService = usersService;
            _authorityService = authorityService;
        }

        [Authorize]
        [HttpGet("authorities")]
        [SkipAuthorityId]
        public async Task<IEnumerable<AuthorityExtended>> GetAuthorities() => await _authorityService.GetAuthorities<AuthorityExtended>();

        [HttpPut("authorities/{authorityId}")]
        [Authorize]
        [SkipAuthorityId]
        public async Task SetAuthorityAsDefault(long authorityId) => await _authorityService.SetAsDefault(authorityId);

        [Authorize]
        [HttpGet("permissions")]
        public async Task<IEnumerable<string>> GetMyPermissions() =>
            await Task.FromResult(User.Claims.Where(w => w.Type == "permission").Select(s => s.Value).ToList());

        [SkipAuthorityId]
        [Authorize]
        [HttpGet]
        public async Task<Profile?> GetMyProfile() => await _userService.GetProfile<Profile>();

        [SkipAuthorityId]
        [Authorize]
        [HttpPost("avatar")]
        public async Task<AvatarResponse> UploadAvatar([FromForm] IFormFile file)
        {
            _ = (await _userService.GetUsers<UsersSearchCriteria, ApplicationUser>(new UsersSearchCriteria
            {
                SmartPAUserId = _userContext.SmartPAUserId
            })).Items.FirstOrDefault()
            ?? throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);

            var attachment = (file ?? (await Request.ReadFormAsync()).Files[0]).Map();

            _ = await _userService.UploadAvatar(attachment);

            return new AvatarResponse()
            {
                Url = await _userService.DownloadAvatar()
            };
        }

        [SkipAuthorityId]
        [Authorize]
        [HttpDelete("avatar")]
        public async Task DeleteAvatar()
        {
            _ = (await _userService.GetUsers<UsersSearchCriteria, ApplicationUser>(new UsersSearchCriteria
            {
                SmartPAUserId = _userContext.SmartPAUserId
            })).Items.FirstOrDefault()
            ?? throw new BusinessLogicValidationException(BusinessLogicValidationExceptionScopes.NotFound);

            await _userService.DeleteAvatar();
        }
    }
}