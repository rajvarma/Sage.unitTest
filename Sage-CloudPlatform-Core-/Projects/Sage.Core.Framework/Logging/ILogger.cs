namespace Sage.Core.Framework.Logging
{
    using System;

    public interface ILogger
    {
        #region Critical method overloads

        /// <summary>
        /// Logs the given messageContent, tenant id, user id and exception with a Severity of Critical.
        /// The entire messageContent and stack trace of the entire exception chain will be logged.  (Each inner exception is logged).  
        /// The given messageContent will have a ": " appended to the end.  For readability
        /// of your log messages, do not end the messageContent with any punctuation characters. 
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tenantId">Tenant ID under whose context the application is running</param>
        /// <param name="userId">User ID under whose context the application is running</param>
        /// <param name="e">The exception to log</param>
        void Critical(string message, Guid tenantId, Guid userId, Exception e = null);

        /// <summary>
        /// Logs the given messageContent and Exception with a severity of Critical. This method is to be used when the TenantId
        /// and UserId is not known.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">The exception to log</param>
        void Critical(string message, Exception e = null);

        #endregion

        #region Error method overloads

        /// <summary>
        /// Logs the given messageContent, tenant id, user id and exception with a Severity of Error.
        /// The entire messageContent and stack trace of the entire exception chain will be logged.  (Each inner exception is logged).  
        /// The given messageContent will have a ": " appended to the end.  For readability
        /// of your log messages, do not end the messageContent with any punctuation characters. 
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tenantId">Tenant ID under whose context the application is running</param>
        /// <param name="userId">User ID under whose context the application is running</param>
        /// <param name="e">The exception to log</param>
        void Error(string message, Guid tenantId, Guid userId, Exception e = null);

        /// <summary>
        /// Logs the given messageContent and Exception with a severity of Error. This method is to be used when the TenantId
        /// and UserId is not known.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">The exception to log</param>
        void Error(string message, Exception e = null);
        
        #endregion

        #region Warning method overloads

        /// <summary>
        /// Logs the given messageContent, tenant id, user id and exception with a Severity of Warning.
        /// The entire messageContent and stack trace of the entire exception chain will be logged.  (Each inner exception is logged).  
        /// The given messageContent will have a ": " appended to the end.  For readability
        /// of your log messages, do not end the messageContent with any punctuation characters. 
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tenantId">Tenant ID under whose context the application is running</param>
        /// <param name="userId">User ID under whose context the application is running</param>
        /// <param name="e">The exception to log</param>
        void Warning(string message, Guid tenantId, Guid userId, Exception e = null);

        /// <summary>
        /// Logs the given messageContent and Exception with a severity of Warning. This method is to be used when the TenantId
        /// and UserId is not known.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">The exception to log</param>
        void Warning(string message, Exception e = null);

        #endregion

        #region Info method overloads

        /// <summary>
        /// Logs the given messageContent, tenant id, user id and exception with a Severity of Info.
        /// The entire messageContent and stack trace of the entire exception chain will be logged.  (Each inner exception is logged).  
        /// The given messageContent will have a ": " appended to the end.  For readability
        /// of your log messages, do not end the messageContent with any punctuation characters. 
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tenantId">Tenant ID under whose context the application is running</param>
        /// <param name="userId">User ID under whose context the application is running</param>
        /// <param name="e">The exception to log</param>
        void Info(string message, Guid tenantId, Guid userId, Exception e = null);

        /// <summary>
        /// Logs the given messageContent and Exception with a severity of Info. This method is to be used when the TenantId
        /// and UserId is not known.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">The exception to log</param>
        void Info(string message, Exception e = null);

        #endregion

        #region Verbose method overloads

        /// <summary>
        /// Logs the given messageContent, tenant id, user id and exception with a Severity of Verbose.
        /// The entire messageContent and stack trace of the entire exception chain will be logged.  (Each inner exception is logged).  
        /// The given messageContent will have a ": " appended to the end.  For readability
        /// of your log messages, do not end the messageContent with any punctuation characters. 
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="tenantId">Tenant ID under whose context the application is running</param>
        /// <param name="userId">User ID under whose context the application is running</param>
        /// <param name="e">The exception to log</param>
        void Verbose(string message, Guid tenantId, Guid userId, Exception e = null);

        /// <summary>
        /// Logs the given messageContent and Exception with a severity of Verbose. This method is to be used when the TenantId
        /// and UserId is not known.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">The exception to log</param>
        void Verbose(string message, Exception e = null);

        #endregion

        /// <summary>
        /// Can be used by the client to determine if the message should be logged.   Use in cases when the client wants to
        /// avoid doing a work to construct a message that won't ever be logged
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        bool ShouldLog(LogLevel level);

    }
}
