namespace HW4AzureFunctions
{
    public class ErrorResponse
    {
        public int ErrorNumber { get; set; }

        public string ParameterName { get; set; }

        public string ParameterValue { get; set; }

        public string ErrorDescription { get; set; }

        public static ErrorResponse New(int ErrorNumber, string ParameterName, string ParameterValue, string ErrorDescription)
        {
            return new ErrorResponse()
            {
                ErrorNumber = ErrorNumber,
                ParameterName = ParameterName,
                ParameterValue = ParameterValue,
                ErrorDescription = ErrorDescription,
            };
        }
    }
}
