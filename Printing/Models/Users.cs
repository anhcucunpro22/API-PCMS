using Printing.AppModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace Printing.Models;

public class Users
{
    [Key]
    public int UserID { get; set; }

    public string? FullName { get; set; }

    public string UserName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            return hash;
        }
    }

    public bool VerifyPassword(string inputPassword, string hashedPassword)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
            var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            return hash == hashedPassword;
        }
    }

    public string Address { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string CodeUser { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public int? OrganizationID { get; set; }

    public bool Isactive { get; set; }

    public void UpdateIsActive(UserUpdateModel updateModel)
    {
        Isactive = updateModel.Isactive;
    }

    public void UserInforUp(UserInforUp userInforUp)
    {
        FullName = userInforUp.FullName;
        Email = userInforUp.Email;
        Address = userInforUp.Address;
        Phone = userInforUp.Phone;
        CodeUser = userInforUp.CodeUser;
        Gender = userInforUp.Gender;
        OrganizationID = userInforUp.OrganizationID;
    }

    public ICollection<Debt> Debts { get; set; } = new List<Debt>();

    [ForeignKey("OrganizationID")]
    public  Organizations? Organization { get; set; }

    public  ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();

    public  ICollection<UserFacilities> UserFacilities { get; set; } = new List<UserFacilities>();

    public  ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
