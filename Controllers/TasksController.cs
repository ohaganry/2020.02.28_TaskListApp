using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Models;

namespace TaskList.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly TaskListDbContext _context;

        public TasksController(TaskListDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            string id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<ToDo> usersTasks = _context.ToDo.Where(x => x.UserId == id).ToList();
            return View(usersTasks);
        }
        [HttpGet]
        public IActionResult AddTask()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddTask(ToDo newTask)
        {
            newTask.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (ModelState.IsValid)
            {
                _context.ToDo.Add(newTask);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
        public IActionResult DeleteTask(int id)
        {
            ToDo foundTask = _context.ToDo.Find(id);
            if(foundTask != null)
            {
                _context.ToDo.Remove(foundTask);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public IActionResult ChangeStatus(int id)
        {
            ToDo foundTask = _context.ToDo.Find(id);
            if(foundTask != null)
            {
                foundTask.Complete = !foundTask.Complete;
                _context.Entry(foundTask).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.Update(foundTask);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}