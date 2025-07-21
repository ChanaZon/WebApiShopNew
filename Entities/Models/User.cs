using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.Models;

public partial class User
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [EmailAddress]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    public string? Salt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();


}
