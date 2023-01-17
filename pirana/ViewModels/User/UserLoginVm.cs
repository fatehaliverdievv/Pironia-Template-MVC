using System.ComponentModel.DataAnnotations;

namespace pirana.ViewModels
{
    public class UserLoginVm
    {
        public string UsernameorEmail { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsPersistance { get; set; }
    }
}
