﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PMS.Infrastructure.Auth;
using PMS.Infrastructure.Enums;
using PMS.Infrastructure.Response;
using PMS.Infrastructure.ViewModel;
using PMS.Services.Interfaces;

namespace IdentityServer.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _userService;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IEventService _events;
        public AccountController(IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IAuthenticationSchemeProvider schemeProvider,
            IEventService events,
            IUserService userService)
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _userService = userService;
        }

        /// <summary>
        /// 登录页面
        /// </summary>
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// 登录post回发处理
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            var loginResult = _userService.UserLogin(model);
            if (loginResult.Code == (int)EResponse.Ok)
            {
                AuthenticationProperties props = new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(1))
                };

                var claim = new Claim[]
                {
                    new Claim("RoleId", (loginResult.Payload.RoleId??0).ToString()),
                    new Claim("OrgId", (loginResult.Payload.OrgId ?? 0).ToString()),
                    new Claim("RealName", loginResult.Payload.Name??string.Empty),
                };

                await HttpContext.SignInAsync(loginResult.Payload.Id.ToString(), loginResult.Payload.Account, props, claim);
                //HttpContext.User = new LoginUserPrincipal(loginResult.Payload.Account);
                
                if (returnUrl != null)
                {
                    return Redirect(returnUrl);
                }

                return View();
            }
            else
            {
                OperateResult result = new OperateResult()
                {
                    Code = (int)EResponse.NotFound,
                    Message = "用户名或密码错误"
                };
                return View(result);
            }
        }
    }
}