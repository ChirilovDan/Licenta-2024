using DoctorAppointment.Areas.Identity.Data;
using DoctorAppointment.DTO;
using DoctorAppointment.EmailSender;
using DoctorAppointment.Models;
using DoctorAppointment.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace DoctorAppointment.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMailSender _sender;
        public DoctorController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMailSender sender)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
            _sender = sender;   
        }
        public IActionResult Index()
        {
            string userId = _userManager.GetUserId(User);
            var obj = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if(obj !=null)
            {
                ViewBag.Name = obj.FirstName + " " + obj.LastName;
                ViewBag.Image = obj.Image;
                ViewBag.About = obj.AboutMe;

            }

            var data = (from po in _context.PatientsOrders
                       join user in _context.Users
                       on po.UserID equals user.Id                     
                       where po.DocID == userId
                       select new AppointmentViewModel
                       {
                           Amount = po.TotalPay,
                           Image = user.Image,
                           Status = po.Status,
                           Name = user.FirstName + " " + user.LastName,
                           AppointmentDate = po.OrderDate,
                           BookingDate = po.SelectDay,
                         
                           Time = po.SelectTime,
                           PatientId = po.UserID,
                       }).ToList();
            return View(data);
        }
        public IActionResult ScheduleTimings()
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
            var obj = _context.ScheduleTimings.ToList();
            return View(obj);
        }
        public IActionResult AddSchedule()
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
            var obj = _context.ScheduleTimings.Where(x=>x.userId == userId).Select(x => x.Day).ToList();
            ViewBag.DisableDay = obj;
            return View();
        }
        [HttpPost]
        public  IActionResult AddSchedule(ScheduleDTO schedule)
        {

            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
            if (schedule.StartTime.Length != schedule.EndTime.Length)
            {
                // Handle the error case if the lengths do not match
                ModelState.AddModelError("", "StartTime and EndTime arrays must have the same length.");
                return View(schedule);
            }

            var combinedTimesList = new List<string>();
            for (int i = 0; i < schedule.StartTime.Length; i++)
            {
                combinedTimesList.Add($"{schedule.StartTime[i]:hh:mm tt} - {schedule.EndTime[i]:hh:mm tt}");
            }

            schedule.CombinedTimes = string.Join(", ", combinedTimesList);

            var st = new ScheduleTiming
            {
                CurrentYear = DateTime.Now,
                Day = schedule.SelectDay,
                Slot = schedule.CombinedTimes,
                userId =userId,
            };
            _context.ScheduleTimings.Add(st);
            _context.SaveChanges();
            return RedirectToAction("ScheduleTimings");
        }

        public IActionResult DeleteSchedule(string day)
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
           

            var getData = _context.ScheduleTimings.Where(t => t.Day == day && t.userId == userId).FirstOrDefault();
            if(getData != null)
            {
                _context.ScheduleTimings.Remove(getData);
                _context.SaveChanges();
                return RedirectToAction("ScheduleTimings");
            }
            return RedirectToAction("ScheduleTimings");
        }

        public IActionResult DoctorProfile()
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }

            var getData =( from user in _context.Users join
                          ud in _context.DoctorContactDetails on
                          user.Id equals ud.UserID
                          where user.Id == userId
                          select new UserDto
                          {
                              Email = user.Email,
                              FirstName = user.FirstName,   
                              LastName = user.LastName,
                              AboutMe = user.AboutMe,
                              Gender = user.Gender,
                              DOB =user.DOB,
                              PhoneNumber =user.PhoneNumber,
                              DoctorAvailability = ud.DoctorAvailability,
                              Image = user.Image,
                              Adress = ud.Adress,
                              City= ud.City,
                              Country = ud.Country,
                              DoctorFee = ud.DoctorFee,
                              PostalCode = ud.PostalCode,
                              State =ud.State,
                             specialitiesId = user.specialitiesId,
                             UserId = user.Id,
                             dtId = ud.id
                          }).FirstOrDefault();
            return View(getData);
        }
        [HttpPost]
        public async Task< IActionResult> DoctorProfile(UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.UserId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if(userDto.ProductFile !=null)
            {
                string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string webRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(userDto.ProductFile.FileName);
                string extension = Path.GetExtension(userDto.ProductFile.FileName);
                string uniqueFileName = fileName + "_" + dateTime + extension;

                 userDto.Image =  user.Image = uniqueFileName;
                string filePath = Path.Combine(webRootPath, "DoctorImage", uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    userDto.ProductFile.CopyTo(fileStream);
                }
            }

            user.PhoneNumber = userDto.PhoneNumber;
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.AboutMe = userDto.AboutMe;
            user.DOB = userDto.DOB;
            user.Gender = userDto.Gender;
            user.Image = userDto.Image;
            user.specialitiesId = userDto.specialitiesId;
           

            var result = await _userManager.UpdateAsync(user);
            if(result.Succeeded)
            {

                var dt = new DoctorContactDetails
                {
                    Adress = userDto.Adress,
                    City = userDto.City,
                    Country = userDto.Country,
                    DoctorAvailability = userDto.DoctorAvailability,
                    DoctorFee = userDto.DoctorFee,
                    id = userDto.dtId,
                    PostalCode = userDto.PostalCode,
                    State = userDto.State,
                    UserID= userDto.UserId,
                
                };
                _context.DoctorContactDetails.Update(dt);
                _context.SaveChanges();
                return RedirectToAction("Index");

            }
            else
            {
                ModelState.AddModelError(string.Empty, "not Update the user information");
                return View(userDto);
            }
           
        }
        public IActionResult UpdatePassword()
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
            return View();
        }
        [HttpPost]
        public async Task< IActionResult> UpdatePassword(UpdatePasswordDTO updatePassword)
        {
            string userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (updatePassword.Password != null)
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, updatePassword.OldPassword, updatePassword.Password);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(updatePassword);
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            await _signInManager.RefreshSignInAsync(user);


            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Login");
        }

        public IActionResult Prescriptions(string id)
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
            var obj = _context.Prescriptions.Where(x=>x.PatientId == id).ToList();
            ViewBag.userId = id;
            return View(obj);
        }

        public IActionResult AddPrescriptions(string id)
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
            ViewBag.patientId = id;
            return View();
        }
        [HttpPost]
        public IActionResult AddPrescriptions(Prescription prescription)
        {
           

            prescription.PrescriptionDate = System.DateTime.Now;
            _context.Prescriptions.Add(prescription);
            _context.SaveChanges();
            var obj = _context.Users.Where(x => x.Id == prescription.PatientId).FirstOrDefault();

            var message = new Message(obj.Email, "Medicine Prescription",
                        $"Hello Dear {obj.FirstName} {obj.LastName}!" +
                        "<br /><br />" +
                        "We are thrilled to inform you that the doctor has written a prescription for you. Please check it on the dashboard!" +
                        "<br /><br />Doctor Appointment",
                        ""
                    );
            _sender.MessageSend(message);


            return RedirectToAction("Prescriptions", new { id = prescription.PatientId });
        }
        public IActionResult EditPrescriptions(int id)
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
            var obj = _context.Prescriptions.Find(id);
            if(obj!=null)
            {
                
                return View(obj);
            }
            return View();
        }
        [HttpPost]
        public IActionResult EditPrescriptions(Prescription prescription)
        {
            _context.Prescriptions.Update(prescription);
            _context.SaveChanges();
            return RedirectToAction("Prescriptions", new { id = prescription.PatientId });
        }
        public IActionResult DeletePrescriptions(int id)
        {
            var obj = _context.Prescriptions.Find(id);
            if(obj !=null)
            {
                _context.Prescriptions.Remove(obj);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult DoctorServices()
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
            var obj = _context.OfferServices.ToList();
            return View(obj);
        }
        public IActionResult AddServices()
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
            return View();
        }

        [HttpPost]
        public IActionResult AddServices(OfferServices offer)
        {
            
            string userID = _userManager.GetUserId(User);
            offer.DoctorId = userID;
            _context.OfferServices.Add(offer);
            _context.SaveChanges();

            return RedirectToAction("DoctorServices");
        }
        public IActionResult EditServices(int id)
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
            var obj = _context.OfferServices.Find(id);
            if(obj !=null)
            {
                return View(obj);
            }
            return View();  
        }
        [HttpPost]
        public IActionResult EditServices(OfferServices offerServices)
        {
            _context.OfferServices.Update(offerServices);
            _context.SaveChanges();
            return RedirectToAction("DoctorServices");
        }
        public IActionResult RemoveServices(int id)
        {
            var obj = _context.OfferServices.Find(id);
            if (obj != null)
            {
                _context.OfferServices.Remove(obj);
                _context.SaveChanges();
                return RedirectToAction("DoctorServices");
            }
            return View();
        }
    }

}
