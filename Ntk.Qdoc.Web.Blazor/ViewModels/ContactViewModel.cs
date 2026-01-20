using System.ComponentModel.DataAnnotations;

namespace Ntk.Qdoc.Web.Blazor.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "نام الزامی است")]
        [StringLength(100, ErrorMessage = "نام نباید بیشتر از 100 کاراکتر باشد")]
        public string Name { get; set; }

        [Required(ErrorMessage = "ایمیل الزامی است")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل نامعتبر است")]
        [StringLength(200, ErrorMessage = "ایمیل نباید بیشتر از 200 کاراکتر باشد")]
        public string Email { get; set; }

        [Required(ErrorMessage = "موضوع الزامی است")]
        [StringLength(200, ErrorMessage = "موضوع نباید بیشتر از 200 کاراکتر باشد")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "پیام الزامی است")]
        [StringLength(2000, ErrorMessage = "پیام نباید بیشتر از 2000 کاراکتر باشد")]
        public string Message { get; set; }
    }
}
