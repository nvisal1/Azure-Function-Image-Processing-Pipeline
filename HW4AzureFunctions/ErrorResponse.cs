using System;
using System.Collections.Generic;
using System.Text;

namespace HW4AzureFunctions
{
    public class ErrorResponse
    {
        public int ErrorNumber { get; set; }

        public string ParameterName { get; set; }

        public string ParameterValue { get; set; }

        public string ErrorDescription { get; set; }
    }
}
