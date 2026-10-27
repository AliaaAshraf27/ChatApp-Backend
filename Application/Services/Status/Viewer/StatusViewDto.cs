using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Status.Viewer
{
    public class StatusViewDto
    {
        public string UserName { get; set; }
        public string? Image { get; set; }
        public DateTime ViewedAt { get; set; }
    }
}
