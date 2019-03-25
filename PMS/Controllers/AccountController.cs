﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PMS.Infrastructure;
using PMS.Infrastructure.Cache;
using PMS.Infrastructure.Model;
using PMS.Repository.Domain;
using PMS.Services.Interfaces;

namespace PMS.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController(ICacheContext cacheContext, IUserService userService) : base(cacheContext, userService)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetUserName()
        {
            //var user = _cacheContext.Get<SysUser>(Account);
            return Json(CurrentUser.Name);
        }

        public string GetUserList(QueryUserReq request)
        {
            request.fullpath = CurrentUser.OrgFullPath;
            return JsonHelper.Instance.Serialize(_userService.GetUserList(request));
        }
    }
}