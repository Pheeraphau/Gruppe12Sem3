﻿using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Mvc.Models.Account;

public class RegisterViewModel
{
    [Required]
    [StringLength(50)]
    [Display(Name = "Username")]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }
}