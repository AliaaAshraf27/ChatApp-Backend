using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Otp.Dto
{
    public class VerifyOtpRequestDto
    {
        public string PhoneNumber { get; set; }
        public string Code { get; set; }
    }
}
