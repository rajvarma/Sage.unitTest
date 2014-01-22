namespace Sage.Core.Framework.Logging
{
    using System;
    using System.Diagnostics;

    using Sage.Core.Framework.Configuration;
    using Sage.Core.Utilities.Diagnostics;

    public class TraceLogger : ILogger
    {
        private const string TraceSourceName = "Sage.Core.Logging.Source";

        private const string TraceSwitchName = "Sage.Core.Logging.Switch";
       
        private readonly TraceSource _traceSource;

        public TraceLogger(IConfigurationManager logConfig)
        {
            ArgumentValidator.ValidateNonNullReference(logConfig,"logConfig","TraceLogger.Ctor()");
          
            this._traceSource = new TraceSource(TraceSourceName) { Switch = new SourceSwitch(TraceSwitchName) };
            LogLevel logLevel;
            Enum.TryParse(logConfig.SystemConfiguration["LogLevel"], out logLevel);

            switch (logLevel)
            {
                case LogLevel.Off:
                    this._traceSource.Switch.Level = SourceLevels.Off;
                    break;
                case LogLevel.Critical:
                    this._traceSource.Switch.Level = SourceLevels.All;
                    break;
                case LogLevel.Error:
                    this._traceSource.Switch.Level = SourceLevels.Critical | SourceLevels.Error; 
                    break;
                case LogLevel.Warning:
                    this._traceSource.Switch.Level = SourceLevels.Critical | SourceLevels.Error | SourceLevels.Warning;
                    break;
                case LogLevel.Information:
                    this._traceSource.Switch.Level = SourceLevels.Critical | SourceLevels.Error | SourceLevels.Warning | SourceLevels.Information;
                    break;
                case LogLevel.Verbose:
                    this._traceSource.Switch.Level = SourceLevels.Critical | SourceLevels.Error | SourceLevels.Warning | SourceLevels.Information | SourceLevels.Verbose;
                    break;
            }
        }

        public void Critical(string message, Guid tenantId, Guid userId, Exception e = null)
        {
            var msg = new LogMessage(TraceEventType.Critical, message, tenantId, userId, e);
            this.LogMessage(msg);
        }

        public void Critical(string message, Exception e = null)
        {
            var msg = new LogMessage(TraceEventType.Critical, message, e);
            this.LogMessage(msg);
        }

        public void Error(string message, Guid tenantId, Guid userId, Exception e = null)
        {
            var msg = new LogMessage(TraceEventType.Error, message, tenantId, userId, e);
            this.LogMessage(msg);
        }

        public void Error(string message, Exception e = null)
        {
            var msg = new LogMessage(TraceEventType.Error, message, e);
            this.LogMessage(msg);
        }

        public void Warning(string message, Guid tenantId, Guid userId, Exception e = null)
        {
            var msg = new LogMessage(TraceEventType.Warning, message, tenantId, userId, e);
            this.LogMessage(msg);
        }

        public void Warning(string message, Exception e = null)
        {
            var msg = new LogMessage(TraceEventType.Warning, message, e);
            this.LogMessage(msg);
        }

        public void Info(string message, Guid tenantId, Guid userId, Exception e = null)
        {
            var msg = new LogMessage(TraceEventType.Information, message, tenantId, userId, e);
            this.LogMessage(msg);
        }

        public void Info(string message, Exception e = null)
        {
            var msg = new LogMessage(TraceEventType.Information, message, e);
            this.LogMessage(msg);
        }

        public void Verbose(string message, Guid tenantId, Guid userId, Exception e = null)
        {
            var msg = new LogMessage(TraceEventType.Verbose, message, tenantId, userId, e);
            this.LogMessage(msg);
        }

        public void Verbose(string message, Exception e = null)
        {
            var msg = new LogMessage(TraceEventType.Verbose, message, e);
            this.LogMessage(msg);
        }

        public bool ShouldLog(LogLevel level)
        {
            TraceEventType eventType;

            switch (level)
            {
                case LogLevel.Off:
                    return false; // should never be passed by the client.
                case LogLevel.Critical:
                    eventType = TraceEventType.Critical;
                    break;
                case LogLevel.Error:
                    eventType = TraceEventType.Error;
                    break;
                case LogLevel.Warning:
                    eventType = TraceEventType.Warning;
                    break;
                case LogLevel.Information:
                    eventType = TraceEventType.Information;
                    break;
                case LogLevel.Verbose:
                    eventType = TraceEventType.Verbose;
                    break;
                default:
                    return true;
            }
            return this._traceSource.Switch.ShouldTrace(eventType);
        }

        private void LogMessage(LogMessage message)
        {
            if (this._traceSource.Switch.ShouldTrace(message.EventType))
            {
                this._traceSource.TraceEvent(message.EventType, 0, message.ToString());
            }
        }

    }
}
