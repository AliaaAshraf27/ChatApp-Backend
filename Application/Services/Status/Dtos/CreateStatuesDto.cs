using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Status.Dtos
{
    public class CreateStatuesDto
    {
       public string? Content { get; set; }
       public IFormFile? Media { get; set; }
       public string? BackgroundColor { get; set; }
       public string? FontStyle { get; set; }
      
    }
}
