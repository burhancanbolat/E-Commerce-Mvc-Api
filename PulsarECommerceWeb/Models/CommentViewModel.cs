using System.ComponentModel.DataAnnotations;

namespace PulsarECommerceWeb.Models;

public class CommentViewModel
{
    [Display(Name = "Yorumunuz")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    [MinLength(4, ErrorMessage = "{0} alanı en az {1} karakter olmalıdır!")]
    public string? Text { get; set; }

    [Display(Name = "Puan")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
    public int Rate { get; set; }

    public Guid ProductId { get; set; }
}
