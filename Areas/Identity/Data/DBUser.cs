using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TWAB.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace TWAB.Areas.Identity;

// Add profile data for application users by adding properties to the DBUser class
public class DBUser : IdentityUser
{
    public string FirstName {  get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate {  get; set; }

    //opcjonalne; wykorzystywane dla rektuterów.
    public string? CompanyName {  get; set; }
    public byte[]? CompanyLogo {  get; set; }
    public LokalizacjaFirmy CompanyLocalization { get; set; }

}