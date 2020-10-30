using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0.Core.Exceptions;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using Wave.Dtos;
using Wave.Models;

namespace WaveApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AdminController : ControllerBase
    {
        private readonly ManagementApiClient _apiClient;
        private readonly IOptions<Auth0ApiConfig> _config;
        public AdminController(ManagementApiClient apiClient, IOptions<Auth0ApiConfig> config)
        {
            _apiClient = apiClient ?? throw new NullReferenceException();
            _config = config ?? throw new NullReferenceException();
        }

        [Authorize(Policy = "read:admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUser([FromQuery] int from = 0, [FromQuery] int take = 50)
        {
            try
            {
                var users = await _apiClient.Users.GetAllAsync(new GetUsersRequest(), new PaginationInfo(from, take));
                return Ok(users);
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - GetAllUser");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "read:admin")]
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var user = await _apiClient.Users.GetAsync(id);
                return Ok(user);
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - GetUser");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "write:admin")]
        [HttpPost("users/{id}/role")]
        public async Task<IActionResult> AddRoleToUsers(string id, [FromBody] IdsDto dto)
        {
            try
            {
                await _apiClient.Users.AssignRolesAsync(id, new AssignRolesRequest { Roles = dto.Ids });
                return Ok();
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - AddRoleToUsers");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "remove:admin")]
        [HttpDelete("users/{id}/role")]
        public async Task<IActionResult> RemoveRoleFromUsers(string id, [FromBody] IdsDto dto)
        {
            try
            {
                await _apiClient.Users.RemoveRolesAsync(id, new AssignRolesRequest { Roles = dto.Ids });
                return Ok();
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - RemoveRoleFromUsers");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "read:admin")]
        [HttpGet("roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _apiClient.Roles.GetAllAsync(new Auth0.ManagementApi.Models.GetRolesRequest());
                return Ok(roles);
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - GetAllRoles");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "read:admin")]
        [HttpGet("roles/{id}")]
        public async Task<IActionResult> GetRole(string id)
        {
            try
            {
                var role = await _apiClient.Roles.GetAsync(id);
                return Ok(role);
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - GetRole");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "write:admin")]
        [HttpPost("roles")]
        public async Task<IActionResult> CreateRole([FromBody] RoleDto dto)
        {
            try
            {
                var role = await _apiClient.Roles.CreateAsync(new Auth0.ManagementApi.Models.RoleCreateRequest { Name = dto.Name, Description = dto.Description });
                return Ok(role);
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - CreateRole");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "modify:admin")]
        [HttpPut("roles/{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleDto dto)
        {
            try
            {
                var role = await _apiClient.Roles.UpdateAsync(id, new Auth0.ManagementApi.Models.RoleUpdateRequest { Name = dto.Name, Description = dto.Description });
                return Ok(role);
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - UpdateRole");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "remove:admin")]
        [HttpDelete("roles/{id}")]
        public async Task<IActionResult> RemoveRole(string id)
        {
            try
            {
                await _apiClient.Roles.DeleteAsync(id);
                return Ok();
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - RemoveRole");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "read:admin")]
        [HttpGet("roles/{id}/users")]
        public async Task<IActionResult> GetUserWithRole(string id)
        {
            try
            {
                var res = await _apiClient.Roles.GetUsersAsync(id);
                return Ok(res);
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - GetUserWithPermission");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "read:admin")]
        [HttpGet("roles/{id}/permission")]
        public async Task<IActionResult> GetRolePermissions(string id)
        {
            try
            {
                var ret = await _apiClient.Roles.GetPermissionsAsync(id, new Auth0.ManagementApi.Paging.PaginationInfo());
                return Ok(ret);
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - GetRolePermissions");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "write:admin")]
        [HttpPost("roles/{id}/permission/{value}")]
        public async Task<IActionResult> AddPermissionToRole(string id, string value)
        {
            try
            {
                var perms = new List<PermissionIdentity>();
                perms.Add(new PermissionIdentity { Identifier = _config.Value.Identifier, Name = value });
                await _apiClient.Roles.AssignPermissionsAsync(id, new AssignPermissionsRequest { Permissions = perms });
                return Ok();
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - AddPermissionToRole");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "remove:admin")]
        [HttpDelete("roles/{id}/permission/{value}")]
        public async Task<IActionResult> RemovePermissionFromRole(string id, string value)
        {
            try
            {
                var perms = new List<PermissionIdentity>();
                perms.Add(new PermissionIdentity { Identifier = _config.Value.Identifier, Name = value });
                await _apiClient.Roles.RemovePermissionsAsync(id, new AssignPermissionsRequest { Permissions = perms });
                return Ok();
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - RemovePermissionFromRole");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "read:admin")]
        [HttpGet("permissions")]
        public async Task<IActionResult> GetApiScopes()
        {
            try
            {
                var res = await _apiClient.ResourceServers.GetAsync(this._config.Value.ApiId);
                return Ok(res.Scopes);
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - GetApiScopes");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "write:admin")]
        [HttpPost("permissions")]
        public async Task<IActionResult> CreateApiScope([FromBody] RoleDto dto)
        {
            try
            {
                var res = await _apiClient.ResourceServers.GetAsync(_config.Value.ApiId);
                res.Scopes.Add(new Auth0.ManagementApi.Models.ResourceServerScope { Value = dto.Name, Description = dto.Description });
                var val = await _apiClient.ResourceServers.UpdateAsync(_config.Value.ApiId, new Auth0.ManagementApi.Models.ResourceServerUpdateRequest { Scopes = res.Scopes });
                return Ok(val.Scopes);
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - CreateApiScope");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "modify:admin")]
        [HttpPut("permissions/{value}")]
        public async Task<IActionResult> ModifyApiScope(string value, [FromBody] RoleDto dto)
        {
            try
            {
                var res = await _apiClient.ResourceServers.GetAsync(_config.Value.ApiId);
                var scope = res.Scopes.Where(q => q.Value == value).SingleOrDefault();
                if (scope is null)
                    return Ok(res.Scopes);
                scope.Value = dto.Name;
                scope.Description = dto.Description;
                var val = await _apiClient.ResourceServers.UpdateAsync(_config.Value.ApiId, new Auth0.ManagementApi.Models.ResourceServerUpdateRequest { Scopes = res.Scopes });
                return Ok(val.Scopes);
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - ModifyApiScope");
                return BadRequest(e.Message);
            }
        }

        [Authorize(Policy = "remove:admin")]
        [HttpDelete("permissions/{value}")]
        public async Task<IActionResult> RemoveApiScope(string value)
        {
            try
            {
                var res = await _apiClient.ResourceServers.GetAsync(_config.Value.ApiId);
                var scope = res.Scopes.Where(q => q.Value == value).SingleOrDefault();
                if (scope is null)
                    return Ok(res.Scopes);
                res.Scopes = res.Scopes.Where(q => q.Value != value).ToList();
                await _apiClient.ResourceServers.UpdateAsync(_config.Value.ApiId, new Auth0.ManagementApi.Models.ResourceServerUpdateRequest { Scopes = res.Scopes });
                return Ok();
            }
            catch (ErrorApiException e)
            {
                Log.Error(e, "Admin - RemoveApiScope");
                return BadRequest(e.Message);
            }
        }

    }
}