using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPI.Entities;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdministratorController : ControllerBase
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly ILogger _log;
        public AdministratorController(RoleManager<Role> roleManager, UserManager<User> userManager, AppDbContext context, ILogger<AdministratorController> log)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _log = log;
        }

        /// <summary>
        /// Получение всех ролей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getroles")]
        public IActionResult GetRoles()
        {
            //Получаем все роли, кроме роли администратора
            var roles = _roleManager.Roles.Where(r => r.Name != "Admin").ToList();
            var rolesModel = GetRoleViewModel(roles);
            return Ok(rolesModel);
        }

        /// <summary>
        /// Получение всех пользователей
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getusers")]
        public async Task<IActionResult> GetUsers()
        {
            var usersAndRoles = new List<UserAndRoleModel>();
            //Получаю всех пользователей
            var users = _userManager.Users.Where(u => u.UserName != "Administrator");
            //Получаю все роли
            var roles = _roleManager.Roles.Select(r => r.Name).Where(r => r != "Admin").ToList();
            foreach (var user in users)
            {
                usersAndRoles.Add(new UserAndRoleModel()
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    Email = user.Email
                });
            }

            //Для каждого пользователя узнаю какие у него роли
            foreach (var user in usersAndRoles)
            {
                user.RoleNames = await _userManager.GetRolesAsync(_userManager.Users.FirstOrDefault(u => u.UserName == user.UserName)) as List<string>;
            }

            return Ok(new { usersAndRoles = usersAndRoles, roles = roles });
        }

        private List<RoleViewModel> GetRoleViewModel(List<Role> roles)
        {
            var roleViewModel = new List<RoleViewModel>();
            foreach (var role in roles)
            {
                roleViewModel.Add(new RoleViewModel()
                {
                    Id = role.Id,
                    RoleName = role.Name,
                    DisplayName = role.DisplayName,
                    CanCreate = role.CanCreate,
                    CanEdit = role.CanEdit,
                    CanPrint = role.CanPrint,
                    CanShow = role.CanShow
                });
            }
            return roleViewModel;
        }

        /// <summary>
        /// Добавление ролей к пользователю
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("addusertorole")]
        public async Task<IActionResult> AddUserToRole([FromBody]UserWithIncludedAndExcludedRolesModel model)
        {
            //Поиск пользователя
            var user = _userManager.FindByNameAsync(model.UserName).Result;
            if (user != null)
            {
                //Добавление ролей пользователю
                await _userManager.AddToRolesAsync(user, model.IncludedRoleNames);
                await _userManager.RemoveFromRolesAsync(user, model.ExcludedRoleNames);
                return Ok();
            }
            return BadRequest();
        }
    }
}