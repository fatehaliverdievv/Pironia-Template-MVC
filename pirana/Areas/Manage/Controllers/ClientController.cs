using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pirana.DAL;
using pirana.Models;

namespace pirana.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Authorize(Roles = "Admin,Moderator")]
    public class ClientController : Controller
    {
        private readonly AppDbContext _context;

        public ClientController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.Clients.ToList());
        }
        public IActionResult Delete(int id)
        {
            Client client = _context.Clients.Find(id);
            if (client is null) return NotFound();
            _context.Clients.Remove(client);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(string name, string surname, string imgurl,string position,string feedback)
        {
            if (!ModelState.IsValid) return View();
            _context.Clients.Add(new Client { Name = name, Surname = surname, ImgUrl = imgurl, Feedback = feedback, Position = position });
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int? id)
        {
            Client client = _context.Clients.Find(id);
            if (client is null) return NotFound();
            if (id is null || id == 0) BadRequest();
            return View(client);
        }
        [HttpPost]
        public IActionResult Update(int? id, Client client)
        {
            if (!ModelState.IsValid) return View();
            if (id is null || id != client.Id) BadRequest();
            Client existedclient = _context.Clients.Find(id);
            if (existedclient is null) return NotFound();
            existedclient.Name = client.Name;
            existedclient.Surname = client.Surname;
            existedclient.ImgUrl = client.ImgUrl;
            existedclient.Position = client.Position;
            existedclient.Feedback = client.Feedback;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
