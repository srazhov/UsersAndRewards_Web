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

        [Route("users")]
        public async Task<IActionResult> Index()
        {
            var items = _mapper.Map<List<UserViewModel>>(await _db.Users.Include(u => u.Rewards.Take(4)).ToListAsync());

            return View(items);
        }

        [Route("users/{letter:maxlength(1)}")]
        public IActionResult GetUsersByLetter(string letter)
        {
            var items = from u in _db.Users
                        where u.Name.StartsWith(letter)
                        select u;
            var mappeds = _mapper.Map<List<UserViewModel>>(items);

            return View("Index", mappeds);
        }

        [Route("users/{name:minlength(2)}")]
        public IActionResult GetUsersByName(string name)
        {
            var items = from u in _db.Users
                        where u.Name.StartsWith(name) || u.Name.EndsWith(name)
                        select u;
            var mappeds = _mapper.Map<List<UserViewModel>>(items);

            return View("Index", mappeds);
        }

        [Route("user/{index=0}")]
        public IActionResult GetUser(int index)
        {
            var item = _mapper.Map<List<UserViewModel>>(_db.Users.Where(u => u.Id == index).Include(u => u.Rewards));

            return View("Index", item);
        }

        [Route("user/{name:minlength(2)}")]
        public async Task<IActionResult> GetUserByName(string name)
        {
            name = name.Replace('_', ' ');
            var item = await (from u in _db.Users
                              where u.Name == name
                              orderby u.Birthdate
                              select u).FirstOrDefaultAsync();
            if (item == null)
            {
                return NotFound();
            }

            var mappeds = _mapper.Map<UserViewModel>(item);

            return View("Index", new List<UserViewModel>() { mappeds });
        }

        [Route("create-user")]
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel userVM, IFormFile uploadFile)
        {
            if (userVM.Birthdate.Year < DateTime.Today.Year - 150 || DateTime.Today < userVM.Birthdate)
            {
                ModelState.AddModelError("", "Возраст пользователя не может быть больше 150 или быть отрицательным");
            }

            if (!ModelState.IsValid)
            {
                return View("Edit", userVM);
            }

            var item = _mapper.Map<User>(userVM);
            if (item.Id == 0)
            {
                _db.Add(item);
            }
            else
            {
                item.Photo = await (from u in _db.Users.AsNoTracking()
                                    where u.Id == item.Id
                                    select u.Photo).SingleAsync();
                _db.Entry(item).State = EntityState.Modified;
            }

            await _db.SaveChangesAsync();

            if (uploadFile != null)
            {
                item.Photo = await CommonActions.CreateImage(_environment, uploadFile, "/imgs/user/");

                _db.Entry(item).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [Route("create-user")]
        [HttpGet]
        public IActionResult Create()
        {
            var NewUser = new UserViewModel()
            {
                Id = 0,
                Name = string.Empty,
                Birthdate = DateTime.Now,
                PhotoUrl = null
            };

            return View("Edit", NewUser);
        }

        [Route("user/{index=0}/edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int index)
        {
            var item = await _db.Users.FindAsync(index);
            if(item == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<UserViewModel>(item);

            return View(result);
        }

        [Route("user/{index=0}/delete")]
        [HttpGet]
        public async Task<IActionResult> Delete(int index)
        {
            var item = await _db.Users.FindAsync(index);
            if (item != null)
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
            if (item != null)
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
            var rawUsers = await _db.Users.Include(u => u.Rewards).ToListAsync();
            var users = _mapper.Map<List<UserViewModel>>(rawUsers);
            var result = new StringBuilder(users.Count * 100);

            foreach (var user in users)
            {
                string resString = $"{user.Name} -- {user.Birthdate.ToShortDateString()} -- {user.Age} y.o ";
                if (!user.RewardsVM.Any())
                {
                    resString += "/// Пользователь не имеет наград /// ";
                }
                else
                {
                    StringBuilder rewResult = new StringBuilder(user.RewardsVM.Count * 50);
                    foreach (var rew in user.RewardsVM)
                    {
                        rewResult.Append($"{rew.Title}, ");
                    }

                    resString += $"/// {rewResult} /// ";
                }

                result.Append(resString + "\n");
            }

            FileInfo fileInfo = new FileInfo("Users data");
            using (var sw = fileInfo.CreateText())
            {
                await sw.WriteAsync(result);
            }

            return File(fileInfo.OpenRead(), System.Net.Mime.MediaTypeNames.Application.Octet, "Users Data.txt");
        }
    }
}
