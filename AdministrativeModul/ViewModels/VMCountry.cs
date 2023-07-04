using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace AdministrativeModul.ViewModels
{
    public class VMCountry
    {
        [ValidateNever]
        public int Id { get; set; }
        [ValidateNever]
        public string Code { get; set; }
        [ValidateNever]
        public string Name { get; set; }
    }
}
