using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using UsersAndRewards.Csharp;
using UsersAndRewards.Database;
using UsersAndRewards.Database.Tables;
using UsersAndRewards.Models;

namespace UsersAndRewards.Controllers
{
    public class RewardController : Controller
    {
        private readonly URContext _db;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public RewardController(URContext db, IMapper mapper, IWebHostEnvironment webHost)
        {
            _db = db;
            _mapper = mapper;
            _environment = webHost;
        }

        public async Task<IActionResult> Index()
        {
            var items = _mapper.Map<List<RewardViewModel>>(await _db.Rewards.ToListAsync());
            
            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create(RewardViewModel rewardVm, IFormFile uploadFile)
        {
            var item = _mapper.Map<Reward>(rewardVm);
            if (item.Id == 0)
            {
                _db.Add(item);
            }
            else
            {
                item.Image = await (from r in _db.Rewards.AsNoTracking()
                                    where r.Id == item.Id
                                    select r.Image).SingleAsync();
                _db.Entry(item).State = EntityState.Modified;
            }

            if (string.IsNullOrEmpty(item.Image) && uploadFile == null || !ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Добавьте изображение");
                }

                return View("Edit", rewardVm);
            }

            await _db.SaveChangesAsync();

            if (uploadFile != null)
            {
                var photoUrl = await CommonActions.CreateImage(_environment, uploadFile, "/imgs/reward/");
                item.Image = photoUrl ?? item.Image;

                _db.Entry(item).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int index)
        {
            var item = await _db.Rewards.FindAsync(index);
            if (index == 0 || item == null)
            {
                //Создание новой награды, если Id == 0
                var NewReward = new RewardViewModel()
                {
                    Id = 0,
                    Title = string.Empty,
                    Description = string.Empty,
                    ImageUrl = null
                };

                return View(NewReward);
            }

            var result = _mapper.Map<RewardViewModel>(item);

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int index)
        {
            var item = await _db.Rewards.FindAsync(index);
            if (item != null)
            {
                _db.Rewards.Remove(item);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> RemoveImage(int index)
        {
            var item = await _db.Rewards.FindAsync(index);
            if (item != null)
            {
                item.Image = null;
                await _db.SaveChangesAsync();

                var reward = _mapper.Map<RewardViewModel>(item);
                return View("Edit", reward);
            }

            return NotFound();
        }
    }
}
