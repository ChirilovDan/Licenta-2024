using DoctorAppointment.Areas.Identity.Data;
using DoctorAppointment.DTO;
using DoctorAppointment.Models;
using DoctorAppointment.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace DoctorAppointment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _context = context;
            _userManager = manager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment;
        }

        public async Task< IActionResult> Index()
        {
            var LoginUser = await _userManager.GetUserAsync(User);
            if (LoginUser != null)
            {
                var roles = await _userManager.GetRolesAsync(LoginUser);
                if (roles.Contains("SuperAdmin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (roles.Contains("Doctor"))
                {
                    return RedirectToAction("Index", "Doctor");
                }
              
            }

            var obj = (from user in _context.Users
                       join userRoles in _context.UserRoles on user.Id equals userRoles.UserId
                       join roles in _context.Roles on userRoles.RoleId equals roles.Id
                       join sp in _context.Specialities on user.specialitiesId equals sp.Id
                       join dt in _context.DoctorContactDetails on user.Id equals dt.UserID
                       where roles.Name == "Doctor"
                       select new UserDto
                       {
                           State = dt.State,
                           Country = dt.Country,
                           Image = user.Image,
                           FirstName = user.FirstName + " " + user.LastName,
                           AboutMe = user.AboutMe,
                           DoctorFee = dt.DoctorFee,
                           DoctorAvailability = dt.DoctorAvailability,
                           UserId = user.Id,    
                       }).ToList();
            var special = _context.Specialities.ToList();
            DoctorSpecialViewModel doctorSpecialView = new DoctorSpecialViewModel
            {
                Users = obj,
                Specialities = special

            };
            return View(doctorSpecialView);  
        }

        public IActionResult Booking(string userID)
        {
            var userData = _context.Users.Where(x => x.Id == userID).FirstOrDefault();
            var userContactDetails = _context.DoctorContactDetails.Where(x => x.UserID == userID).FirstOrDefault();
            var sch = _context.ScheduleTimings.Where(x => x.userId == userID).ToList();
            var sr = _context.OfferServices.Where(x => x.DoctorId == userID).ToList();
            BookingViewModel bookingView = new BookingViewModel
            {
                Contry = userContactDetails.Country,
                State = userContactDetails.State,
                DoctorName = userData.FirstName + " " + userData.LastName,
                Image = userData.Image,
                scheduleTimings = sch,
                userid = userID,
                OfferServices =sr,
            };
            
            return View(bookingView);
        }

        public IActionResult checkout(CheckoutViewModel checkoutView)
        {
            decimal sum = 0;
            if (checkoutView.SelectedCheckboxList !=null)
            {
                var selectedServices = checkoutView.SelectedCheckboxList?.Split(',')
                                                        .Select(int.Parse)
                                                        .ToList() ?? new List<int>();
                var getData = _context.OfferServices.ToList();
                
                for (int i = 0; i < selectedServices.Count; i++)
                {
                    var service = getData.FirstOrDefault(s => s.ID == selectedServices[i]);
                    if (service != null)
                    {
                        sum += service.Price;
                    }

                }
                ViewBag.TotalPrice = sum;
            }
           

            var obj = (from user in _context.Users
                       join
                       dt in _context.DoctorContactDetails on
                       user.Id equals dt.UserID
                       where user.Id == checkoutView.DocId
                       select new SummeryviewModel
                       {
                           Country = dt.Country,
                           Day = checkoutView.SelectedDay,
                           Date = System.DateTime.Now,
                           Email = user.Email,
                           Fee = dt.DoctorFee,
                           TotalFee = (dt.DoctorFee + sum),
                           Image = user.Image,
                           Name = user.FirstName + " " + user.LastName,
                           State= dt.State,
                           SelectTime= checkoutView.SelectedSlot,
                           Docid = checkoutView.DocId,
                           ServiceList = checkoutView.SelectedCheckboxList
                       }).FirstOrDefault();
            return View(obj);
        }
        [HttpPost]
        public IActionResult checkout(CheckOutDTO checkOut)
        {
            string userid = _userManager.GetUserId(User);
            PatientsOrder patients = new PatientsOrder
            {
                DocID = checkOut.Docid,
                OrderDate =System.DateTime.Now,
                SelectDay =checkOut.Day,
                SelectTime = checkOut.SelectTime,
                Status ="Confirm",
                TotalPay =checkOut.Fee,
                UserID = userid,
            };
            _context.PatientsOrders.Add(patients);
            _context.SaveChanges();

            var car = new CardDetails
            {
                CardNumber = checkOut.CardNumber,
                CVV = checkOut.CardCVV,
                ExpiryDate = checkOut.CardExpiryDate,
                Name =checkOut.CardHolderName,
                OrderID = patients.Id,

            };
            var selectedServices = checkOut.ServiceList?.Split(',')
                                                        .Select(int.Parse)
                                                        .ToList() ?? new List<int>();

            for (int i = 0; i < selectedServices.Count; i++)
            {
                var ptd = new PatientsServicesDetails
                {
                    servicesID = selectedServices[i],
                    Orderid = patients.Id,  
                };
                _context.PatientsServicesDetails.Add(ptd);
                _context.SaveChanges();

            }
           
            _context.CardDetails.Add(car);
            _context.SaveChanges();

            return RedirectToAction("BookingSuccess", new { id = checkOut.Docid, time = checkOut.SelectTime }) ;
        }
        public IActionResult BookingSuccess(string id, string time)
        {
            var obj = _context.Users.Where(x => x.Id == id).FirstOrDefault();
            ViewBag.user = obj.FirstName + " " + obj.LastName;
            ViewBag.Time = time;

            return View();
        }
        public IActionResult PatientDashboard()
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                

            }
            var getData = (from user in _context.Users
                           join
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
                               DOB = user.DOB,
                               PhoneNumber = user.PhoneNumber,
                               DoctorAvailability = ud.DoctorAvailability,
                               Image = user.Image,
                               Adress = ud.Adress,
                               City = ud.City,
                               Country = ud.Country,
                               DoctorFee = ud.DoctorFee,
                               PostalCode = ud.PostalCode,
                               State = ud.State,
                               specialitiesId = user.specialitiesId,
                               UserId = user.Id,
                               dtId = ud.id,
                               BloodGroup = user.BloodGroup,
                           }).FirstOrDefault();
            return View(getData);
           
        }
        [HttpPost]
        public async Task< IActionResult> PatientDashboard(UserDto userDto)
        {
            var user = await _userManager.FindByIdAsync(userDto.UserId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            if (userDto.ProductFile != null)
            {
                string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string webRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(userDto.ProductFile.FileName);
                string extension = Path.GetExtension(userDto.ProductFile.FileName);
                string uniqueFileName = fileName + "_" + dateTime + extension;

                userDto.Image = user.Image = uniqueFileName;
                string filePath = Path.Combine(webRootPath, "UserImage", uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    userDto.ProductFile.CopyTo(fileStream);
                }
            }

            user.PhoneNumber = userDto.PhoneNumber;
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.BloodGroup = userDto.BloodGroup;
            user.DOB = userDto.DOB;
            user.Gender = userDto.Gender;
            user.Image = userDto.Image;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {

                var dt = new DoctorContactDetails
                {
                    Adress = userDto.Adress,
                    City = userDto.City,
                    Country = userDto.Country,
                   
                    id = userDto.dtId,
                    PostalCode = userDto.PostalCode,
                    State = userDto.State,
                    UserID = userDto.UserId,

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
        public async Task<IActionResult> UpdatePassword(UpdatePasswordDTO updatePassword)
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
                    return RedirectToAction("PatientDashboard");
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
     

        public IActionResult BookAppointment()
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
           
            var obj = (from po in _context.PatientsOrders
                       join user in _context.Users
                       on po.DocID equals user.Id
                       join sp in _context.Specialities on
                       user.specialitiesId equals sp.Id
                       where po.UserID == userId
                       select new AppointmentViewModel
                       {
                           Amount = po.TotalPay,
                           Image = user.Image,
                           Status = po.Status,
                           Name = user.FirstName + " " + user.LastName, 
                           AppointmentDate= po.OrderDate,
                           BookingDate= po.SelectDay,
                           Special = sp.Name,
                           Time = po.SelectTime,
                           id = po.Id
                       }).ToList();
            return View(obj);
        }
        public IActionResult UpdateStatus(int id)
        {
            var obj = _context.PatientsOrders.Find(id);
            if(obj !=null)
            {
                 obj.Status = "Cancel";
                _context.PatientsOrders.Update(obj);
                _context.SaveChanges();
                return new JsonResult("update Status successfully");
            }
            return new JsonResult("Not update Status successfully");
        }
        public IActionResult Prescription()
        {
            string userId = _userManager.GetUserId(User);
            var objs = _context.Users.Where(x => x.Id == userId).FirstOrDefault();
            if (objs != null)
            {
                ViewBag.Name = objs.FirstName + " " + objs.LastName;
                ViewBag.Image = objs.Image;
                ViewBag.About = objs.AboutMe;

            }
            string userid = _userManager.GetUserId(User);
            var obj = _context.Prescriptions.Where(x=>x.PatientId == userid).ToList();
            return View(obj);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
