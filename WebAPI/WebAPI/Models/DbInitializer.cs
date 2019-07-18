using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Entities;

namespace WebAPI.Models
{
    /// <summary>
    /// Загружаем тестовые данные в базу данных
    /// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Инициализация базы
        /// </summary>
        public static void Initialze(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            context.Database.EnsureCreated();
            SetRoles(roleManager);
            SetUsers(userManager);
            SetSubscribers(context);
        }

        /// <summary>
        /// Установить роли в базу
        /// </summary>
        /// <param name="roleManager"></param>
        private static void SetRoles(RoleManager<Role> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                Role role = new Role()
                {
                    DisplayName = "Администратор системы",
                    Name = "Admin"
                };
                roleManager.CreateAsync(role).Wait();
            }

            //Role1 - Manager: Добавление записи, Редактирование записи, просмотр списка
            if (!roleManager.RoleExistsAsync("Manager").Result)
            {
                Role role = CreateRole("Менеджер", "Manager", canCreate: true, canEdit: true, canShow: true, canPrint: false);
                roleManager.CreateAsync(role).Wait();
            }

            //Role2 - Operator: Редактирование записи, Просмотр списка
            if (!roleManager.RoleExistsAsync("Operator").Result)
            {
                Role role = CreateRole("Оператор", "Operator", canCreate: false, canEdit: true, canShow: true, canPrint: false);
                roleManager.CreateAsync(role).Wait();
            }

            //Role3 - User: Просмотр списка, Печать списка
            if (!roleManager.RoleExistsAsync("User").Result)
            {
                Role role = CreateRole("Пользователь", "User", canCreate: false, canEdit: false, canShow: true, canPrint: true);
                roleManager.CreateAsync(role).Wait();
            }
        }

        /// <summary>
        /// Создание роли
        /// </summary>
        /// <param name="displayName">Отображаемое название роли на русском языке</param>
        /// <param name="name">Название роли</param>
        /// <param name="canCreate">Разрешено добавлять</param>
        /// <param name="canEdit">Разрешено изменять</param>
        /// <param name="canShow">Разрешено просматривать</param>
        /// <param name="canPrint">Разрешено печатать</param>
        /// <returns></returns>
        private static Role CreateRole(string displayName, string name, bool canCreate, bool canEdit, bool canShow, bool canPrint)
        {
            return new Role() { DisplayName = displayName, Name = name, CanCreate = canCreate, CanEdit = canEdit, CanShow = canShow, CanPrint = canPrint };
        }

        /// <summary>
        /// Установить пользователей в базу
        /// </summary>
        /// <param name="userManager"></param>
        private static void SetUsers(UserManager<User> userManager)
        {
            if (userManager.FindByNameAsync("Administrator").Result == null)
            {
                var userAdmin = new User()
                {
                    UserName = "Administrator",
                    Email = "admin@mail.ru",
                    FirstName = "Администратор"
                };
                IdentityResult result = userManager.CreateAsync(userAdmin, "Adm1n_").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(userAdmin, "Admin").Wait();
                }
            }

            if (userManager.FindByNameAsync("Ivan").Result == null)
            {
                var user = new User()
                {
                    UserName = "Ivan",
                    Email = "ivan@mail.ru",
                    FirstName = "Иван"
                };
                IdentityResult result = userManager.CreateAsync(user, "IvanP@ss1").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Operator").Wait();
                }
            }
            if (userManager.FindByNameAsync("Stas").Result == null)
            {
                var user = new User()
                {
                    UserName = "Stas",
                    Email = "stas@mail.ru",
                    FirstName = "Стас"
                };
                IdentityResult result = userManager.CreateAsync(user, "StasP@ss1").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRolesAsync(user, new List<string>() { "Manager", "User" }).Wait();
                }
            }
        }

        /// <summary>
        /// Установить абонентов в базу
        /// </summary>
        /// <param name="context"></param>
        public static void SetSubscribers(AppDbContext context)
        {
            var result = context.Subscribers.Count();
            if (result == 0)
            {
                context.Subscribers.Add(
                    new Subscriber()
                    {
                        INN = "123456789",
                        ShortName = "Стас",
                        FullName = "Станислав Викторович",
                        Address = "Москва, улица Боровицкая",
                        Phones = "89406541593",
                        FIOHead = "Качановский Юрий",
                        SubscriberRepresentative = "ООО ГазПром",
                        RepresentativePhones = "245632"
                    });

                context.Subscribers.Add(
                    new Subscriber()
                    {
                        INN = "4357837483",
                        ShortName = "Олег",
                        FullName = "Олег Николаевич",
                        Address = "Москва, улица Мичурина",
                        Phones = "89403548754",
                        FIOHead = "Сарае Павел",
                        SubscriberRepresentative = "ООО ТатНефть",
                        RepresentativePhones = "436427"
                    });

                context.Subscribers.Add(
                    new Subscriber()
                    {
                        INN = "89848343542",
                        ShortName = "Екатерина",
                        FullName = "Екатерина Михайловна",
                        Address = "Брянск, улица Ленина",
                        Phones = "89007432345",
                        FIOHead = "Потапов Александр",
                        SubscriberRepresentative = "ООО Сбербанк",
                        RepresentativePhones = "456765"
                    });
                context.SaveChanges();
            }
        }
    }
}
