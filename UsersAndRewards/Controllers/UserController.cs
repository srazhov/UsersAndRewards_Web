using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UsersAndRewards.Database;
using UsersAndRewards.Database.Tables;
using UsersAndRewards.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using UsersAndRewards.Csharp;
using System.Text;

namespace UsersAndRewards.Controllers
{
    public class UserController : Controller
    {
        private readonly URContext _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public UserController(URContext context, IMapper mapper, IWebHostEnvironment webHost)
        {
            _db = context;
            _mapper = mapper;
            _environment = webHost;
        }

        public async Task<IActionResult> Index()
        {
            var items = _mapper.Map<List<UserViewModel>>(await _db.Users.ToListAsync());

            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel userVM, IFormFile uploadFile)
        {
            var item = _mapper.Map<User>(userVM);

            item.Photo = await CommonActions.CreateImage(_environment, uploadFile, "/imgs/user/");
            if (item.Id == 0)
            {
                _db.Add(item);
            }
            else
            {
                _db.Entry(item).State = EntityState.Modified;
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int index)
        {
            var item = await _db.Users.FindAsync(index);
            if(index == 0 || item == null)
            {
                //Создание нового пользователя, если Id == 0
                var NewUser = new UserViewModel()
                {
                    Id = 0,
                    Name = string.Empty,
                    Birthdate = DateTime.Now,
                    PhotoUrl = null
                };

                return View(NewUser);
            }

            var result = _mapper.Map<UserViewModel>(item);

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int index)
        {
            var item = await _db.Users.FindAsync(index);
            if(item != null)
            {
                _db.Users.Remove(item);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> RemoveImage(int index)
        {
            var item = await _db.Users.FindAsync(index);
            if(item != null)
            {
                item.Photo = null;
                await _db.SaveChangesAsync();

                var user = _mapper.Map<UserViewModel>(item);
                return View("Edit", user);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<FileResult> GetUsersFile()
        {
            var rawUsers = await _db.Users.ToListAsync();
            var users = _mapper.Map<List<UserViewModel>>(rawUsers);
            var result = new StringBuilder(users.Count * 100);

            foreach (var user in users)
            {
                result.Append($"{user.Name} -- {user.Birthdate.ToShortDateString()} -- {user.Age} y.o \n");
            }

            FileInfo fileInfo = new FileInfo("Users data");
            if (!fileInfo.Exists)
            {
                using var sw = fileInfo.CreateText();
                sw.Write(result);
            }

            return File(fileInfo.OpenRead(), System.Net.Mime.MediaTypeNames.Application.Octet, "Users Data.txt");
        }
    }
}
