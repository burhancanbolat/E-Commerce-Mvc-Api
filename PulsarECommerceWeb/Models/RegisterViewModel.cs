using PulsarECommerceData;
using System.ComponentModel.DataAnnotations;

namespace PulsarECommerceWeb.Models;

public class RegisterViewModel
{
    [Display(Name = "E-Posta")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    [DataType(DataType.EmailAddress)]
    [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta yazınız!")]
    public string? UserName { get; set; }

    [Display(Name = "Ad")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    [DataType(DataType.Text)]
    public string? FirstName { get; set; }

    [Display(Name = "Soyad")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    [DataType(DataType.Text)]
    public string? LastName { get; set; }

    [Display(Name = "Cinsiyet")]
    [DataType(DataType.Text)]
    public Genders? Gender { get; set; }

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
