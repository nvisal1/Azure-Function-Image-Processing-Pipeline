namespace HW4AzureFunctions
{
    /// <summary>
    /// Object used for displaying error information
    /// to a client
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Numeric error the represents the issue
        /// </summary>
        public int ErrorNumber { get; set; }

        /// <summary>
        /// The name of the parameter that has the issue
        /// 
        /// If the error is not tied to a specific parameter, then
        /// this value can be null
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// The value of the parameter that caused the error
        /// 
        /// If the error is not tied to a specific parameter,
        /// then this value can be null
        /// </summary>
        public string ParameterValue { get; set; }

        /// <summary>
        /// A description of the error, not localized, intended
        /// for developer consumption
        /// </summary>
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
