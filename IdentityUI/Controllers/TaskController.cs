using Interview.Model;
using Interview.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdentityUI.Controllers
{
   
        public class TaskController : Controller
        {
           // private static readonly Logger logger = LogManager.GetCurrentClassLogger();
            private readonly ITaskService _taskService;
            public TaskController(ITaskService taskService)
            {
                _taskService = taskService;
            }
            // GET: Task
            public ActionResult Index(int profileId)
            {
                try
                {
                    ViewBag.ProfileId = profileId;
                    var tasks = _taskService.GetTasks(profileId);
                    return View(tasks);
                }

                catch (Exception ex)
                {
                    // here error message is passed to "Error.chtml" by calling return function 
                    ViewBag.ErrorMessage = ex.Message;
                    return View("Error");
                }
            }

            // GET: Task/Details/5
            public ActionResult Details(int id)
            {
                return View();
            }

            // GET: Task/Create
            public ActionResult Create(int profileId)
            {
                try
                {
                    ViewBag.ProfileId = profileId;
                    return View();
                }
                catch (Exception ex)
                {
                    // here error message is passed to "Error.chtml" by calling return function 
                    ViewBag.ErrorMessage = ex.Message;
                    return View("Error");
                }

            }

            // POST: Task/Create
            [HttpPost]
            public ActionResult Create(TaskModel model)
            {
                try
                {
                    bool isInserted = false;
                    if (ModelState.IsValid)
                    {
                        isInserted = _taskService.TaskAdding(model);
                        if (isInserted)
                        {
                            @TempData["InfoMessage"] = "Saved successfully";
                            return RedirectToAction("Index", new { profileId = model.ProfileId });
                        }
                        else
                        {
                            @TempData["InfoMessage"] = "Unsuccessful";
                            ViewBag.ProfileId = model.ProfileId;
                        }
                    }

                    Response.Write("<script>alert('Enter details in all fields');</script>");
                    return View(model);
                }
                catch (Exception ex)
                {
                    // here error message is passed to "Error.chtml" by calling return function 
                    ViewBag.ErrorMessage = ex.Message;
                    return View("Error");
                }
            }


            // GET: Task/Edit/5
            public ActionResult Edit(int id)
            {
                try
                {
                    var task = _taskService.getTaskByID(id);
                    if (task == null)
                    {
                        TempData["InfoMessage"] = "Task Not Found";
                        return RedirectToAction("Index");
                    }
                    return View(task);
                }
                catch (Exception ex)
                {
                    // here error message is passed to "Error.chtml" by calling return function 
                    ViewBag.ErrorMessage = ex.Message;
                    return View("Error");
                }

            }

            // POST: Task/Edit/5
            [HttpPost]
            public ActionResult Edit(TaskModel model)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        _taskService.UpdatingTask(model);
                        return RedirectToAction("Index", new { profileId = model.ProfileId });
                    }
                    ViewBag.ProfileId = model.ProfileId;
                    Response.Write("<script>alert('Enter details in all fields');</script>");
                    return View(model);
                }
                catch (Exception ex)
                {
                    // here error message is passed to "Error.chtml" by calling return function 
                    ViewBag.ErrorMessage = ex.Message;
                    return View("Error");
                }

            }

            // GET: Task/Delete/5
            public ActionResult Delete(int id)
            {
                try
                {
                    var taskmodel = _taskService.getTaskByID(id);
                    if (taskmodel == null)
                    {
                        TempData["InfoMessage"] = "Task Not Found";
                        return RedirectToAction("Index", new { profileId = taskmodel.ProfileId });
                    }
                    return View(taskmodel);
                }
                catch (Exception ex)
                {
                    // here error message is passed to "Error.chtml" by calling return function 
                    ViewBag.ErrorMessage = ex.Message;
                    return View("Error");
                }
            }

            // POST: Task/Delete/5
            [HttpPost, ActionName("Delete")]
            public ActionResult DeleteConfirm(int id)
            {
                try
                {
                    var taskmodel = _taskService.getTaskByID(id);
                    _taskService.DeleteTask(id);
                    return RedirectToAction("Index", new { profileId = taskmodel.ProfileId });
                }
                catch (Exception ex)
                {

                    //passes error message to Error.chtml automatically
                    throw new ApplicationException($"An error occurred : {ex.Message}", ex);
                }
            }

        }
    }
