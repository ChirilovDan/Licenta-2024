using DoctorAppointment.Areas.Identity.Data;
using DoctorAppointment.DTO;
using DoctorAppointment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace DoctorAppointment.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _hostEnvironment;
        public LoginController(ApplicationDbContext context, UserManager<ApplicationUser> manager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = manager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task< IActionResult> Login(LoginDTO obj)
        {

            var remember = true;
            var result = await _signInManager.PasswordSignInAsync(obj.Email, obj.Password, remember, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(obj.Email);
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("SuperAdmin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (roles.Contains("Doctor"))
                {
                    return RedirectToAction("Index", "Doctor");
                }
                else if (roles.Contains("Patience"))
                {
                    return RedirectToAction("PatientDashboard", "Home");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(obj);
            }
            return View(obj);
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task< IActionResult> Register(RegisterViewModel user)
        {
            var check = _context.Users.Where(x => x.Email == user.Email).FirstOrDefault();
            if(check == null)
            {
                string dateTime = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                string webRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(user.ProductFile.FileName);
                string extension = Path.GetExtension(user.ProductFile.FileName);
                string uniqueFileName = fileName + "_" + dateTime + extension;

                user.Image = uniqueFileName;
                string filePath = Path.Combine(webRootPath, "UserImage", uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    user.ProductFile.CopyTo(fileStream);
                }

                var app = new ApplicationUser
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    BloodGroup = user.BloodGroup,
                    DOB = user.DOB,
                    Gender = user.Gender,
                    PhoneNumber = user.PhoneNumber,

                    UserName = user.Email,
                    Image = user.Image,
                    Email = user.Email,
                };
                var result = await _userManager.CreateAsync(app, user.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(app, "Patience");

                    var dt = new DoctorContactDetails
                    {
                        Adress = user.Adress,
                        City = user.City,
                        Country = user.Country,
                        PostalCode = user.PostalCode,
                        State = user.State,
                        UserID = app.Id,
                    };
                    _context.DoctorContactDetails.Add(dt);
                    _context.SaveChanges();
                    return RedirectToAction("Login");
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
    }
}
