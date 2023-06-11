using PulsarECommerceData;
using System.ComponentModel.DataAnnotations;

namespace PulsarECommerceWeb.Models;

public class ResetPasswordFormViewModel
{
    public Guid UserId { get; set; }

    [Display(Name = "Kod")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    public string? Token { get; set; }

    [Display(Name = "Parola")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Parola Tekrar")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "{0} ile {1} alanı aynı olmalıdır!")]
    public string? PasswordCheck { get; set; }

}
