using Interview.Model;
using Interview.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IdentityUI.Controllers
{
    //[Authorize(Roles="User")]
        public class ProfileController : Controller
        {
            private readonly IProfileService _profileService;
            ErrorViewModel errorModel = new ErrorViewModel();
            public ProfileController(IProfileService profileService)
            {
                _profileService = profileService;
            }
            // GET: Profile


            [Route("Profile")]

            public ActionResult Index()
            {
                try
                {
                    var ProfileList = _profileService.GetAll();
                    return View(ProfileList);
                }
                catch (Exception ex)
                {
                    // here error message is passed to "Error1.chtml" by calling return function 
                    errorModel.ErrorMessage = "Error occurred while updating: " + ex.Message;
                    return View("Error1", errorModel);
                }

            }

            // GET: Profile/Details/5
            public ActionResult Details(int id)
            {
                return View();
            }

            // GET: Profile/Create
            public ActionResult Create()
            {
                return View();
            }

            // POST: Profile/Create
            [HttpPost]

            public ActionResult Create(ProfileModel model)
            {
                try
                {

                    bool isInserted = false;
                    if (ModelState.IsValid)
                    {
                        isInserted = _profileService.InsertProfile(model);
                        if (isInserted)
                        {
                            @TempData["InfoMessage"] = "Saved successfully";

                            return RedirectToAction("Index");
                        }
                        else
                        {
                            @TempData["InfoMessage"] = "Unsuccessful";
                        }
                    }
                    else
                    {
                        Response.Write("<script>alert('Enter details in all fields');</script>");
                    }


                    return View(model);
                }
                catch (Exception ex)
                {
                    //error is shown in same create view

                    ModelState.AddModelError("", $"An error occurred while creating the profile: {ex.Message}");

                    return View(model);


                }
            }

            // GET: Profile/Edit/5
            public ActionResult Edit(int id)
            {
                try
                {
                    var _profile = _profileService.getProfileByID(id);
                    if (_profile == null)
                    {
                        TempData["InfoMessage"] = "Profile Not Found";
                        return RedirectToAction("Index");
                    }
                    return View(_profile);
                }
                catch (Exception ex)
                {
                    // here error message is passed to "Error1.chtml" by calling return function 
                    errorModel.ErrorMessage = "Error occurred while updating: " + ex.Message;
                    return View("Error1", errorModel);
                }

            }

            // POST: Profile/Edit/5
            [HttpPost]
            public ActionResult Edit(ProfileModel model)
            {

                try
                {
                    bool isUpdated = false;
                    if (ModelState.IsValid)
                    {
                        isUpdated = _profileService.UpdateProfile(model);
                        if (isUpdated)
                        {
                            @TempData["InfoMessage"] = "Saved successfully";
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            @TempData["InfoMessage"] = "Unsuccessful";
                        }
                    }
                    Response.Write("<script>alert('Enter details in all fields');</script>");

                    return View(model);
                }
                catch (Exception ex)
                {
                    // here error message is passed to "Error1.chtml" 
                    errorModel.ErrorMessage = "Error occurred while updating: " + ex.Message;
                    return View("Error1", errorModel);
                }
            }
            // GET: Profile/Delete/5
            public ActionResult Delete(int id)
            {
                try
                {
                    var _profile = _profileService.getProfileByID(id);
                    if (_profile == null)
                    {
                        TempData["InfoMessage"] = "Product Not Available";
                        return RedirectToAction("Index");
                    }
                    return View(_profile);
                }
                catch (Exception ex)
                {
                    // here error message is passed to "Error1.chtml" 
                    errorModel.ErrorMessage = "Error occurred while deleting: " + ex.Message;
                    return View("Error1", errorModel);
                }
            }

            // POST: Profile/Delete/5
            [HttpPost, ActionName("Delete")]
            public ActionResult DeleteConfirmation(int id)
            {
                try
                {
                    _profileService.DeleteProfile(id);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    //passes error message to Error1.chtml automatically
                    errorModel.ErrorMessage = "Error occurred while updating: " + ex.Message;
                    return View("Error1", errorModel);
                }
            }
        }
}
