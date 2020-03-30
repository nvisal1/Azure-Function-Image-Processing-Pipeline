namespace HW4AzureFunctions
{
    /// <summary>
    /// Contains job status codes
    /// </summary>
    public class JobStatusCodes
    {
        public static readonly int IMAGE_OBTAINED = 1;

        public static readonly int BEING_CONVERTED = 2;

        public static readonly int CONVERT_SUCCESS = 3;

        public static readonly int CONVERT_FAIL = 4;
    }
}
