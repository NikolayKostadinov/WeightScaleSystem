namespace WeightScale.Utility.Helpers
{
    using System;
    using System.Text;

    public static class ExceptionExtensions 
    {
        public static string ToMessageAndCompleteStacktrace(this Exception exception) 
        {
            Exception e = exception;
            StringBuilder s = new StringBuilder();
            while (e != null) 
            {
                s.AppendLine("Exception type: " + e.GetType().FullName);
                s.AppendLine("Message       : " + e.Message);
                s.AppendLine("Stacktrace:");
                s.AppendLine(e.StackTrace);
                s.AppendLine();
                e = e.InnerException;
            }
            return s.ToString();
        }

        public static string ToMessage(this Exception exception) 
        {
            Exception e = exception;
            StringBuilder s = new StringBuilder();
            while (e != null) 
            {
                s.AppendLine("Exception type: " + e.GetType().FullName);
                s.AppendLine("Message       : " + e.Message);
                s.AppendLine();
                e = e.InnerException;
            }
            return s.ToString();
        }
    }
}
