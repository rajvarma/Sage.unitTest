namespace Sage.Core.Framework.Logging
{
    using System;
    using System.Diagnostics;
    using System.Text;

    internal class LogMessage
    {
        public TraceEventType EventType { get; set; }
        public Exception Exception { get; set;  }
        public string Message { get; set; }
        public Guid TenantId { get; set;  }
        public Guid UserId { get; set;  }

        public LogMessage(TraceEventType eventType, string message, Exception err)
            : this(eventType, message, null, null, err)
        {                       
        }

        public LogMessage(TraceEventType eventType, string message, Guid? tenantId, Guid? userId, Exception err)
        {
            this.EventType = eventType;
            this.Message = message;
            this.TenantId = tenantId ?? Guid.Empty;
            this.UserId = userId ?? Guid.Empty;
            this.Exception = err;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            string exceptionText = string.Empty;
            if (this.Exception != null)
            {
                exceptionText = ExceptionFormatter.ExceptionToString(this.Exception);
            }

            sb.AppendFormat(LoggingResources.LogMessageFormat, this.EventType.ToString(), this.TenantId, this.UserId, this.Message, exceptionText);
            return sb.ToString();
        }
    }
}
