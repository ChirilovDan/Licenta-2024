#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DoctorAppointment.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }   
    public string LastName { get; set; }    
    public string Gender {  get; set; } 
    public DateTime DOB { get; set; }
    public string Image {  get; set; }  
    public string AboutMe {  get; set; }    
    public int specialitiesId {  get; set; }  
    
    public string BloodGroup {  get; set; }
}

