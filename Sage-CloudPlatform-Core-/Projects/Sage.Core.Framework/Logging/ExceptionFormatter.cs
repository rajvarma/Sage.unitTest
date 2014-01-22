namespace Sage.Core.Framework.Logging
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Formats Error strings for Reporting in logs 
    /// </summary>
    internal static class ExceptionFormatter
    {
        /// <summary>
        /// Formats an Exception to a string
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string ExceptionToString(Exception ex)
        {
            var strBuilder = new StringBuilder();

            // Log the inner exceptions
            if (ex.InnerException != null)
            {
                strBuilder.AppendLine("(Inner Exception)");
                strBuilder.AppendLine(ExceptionToString(ex.InnerException));
                strBuilder.AppendLine("(Outer Exception)");
            }

            strBuilder.Append("Exception Source:      ");
            try
            {
                strBuilder.AppendLine(ex.Source);
            }
            catch (Exception e)
            {
                strBuilder.AppendLine(e.Message);
            }

            strBuilder.Append("Exception Type:        ");
            try
            {
                strBuilder.AppendLine(ex.GetType().FullName);
            }
            catch (Exception e)
            {
                strBuilder.AppendLine(e.Message);
            }
            strBuilder.Append("Exception Message:     ");
            try
            {
                strBuilder.Append(ex.Message);
            }
            catch (Exception e)
            {
                strBuilder.Append(e.Message);
            }

            try
            {
                var stackTrace = new StackTrace(ex, true);
                strBuilder.AppendLine(StackTraceToString(stackTrace));
            }
            catch (Exception e)
            {
                strBuilder.AppendLine(e.Message);
            }
            return strBuilder.ToString();
        }

        ///// <summary>
        ///// Converts The entire stack trace to string for logging
        ///// </summary>
        ///// <param name="stackTrace"></param>
        ///// <returns></returns>
        public static string StackTraceToString(StackTrace stackTrace)
        {
            var sb = new StringBuilder();
            sb.AppendLine("---- Stack Trace ----");

            var stackFrames = stackTrace.GetFrames();
            if ( stackFrames != null )
            foreach (StackFrame sf in stackFrames)
            {
                sb.Append(StackFrameToString(sf));
            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        ///// <summary>
        ///// Converts the StackFrame into a string
        ///// </summary>
        ///// <param name="sf"></param>
        ///// <returns></returns>
        public static string StackFrameToString(StackFrame sf)
        {
            var sb = new StringBuilder();
            MemberInfo mi = sf.GetMethod();

            if (mi.DeclaringType != null)
            {
                // Write out the method with name space
                sb.AppendFormat("   {0}.{1}.{2}", mi.DeclaringType.Namespace, mi.DeclaringType.Name, mi.Name);
            }

            // Write out the parameters to the method
            int nIndex = 0;
            sb.Append("(");
            foreach (ParameterInfo param in sf.GetMethod().GetParameters())
            {
                if (nIndex > 0)
                    sb.Append(", ");

                sb.AppendFormat("{0} {1}",
                    param.ParameterType.Name,
                    param.Name);
                nIndex++;
            }
            sb.Append(")");

            sb.AppendFormat(" in {0}", mi.Module.Name);
            // if source code is available, append location info
            if (!string.IsNullOrEmpty(sf.GetFileName()))
            {
                sb.AppendFormat(",file: {0},line: {1:#0000}",
                    System.IO.Path.GetFileName(sf.GetFileName()),
                    sf.GetFileLineNumber());
            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
    }
}
