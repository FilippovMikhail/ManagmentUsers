using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Aspose.Words;
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
    [Authorize]
    public class SubscribersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly ILogger _log;
        public SubscribersController(UserManager<User> userManager, AppDbContext context, ILogger<SubscribersController> log)
        {
            _userManager = userManager;
            _context = context;
            _log = log;
        }

        /// <summary>
        /// Создание записи об абоненте
        /// </summary>
        /// /// <param name="subscriberModel">Данные абонента</param>
        [HttpPost]
        [Route("createsubscriber")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> CreateSubscriber([FromBody]SubscriberModel subscriberModel)
        {
            try
            {
                var subscriber = NewSubscriber(subscriberModel);
                //Добавляем в базу данные об абоненте
                await _context.Subscribers.AddAsync(subscriber);
                await _context.SaveChangesAsync();
                _log.LogInformation($"Добавление нового абонента в систему");
                return Ok();
            }
            catch (Exception ex)
            {
                _log.LogInformation($"Ошибка при добавлении нового абонента в систему: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

        }

        /// <summary>
        /// Получить пользователя
        /// </summary>
        private User GetUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userName = identity.Claims.FirstOrDefault(cl => cl.Type.Contains("nameidentifier")).Value;
            //Поск пользователя
            var user = _userManager.FindByNameAsync(userName).Result;
            return user;
        }

        /// <summary>
        /// Создание модели для нового абонента для добавления его  базу данных
        /// </summary>
        /// <param name="subscriberModel"></param>
        /// <returns></returns>
        private Subscriber NewSubscriber(SubscriberModel subscriberModel)
        {
            var subscriber = new Subscriber()
            {
                Address = subscriberModel.Address,
                FIOHead = subscriberModel.FIOHead,
                FullName = subscriberModel.FullName,
                INN = subscriberModel.INN,
                Phones = subscriberModel.Phones,
                RepresentativePhones = subscriberModel.RepresentativePhones,
                ShortName = subscriberModel.ShortName,
                SubscriberRepresentative = subscriberModel.SubscriberRepresentative
            };
            return subscriber;
        }

        /// <summary>
        /// Создание модели для нового абонента для отправки его клиенту
        /// </summary>
        /// <param name="subscribers"></param>
        /// <returns></returns>
        private List<SubscriberModel> NewSubscriberModel(List<Subscriber> subscribers)
        {
            var subscriberModel = new List<SubscriberModel>();
            foreach (var subscriber in subscribers)
            {
                subscriberModel.Add(new SubscriberModel()
                {
                    Id = subscriber.Id,
                    Address = subscriber.Address,
                    FIOHead = subscriber.FIOHead,
                    FullName = subscriber.FullName,
                    INN = subscriber.INN,
                    Phones = subscriber.Phones,
                    RepresentativePhones = subscriber.RepresentativePhones,
                    ShortName = subscriber.ShortName,
                    SubscriberRepresentative = subscriber.SubscriberRepresentative
                });
            }
            return subscriberModel;
        }

        /// <summary>
        /// Получение всех записей абонентов для пользователя
        /// </summary>
        [HttpGet]
        [Route("getsubscribers")]
        [Authorize(Roles = "Manager, Operator, User")]
        public IActionResult GetSubscribers()
        {
            try
            {
                //Получаем сех абонентов из БД
                var subscribers = _context.Subscribers.ToList();
                List<SubscriberModel> subscribersModel = NewSubscriberModel(subscribers);
                _log.LogInformation($"Получение абонентов из базы данных");
                return Ok(subscribersModel);
            }
            catch (Exception ex)
            {
                _log.LogInformation($"Ошибка при получении абонентов из базы данных: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Обновление данных абонента
        /// </summary>
        /// <param name="id">Идентификатор записи, которую нужно изменить</param>
        /// <param name="subscriberModel">Новые данные абонента</param>
        [HttpPut]
        [Route("updatesubscriber")]
        [Authorize(Roles = "Manager, Operator")]
        public async Task<IActionResult> UpdateSubscriber([FromBody]SubscriberModel subscriberModel)
        {
            try
            {
                //Получаем пользователя
                var user = GetUser();
                var subscriber = _context.Subscribers.FirstOrDefault(s => s.Id == subscriberModel.Id);
                if (subscriber != null)
                {
                    subscriber.Address = subscriberModel.Address;
                    subscriber.FIOHead = subscriberModel.FIOHead;
                    subscriber.FullName = subscriberModel.FullName;
                    subscriber.INN = subscriberModel.INN;
                    subscriber.Phones = subscriberModel.Phones;
                    subscriber.RepresentativePhones = subscriberModel.RepresentativePhones;
                    subscriber.ShortName = subscriberModel.ShortName;
                    subscriber.SubscriberRepresentative = subscriberModel.SubscriberRepresentative;

                    _context.Entry(subscriber).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await _context.SaveChangesAsync();

                    _log.LogInformation($"Обновление абонента с идентификатором {subscriberModel.Id}");
                    return Ok();
                }
                return BadRequest(new JsonResult("Запись с абонентом не найдена"));
            }
            catch (Exception ex)
            {
                _log.LogInformation($"Ошибка при обновлении абонента с идентификатором {subscriberModel.Id}: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

        }

        /// <summary>
        /// Печать списка абонентов
        /// </summary>
        [HttpGet]
        [Route("printsubscriber")]
        [Authorize(Roles = "User")]
        //public FileResult PrintSubscriber()
        public IActionResult PrintSubscriber()
        {
            //Получаем пользователя
            var user = GetUser();
            //Получаем все записи абонентов для пользователя
            var subscribers = _context.Subscribers.ToList();
            //var subscribers = _context.Subscribers.Where(s => s.User.Id == user.Id).ToList();

            //Получение печатной формы
            var streamTemplate = GetResourceStream("PrintTemplateForSubscribers.docx");
            Aspose.Words.License license = new License();
            Stream licFile = GetResourceStream("Aspose.Total.lic");
            license.SetLicense(licFile);
            var document = new Aspose.Words.Document(streamTemplate);

            var fildNameUser = document.Range.Fields.FirstOrDefault(field => field.Result == "«NameUser»");
            if (fildNameUser != null)
            {
                fildNameUser.Result = user.UserName;
            }

            DocumentBuilder builder = new DocumentBuilder(document);
            //Переходим на закладку TableSubscribers в документе
            builder.MoveToBookmark("TableSubscribers");
            //Начинаем построение таблицы
            builder.StartTable();

            //Добавление названий столбцов
            builder.InsertCell();
            builder.Write("Идентификатор");

            builder.InsertCell();
            builder.Write("ИНН");

            builder.InsertCell();
            builder.Write("Краткое наименование абонента");

            builder.InsertCell();
            builder.Write("Полное наименование абонента");

            builder.InsertCell();
            builder.Write("Адрес абонента");

            builder.InsertCell();
            builder.Write("Телефоны абонента");

            builder.InsertCell();
            builder.Write("ФИО руководителя");

            builder.InsertCell();
            builder.Write("Представитель абонента");

            builder.InsertCell();
            builder.Write("Телефоны представителя");

            builder.EndRow();

            //Заполняем документ данными
            foreach (var subscriber in subscribers)
            {
                //Добавление ячеек с данными
                builder.InsertCell();
                builder.Write(subscriber.Id.ToString());

                builder.InsertCell();
                builder.Write(subscriber.INN);

                builder.InsertCell();
                builder.Write(subscriber.ShortName);

                builder.InsertCell();
                builder.Write(subscriber.FullName);

                builder.InsertCell();
                builder.Write(subscriber.Address);

                builder.InsertCell();
                builder.Write(subscriber.Phones);

                builder.InsertCell();
                builder.Write(subscriber.FIOHead);

                builder.InsertCell();
                builder.Write(subscriber.SubscriberRepresentative);

                builder.InsertCell();
                builder.Write(subscriber.RepresentativePhones);

                builder.EndRow();
            }

            builder.EndTable();

            MemoryStream outStream = new MemoryStream();
            document.Save(outStream, SaveFormat.Pdf);
            return Ok(outStream.ToArray());
        }

        private Stream GetResourceStream(string name)
        {
            Assembly currAssembly = Assembly.GetExecutingAssembly();
            string path = currAssembly.GetManifestResourceNames()
                .FirstOrDefault(f => f.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            return path == null ? null : currAssembly.GetManifestResourceStream(path);
        }
    }
}