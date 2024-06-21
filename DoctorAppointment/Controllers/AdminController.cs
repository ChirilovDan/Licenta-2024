using DoctorAppointment.Areas.Identity.Data;
using DoctorAppointment.DTO;
using DoctorAppointment.Models;
using DoctorAppointment.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoctorAppointment.Controllers
{


    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AdminController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            ViewBag.doc = (from user in _context.Users
                       join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                       join roles in _context.Roles on userRoles.RoleId equals roles.Id
                       where roles.Name == "Doctor"
                       select user).Count();

            ViewBag.Pat = (from user in _context.Users
                       join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                       join roles in _context.Roles on userRoles.RoleId equals roles.Id
                       where roles.Name == "Patience"
                       select user).Count();
            ViewBag.op = _context.PatientsOrders.Count();
            var obj = _context.PatientsOrders.Select(x => x.TotalPay).Sum();
            ViewBag.Res = obj.ToString("0");

            return View();
        }
        public IActionResult Specialities()
        {
            var obj = _context.Specialities.ToList();
            return View(obj);
        }
        public IActionResult SpecialitiesAdd()
        {

            return View();
        }
        [HttpPost]
        public IActionResult SpecialitiesAdd(Specialities specialities)
        {
            if(specialities !=null)
            {
                string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string webRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(specialities.ProductFile.FileName);
                string extension = Path.GetExtension(specialities.ProductFile.FileName);
                string uniqueFileName = fileName + "_" + dateTime + extension;

                specialities.Image = uniqueFileName;
                string filePath = Path.Combine(webRootPath, "specialities", uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    specialities.ProductFile.CopyTo(fileStream);
                }
                _context.Specialities.Add(specialities);
                _context.SaveChanges();
                return RedirectToAction("Specialities");
            }
            return View(specialities);
        }
        public IActionResult SpecialitiesEdit(int id)
        {
            var obj = _context.Specialities.Find(id);
            if(obj !=null)
            {
                return View(obj);
            }
            return RedirectToAction("Specialities");
        }
        [HttpPost]
        public IActionResult SpecialitiesEdit(Specialities specialities)
        {
            if (specialities != null)
            {
                if(specialities.ProductFile !=null)
                {
                    string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    string webRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(specialities.ProductFile.FileName);
                    string extension = Path.GetExtension(specialities.ProductFile.FileName);
                    string uniqueFileName = fileName + "_" + dateTime + extension;

                    specialities.Image = uniqueFileName;
                    string filePath = Path.Combine(webRootPath, "specialities", uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        specialities.ProductFile.CopyTo(fileStream);
                    }
                }
              
                _context.Specialities.Update(specialities);
                _context.SaveChanges();
                return RedirectToAction("Specialities");
            }
            return View(specialities);
        }
        public IActionResult DoctorList()
        {
            var obj = (from user in _context.Users
                      join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                      join roles in _context.Roles on userRoles.RoleId equals roles.Id
                      join dt in _context.DoctorContactDetails on user.Id equals dt.UserID
                      join sp in _context.Specialities on user.specialitiesId equals sp.Id
                      where roles.Name == "Doctor"
                      select new DoctorViewModel
                      {
                         DOB = user.DOB,
                         Email = user.Email,
                         Image = user.Image,
                         Name = user.FirstName + " " + user.LastName,
                         SpeicalFiled= sp.Name,

                      }).ToList();  
            return View(obj);
        }
       
        public IActionResult AddDoctor()
        {
            ViewBag.special = new SelectList(_context.Specialities, "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddDoctor(UserDto user)
        {
            var check = _context.Users.Where(x => x.Email == user.Email).FirstOrDefault();
            if (check == null)
            {
                string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string webRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(user.ProductFile.FileName);
                string extension = Path.GetExtension(user.ProductFile.FileName);
                string uniqueFileName = fileName + "_" + dateTime + extension;

                user.Image = uniqueFileName;
                string filePath = Path.Combine(webRootPath, "DoctorImage", uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    user.ProductFile.CopyTo(fileStream);
                }

                var app = new ApplicationUser
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    AboutMe = user.AboutMe,
                    DOB = user.DOB,
                    Gender = user.Gender,
                    PhoneNumber = user.PhoneNumber,
                    specialitiesId = user.specialitiesId,
                    UserName = user.Email,
                    Image = user.Image,
                    Email = user.Email,
                };
                var result = await _userManager.CreateAsync(app, user.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(app, "Doctor");

                    var dt = new DoctorContactDetails
                    {
                        Adress = user.Adress,
                        City = user.City,
                        Country = user.Country,
                        DoctorAvailability = user.DoctorAvailability,
                        DoctorFee = user.DoctorFee,
                        PostalCode = user.PostalCode,
                        State = user.State,
                        UserID = app.Id,
                    };
                    _context.DoctorContactDetails.Add(dt);
                    _context.SaveChanges();
                    return RedirectToAction("DoctorList");
                }
                else
                {

                    ModelState.AddModelError("", "Password must contain at least one special character, one capital letter, and be 6 or more characters long.");
                    return View(user);
                }
            }
            else
            {
                ModelState.AddModelError("", $"The email {user.Email} is already available in the system. Please try again.");
                return View(user);
            }
           
           
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login","Login");
        }
        public IActionResult Appointments()
        {
            var data = (from po in _context.PatientsOrders
                        join user in _context.Users
                        on po.UserID equals user.Id                    
                        select new AppointmentViewModel
                        {
                            Amount = po.TotalPay,
                            Image = user.Image,
                            Status = po.Status,
                            Name = user.FirstName + " " + user.LastName,
                            AppointmentDate = po.OrderDate,
                            BookingDate = po.SelectDay,
                            Time = po.SelectTime,
                            id = po.Id,
                            PatientId = po.UserID,
                        }).ToList();
            return View(data);
        }
        public IActionResult UpdateStatus(int id)
        {
            var obj = _context.PatientsOrders.Find(id);
            if (obj != null)
            {
              
                _context.PatientsOrders.Remove(obj);
                _context.SaveChanges();
                return new JsonResult("Delete successfully");
            }
            return new JsonResult("Delete not successfully");
        }
        public IActionResult Prescriptions(string id)
        {
            var obj = _context.Prescriptions.Where(x => x.PatientId == id).ToList();
            return View(obj);
        }
    }
}
