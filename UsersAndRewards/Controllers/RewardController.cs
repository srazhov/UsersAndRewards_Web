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

        [Route("awards")]
        public async Task<IActionResult> Index()
        {
            var items = _mapper.Map<List<RewardViewModel>>(await _db.Rewards.ToListAsync());

            return View(items);
        }

        [Route("awards/{letter:maxlength(1)}")]
        public IActionResult GetRewardsByLetter(string letter)
        {
            var items = from u in _db.Rewards
                        where u.Title.StartsWith(letter)
                        select u;
            var mappeds = _mapper.Map<List<RewardViewModel>>(items);

            return View("Index", mappeds);
        }

        [Route("awards/{name:minlength(2)}")]
        public IActionResult GetRewardsByWord(string name)
        {
            var items = from u in _db.Rewards
                        where u.Title.Contains(name)
                        select u;
            var mappeds = _mapper.Map<List<RewardViewModel>>(items);

            return View("Index", mappeds);
        }

        [Route("award/{name:minlength(2)}")]
        public IActionResult GetRewardsByName(string name)
        {
            name = name.Replace("_", " ");
            var items = (from u in _db.Rewards
                         where u.Title == name
                         select u).Take(1);
            var mappeds = _mapper.Map<List<RewardViewModel>>(items);

            return View("Index", mappeds);
        }

        [Route("award/{id=0}")]
        public async Task<IActionResult> GetItem(int id)
        {
            var reward = await _db.Rewards.FindAsync(id);
            if (reward == null)
            {
                return NotFound();
            }

            var items = new List<RewardViewModel>() { _mapper.Map<RewardViewModel>(reward) };
            return View("Index", items);
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

        [Route("create-award")]
        [HttpGet]
        public IActionResult Create()
        {
            var NewReward = new RewardViewModel()
            {
                Id = 0,
                Title = string.Empty,
                Description = string.Empty,
                ImageUrl = null
            };

            return View("Edit", NewReward);
        }

        [Route("award/{index=0}/edit")]
        [HttpGet]
        public async Task<IActionResult> Edit(int index)
        {
            var item = await _db.Rewards.FindAsync(index);
            if (item == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<RewardViewModel>(item);

            return View(result);
        }

        [Route("award/{index=0}/delete")]
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

        [Route("award-user/{ids}")]
        public IActionResult RewardUserByIds(string ids)
        {
            var splitValues = ids.Split("_");
            if(splitValues.Length != 2)
            {
                return NotFound();
            }

            if (!int.TryParse(splitValues[0], out int user))
            {
                user = 0;
            }

            if(!int.TryParse(splitValues[1], out int reward))
            {
                reward = 0;
            }

            return RedirectToAction("RewardUser", new { userIndex = user, rewardIndex = reward });
        }

        [HttpGet]
        public async Task<IActionResult> RewardUser(int userIndex, int rewardIndex)
        {
            if (rewardIndex == 0)
            {
                ViewBag.RewardSelector = userIndex;
                var items = _mapper.Map<List<RewardViewModel>>(await _db.Rewards.ToListAsync());

                return View("Index", items);
            }

            var user = await _db.Users.FindAsync(userIndex);
            var reward = await _db.Rewards.FindAsync(rewardIndex);
            if (user == null || reward == null)
            {
                return NotFound();
            }

            var result = from u in _db.Users
                         where u.Id == userIndex
                         where u.Rewards.Any(r => r.Id == rewardIndex)
                         select u;
            if (result.Any())
            {
                return Content("Нельзя награждать одной и той же наградой дважды");
            }

            user.Rewards.Add(reward);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "User");
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
