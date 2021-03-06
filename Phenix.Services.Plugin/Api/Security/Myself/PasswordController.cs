﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Phenix.Actor;
using Phenix.Core.Data.Schema;
using Phenix.Services.Plugin.Actor.Security;

namespace Phenix.Services.Plugin.Api.Security.Myself
{
    /// <summary>
    /// 登录口令控制器
    /// </summary>
    [Route(Phenix.Core.Net.Api.ApiConfig.ApiSecurityMyselfPasswordPath)]
    [ApiController]
    public sealed class PasswordController : Phenix.Core.Net.Api.ControllerBase
    {
        // phAjax.changePassword()
        /// <summary>
        /// 修改登录口令
        /// </summary>
        [Authorize]
        [HttpPut]
        public async Task<bool> Put()
        {
            string[] passwords = (await Request.ReadBodyAsStringAsync(true)).Split(Standards.RowSeparator);
            return await ClusterClient.Default.GetGrain<IUserGrain>(User.Identity.Name).ChangePassword(passwords[0], passwords[1], Request.GetRemoteAddress(), true);
        }
    }
}