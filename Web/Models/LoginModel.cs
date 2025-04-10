using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class LoginModel
{
    [Required(ErrorMessage = "Required")]
    [EmailAddress(ErrorMessage = "Invalid Format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Required")]
    public string Password { get; set; } = string.Empty;
}
