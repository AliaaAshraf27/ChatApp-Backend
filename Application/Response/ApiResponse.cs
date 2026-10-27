using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string? Message { get; set; }
        public string Error { get; set; }

        public static ApiResponse<T> SuccessResponse(T? data, string? message = null) =>
          new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> FailureResponse(string error) =>
            new() { Success = false, Error = error };
    }
}
