using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using WebAPI.Entities;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ILogger _log;
        public AuthenticationController(
            IConfiguration configuration,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            ILogger<AuthenticationController> log)
        {
            _configuration = configuration;
            _userManager = userManager;
            _log = log;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous] //Явно разрешаем доступ к методу действия
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
        {
            //Поиск пользователя по имени
            var user = await _userManager.FindByNameAsync(loginModel.UserName);
            //Если пользователь найден и пароль корректен
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                _log.LogInformation($"Вход пользователя {user.UserName} в систему");
                //Поиск роли у пользователя
                var roles = await _userManager.GetRolesAsync(user);
                //Получаем JWT и отправляем его пользователю
                return Ok(new { token = GetToken(user), userName = user.UserName.ToString(), userRole = new JsonResult(roles) });
            }
            _log.LogInformation($"Попытка войти в систему с логином: {loginModel.UserName} и паролем {loginModel.Password}");
            return Unauthorized();
        }


        [HttpPost]
        [Route("registration")]
        [AllowAnonymous]
        public async Task<IActionResult> Registration([FromBody] RegisterModel registerModel)
        {
            //Создаем нового пользователя
            User user = new User()
            {
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                FirstName = registerModel.FirstName
            };
            try
            {
                //Создаем нового пользоателя
                await _userManager.CreateAsync(user, registerModel.Password);
                _log.LogInformation($"Регистрация нового пользователя {user.UserName} системе");
                return Ok(new { username = user.UserName, email = user.Email, status = 1});
            }
            catch (Exception ex)
            {
                _log.LogInformation($"Ошибка при регистрации: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Генерация JWT
        /// </summary>
        /// <param name="user">Пользователь, для когторого JWT генерируется</param>
        /// <returns></returns>
        private string GetToken(User user)
        {
            //Получаем список ролей для пользователя
            var roles = _userManager.GetRolesAsync(user).Result;

            //Создаем утверждения для токена
            //Подготавливаем набор утверждений (claim), который будет сохранен в блоке PAYLOAD токена
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserName)); //запишем имя пользователя как его идентификатор

            //Запишем роли пользователя
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //Ключ для проверки подписи
            var key = _configuration.GetValue<string>("Tokens:Key");
            var siginingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(siginingKey, SecurityAlgorithms.HmacSha256);

            //Генерируем JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["Tokens:Issuer"], //имя приложения, сгенерировавшего JWT
                audience: _configuration["Tokens:Audience"], //для кого был сгенерирован JWT
                claims: claims, //Утверждения
                expires: DateTime.Now.AddHours(_configuration.GetValue<double>("Tokens:Expires")), //дата, после который JWT будет считаться не валидным и клиенту потребуется сгенерировать новый JWT
                signingCredentials: signingCredentials //определяет способ создания цифровой подписи, которая подтверждает валидность JWT
                );

            //Генерируем строку с токеном
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
    }
}

