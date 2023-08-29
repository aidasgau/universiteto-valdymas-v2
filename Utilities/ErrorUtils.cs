using System.Data.Common;

namespace project_mvc.Utilities
{
    public static class ErrorUtils
    {
        public static string GetErrorMessage(Exception ex, string tableName)
        {
            if (ex is DbException)
            {
                return $"An error occurred while retrieving {tableName} from the database.";
            }
            else if (ex is HttpRequestException)
            {
                var httpRequestException = ex as HttpRequestException;
                return httpRequestException.Message;
            }
            else
            {
                return "An unexpected error occurred.";
            }
        }
    }
}