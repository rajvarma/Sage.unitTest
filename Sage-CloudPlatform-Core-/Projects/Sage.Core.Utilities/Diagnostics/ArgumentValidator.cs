using System.Linq;

namespace Sage.Core.Utilities.Diagnostics
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;

    /// <summary>
    /// Utility class to assist with validating method arguments
    /// </summary>
    public static class ArgumentValidator
    {

        /// <summary>
        /// Make sure a reference argument is non-null
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <exception cref="ArgumentNullException"/>
        public static void ValidateNonNullReference(object argument, string name, string source)
        {
            if (argument == null)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.NullReferenceErrorFormat, name, source);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentNullException(name, errorMessage);
            }
        }

        /// <summary>
        /// Make sure an array argument is both non-null and non-empty
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static void ValidateNonEmptyArray(Array argument, string name, string source)
        {
            ValidateNonNullReference(argument, name, source);
            if (argument.Length == 0)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.EmptyArrayErrorFormat, name, source);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentException(errorMessage, name);
            }
        }

        /// <summary>
        /// Make sure a list argument is both non-null and non-empty
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static void ValidateNonEmptyList(IList argument, string name, string source)
        {
            ValidateNonNullReference(argument, name, source);
            if (argument.Count == 0)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.EmptyListErrorFormat, name, source);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentException(errorMessage, name);
            }
        }

        /// <summary>
        /// Make sure an array argument is non-null and contains exactly the number of specified expected elements.
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <param name="length">The expected number of elements</param>
        public static void ValidateArrayLength(Array argument, string name, string source, int length)
        {
            ValidateNonNullReference(argument, name, source);
            if (argument.Length != length)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.InvalidArrayLengthErrorFormat, name, source, argument.Length, length);
                //                  EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentException(errorMessage, name);
            }
        }

        /// <summary>
        /// Make sue a string argument is both non-null and non-empty
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static void ValidateNonEmptyString(string argument, string name, string source)
        {
            ValidateNonNullReference(argument, name, source);
            if (argument == string.Empty)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.EmptyStringErrorFormat, name, source);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentException(errorMessage, name);
            }
        }

        /// <summary>
        /// Make sure a string is not longer than it's maximum allowed length
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <param name="maximumValue">The Maximum length of the argument</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static void ValidateMaxStringLength(string argument, string name, string source, int maximumValue)
        {
            if (argument != null && argument.Length > maximumValue)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.MaximumStringLengthErrorFormat, name, source, maximumValue);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentOutOfRangeException(name, argument.Length, errorMessage);
            }
        }

        /// <summary>
        /// Make sure a string is not less than it's minimum length
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <param name="minimumValue">The minimum length of the argument</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static void ValidateMinStringLength(string argument, string name, string source, int minimumValue)
        {
            if (argument != null && argument.Length < minimumValue)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.MinimumStringLengthErrorFormat, name, source, minimumValue);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentOutOfRangeException(name, argument.Length, errorMessage);
            }
        }

        /// <summary>
        /// Make sure a string matches the specified regular expression
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <param name="regularExpression">The regular expression to validate against</param>
        /// <param name="message">Regular expression message.</param>
        public static void ValidateStringIsMatchForRegularExpression(string argument, string name, string source, string regularExpression, string message)
        {
            ValidateStringIsMatchForRegularExpression(argument, name, source, regularExpression, message, RegexOptions.None);
        }

        /// <summary>
        /// Make sure a string matches the specified regular expression
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <param name="regularExpression">The regular expression to validate against</param>
        /// <param name="message">Regular expression message.</param>
        /// <param name="regexOptions">Options to use when testing for match</param>
        public static void ValidateStringIsMatchForRegularExpression(string argument, string name, string source, string regularExpression, string message, RegexOptions regexOptions)
        {
            ValidateNonNullReference(argument, name, source);
            if (!Regex.IsMatch(argument, regularExpression, regexOptions))
            {
                ValidateCallerInfo(ref name, ref source);
                var condition = !string.IsNullOrEmpty(message)
                                       ? message
                                       : string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.NoMatchRegExMessage, regularExpression, regexOptions);
                
                var errorMessage = string.Format(Thread.CurrentThread.CurrentCulture,
                                                     DiagnosticsResources.InvalidRegExpresionErrorFormat, source, name,
                                                     argument, condition);

                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentException(errorMessage, name);
            }
        }

        /// <summary>
        /// Make sure a string is suitable to be a Uri used for remoting (i.e., it must be well-formed and non-relative)
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        public static void ValidateStringIsRemotingUri(string argument, string name, string source)
        {
            ValidateNonEmptyString(argument, name, source);

            try
            {
                var uri = new Uri(argument);
                if (!uri.IsAbsoluteUri)
                {
                    ValidateCallerInfo(ref name, ref source);
                    string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.InvalidUriErrorFormat, source, name, argument);
                    //    EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                    throw new ArgumentException(errorMessage, name);
                }
            }
            catch (UriFormatException ex)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.InvalidUriStringErrorFormat, source, name, argument);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentException(errorMessage, name, ex);
            }
        }

        /// <summary>
        /// Make sure a file exists
        /// </summary>
        /// <param name="argument">The argument to validate. It should be the full name and path of a file to check for</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        public static void ValidateFileExists(string argument, string name, string source)
        {
            ValidateNonEmptyString(argument, name, source);

            if (!File.Exists(argument))
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.FileDoesNotExistErrorFormat, argument);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new FileNotFoundException(errorMessage, argument);
            }
        }

        /// <summary>
        /// Make sure a directory exists
        /// </summary>
        /// <param name="argument">The argument to validate. It should be the full name and path of a directory to check for</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        public static void ValidateDirectoryExists(string argument, string name, string source)
        {
            ValidateNonEmptyString(argument, name, source);

            if (!Directory.Exists(argument))
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.DirectoryDoesNotExistErrorFormat, argument);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new DirectoryNotFoundException(errorMessage);
            }
        }

        /// <summary>
        /// Make sure an Integer value is greater then or equal to the minimum value.
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <param name="minimumValue">The minimum value of the argument</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static void ValidateMinIntegerValue(int argument, string name, string source, int minimumValue)
        {
            if (argument < minimumValue)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.BelowMinimumIntErrorFormat, source, name, minimumValue);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentOutOfRangeException(name, argument, errorMessage);
            }

        }

        /// <summary>
        /// Make sure an Integer value is less than or equal to the maximum
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <param name="maximumValue">The maximum value of the argument</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static void ValidateMaxIntegerValue(int argument, string name, string source, int maximumValue)
        {
            if (argument > maximumValue)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.AboveMaximumIntErrorFormat, source, name, maximumValue);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentOutOfRangeException(name, argument, errorMessage);
            }
        }

        /// <summary>
        /// Make sure the content length is less than or equal to the maximum
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <param name="maximumValue">The maximum value of the argument</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static void ValidateMaxContentLength(Stream argument, string name, string source, long maximumValue)
        {
            if (argument.Length > maximumValue)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.AboveMaximumContentErrorFormat, source, name, maximumValue);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentOutOfRangeException(name, argument, errorMessage);
            }
        }

        /// <summary>
        /// Make sure an integer argument is within a specific range
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation check</param>
        /// <param name="minimumValue">The minimum value of the argument</param>
        /// <param name="maximumValue">The maximum value of the argument</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static void ValidateIntegerRange(int argument, string name, string source, int minimumValue, int maximumValue)
        {
            ValidateMinIntegerValue(argument, name, source, minimumValue);
            ValidateMaxIntegerValue(argument, name, source, maximumValue);
        }

        /// <summary>
        /// Make sure an IntPtr is non null
        /// </summary>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation request</param>
        public static void ValidateNonNullIntPtr(IntPtr argument, string name, string source)
        {
            if (argument == IntPtr.Zero)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.NullIntPtrError, name, source);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentNullException(name, errorMessage);
            }
        }

        /// <summary>
        /// Make sure an object is an instance of a particular type
        /// </summary>
        /// <remarks>
        /// This validation passes if the specified type is in the inheritance hierarchy of the
        /// object represented by the argument parameter, or if the specified type is an interface
        /// that is supported by the argument parameter.
        /// </remarks>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation request</param>
        /// <param name="type">The required type of the argument</param>
        public static void ValidateIsInstanceOfType(object argument, string name, string source, Type type)
        {
            ValidateNonNullReference(type, "type", "ArgumentValidtor.ValidateIsInstanceOfType()");

            if (!type.IsInstanceOfType(argument))
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.InvalidTypeErrorFormat, (null != argument) ? argument.GetType().ToString() : "null", source, type);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentException(errorMessage, name);
            }
        }

        /// <summary>
        /// Make sure a type is a subclass of a particular type
        /// </summary>
        /// <remarks>
        /// This validation passes if the specified type and the type specified by the argument
        /// parameter represent classes, and the class represented by the argument parameter
        /// derives from the class represented by specified type parameter.
        /// 
        /// This validation will fail if the specified type and the specified argument represent
        /// the same class.
        /// </remarks>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation request</param>
        /// <param name="type">The required type of the argument</param>
        public static void ValidateIsSubclassOfType(Type argument, string name, string source, Type type)
        {
            ValidateNonNullReference(argument, "type", "ArgumentValidtor.ValidateIsSubclassOfType()");
            ValidateNonNullReference(type, "type", "ArgumentValidtor.ValidateIsInstanceOfType()");

            if (!argument.IsSubclassOf(type))
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.UnexpectedTypeErrorFormat, argument, source, type);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentException(errorMessage, name);
            }
        }

        /// <summary>
        /// Make sure a type is an interface
        /// </summary>
        /// <remarks>
        /// This validation passes if the specified type is an interface.
        /// </remarks>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation request</param>
        public static void ValidateTypeIsInterface(Type argument, string name, string source)
        {
            ValidateNonNullReference(argument, "argument", "ArgumentValidtor.ValidateTypeIsInterface()");

            if (!argument.IsInterface)
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.InterfaceExpectedErrorFormat, argument.FullName, source);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentException(errorMessage, name);
            }
        }

        /// <summary>
        /// Make sure an object is an instance of a particular type
        /// </summary>
        /// <remarks>
        /// This validation passes if the specified type is in the inheritance hierarchy of the
        /// object represented by the argument parameter, or if the specified type is an interface
        /// that is supported by the argument parameter.
        /// </remarks>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation request</param>
        /// <param name="type">The required type of the argument</param>
        public static void ValidateIsAssignableFromType(Type argument, string name, string source, Type type)
        {
            ValidateNonNullReference(type, "type", "ArgumentValidtor.ValidateIsInstanceOfType()");

            if (!argument.IsAssignableFrom(type))
            {
                ValidateCallerInfo(ref name, ref source);
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.InvalidTypeErrorFormat, (null != argument) ? argument.GetType().ToString() : "null", source, type);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentException(errorMessage, name);
            }
        }

        /// <summary>
        /// Make sure an content is an instance of a particular type
        /// </summary>
        /// <remarks>
        /// This validation passes if the specified type is n allowed content type. 
        /// For now, allowed types are Stream, byte[] and text(string).
        /// </remarks>
        /// <param name="argument">The argument to validate</param>
        /// <param name="name">The name of the argument</param>
        /// <param name="source">The source of the validation request</param>
        /// <param name="allowedTypes">Types allowed</param>
        public static void ValidateIsAllowedType(Type argument, string name, string source, params Type[] allowedTypes)
        {
            ValidateNonNullReference(argument, "argument", "ArgumentValidtor.ValidateIsInstanceOfType()");

            if (allowedTypes.All(constraint => constraint != argument))
            {
                ValidateCallerInfo(ref name, ref source);
                string allowedContentTypes = String.Join(", ", allowedTypes.ToArray().Select(x => x.FullName).ToList());
                string errorMessage = string.Format(Thread.CurrentThread.CurrentCulture, DiagnosticsResources.InvalidTypeErrorFormat, argument.FullName, source, allowedContentTypes);
                //EventLogger.WriteMessage(source, errorMessage, MessageType.Error);
                throw new ArgumentException(errorMessage, name);
            }
        }

        #region Private methods
        /// <summary>
        /// Make sure the caller info is ok before generating error messages
        /// </summary>
        /// <param name="name">The argument name</param>
        /// <param name="source">The calling source</param>
        // ReSharper disable once UnusedParameter.Local
        // ReSharper disable once UnusedParameter.Local
        private static void ValidateCallerInfo(ref string name, ref string source)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = "argument";
            }

            if (string.IsNullOrEmpty(source))
            {
                source = "Method";
            }
        }
        #endregion
    }
}
