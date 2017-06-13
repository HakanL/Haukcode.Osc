using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Rug.Loading
{
    public sealed class LoadContext
    {
        public struct ErrorLine
        {
            public string Message;

            public Exception Exception;

            public bool IsCritical;

            public StackTrace Trace;

            public bool HasSourceInformation; 

            public string SourceFile;

            public int LineNumber;
            public int LinePosition;

            public override string ToString()
            {
                return Message;
            }
        }

        /// <summary>
        /// The errors encountered while loading.
        /// </summary>
        public readonly List<ErrorLine> Errors = new List<ErrorLine>();

        public void ClearErrors()
        {
            Errors.Clear();
            HasHadCriticalError = false; 
        }

        /// <summary>
        /// The reporter.
        /// </summary>
        public readonly IReporter Reporter;

        /// <summary>
        /// Whether the context has encountered a critical error.
        /// </summary>
        public bool HasHadCriticalError = false;

        public bool RecordStackTrace = false;

        public int CriticalErrorCount
        {
            get
            {
                int count = 0; 
                foreach (ErrorLine error in Errors)
                {
                    if (error.IsCritical == true)
                    {
                        count++; 
                    }
                }

                return count; 
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadContext"/> class.
        /// </summary>
        /// <param name="reporter">The reporter.</param>
        public LoadContext(IReporter reporter)
        {
            Reporter = reporter;
        }

        /// <summary>
        /// Adds the specified error message to the set of errors encountered during loading.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            Error(message, null);
        }

        /// <summary>
        /// Adds the specified error message to the set of errors encountered during loading.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="isCriticalError">if set to <c>true</c> [is critical error].</param>
        /// <param name="exception">The ex.</param>
        public void Error(string message, bool isCriticalError, Exception exception = null)
        {
            Error(message, null, isCriticalError, exception);
        }

        /// <summary>
        /// Adds the specified error message to the set of errors encountered during loading.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="node">XML node that is the source of the error.</param>
        /// <param name="isCriticalError">if set to <c>true</c> [is critical error].</param>
        /// <param name="exception">The ex.</param>
        public void Error(string message, XmlNode node, bool isCriticalError = true, Exception exception = null)
        {
            string sourceFile = null; 
            int lineNumber = 0;
            int linePosition = 0;
            bool hasSourceInformation = false; 

            if (node is XmlElementWithPosition)
            {
                XmlElementWithPosition nodeWithPosition = node as XmlElementWithPosition;

                sourceFile = (nodeWithPosition.OwnerDocument as XmlDocumentWithPosition)?.FileName;

                if (nodeWithPosition.HasLineInfo() == true)
                {
                    hasSourceInformation = true; 
                    lineNumber = nodeWithPosition.LineNumber;
                    linePosition = nodeWithPosition.LinePosition;
                }
            } 

            Errors.Add(new ErrorLine()
            {
                Message = message,
                IsCritical =
                isCriticalError,
                Exception = exception,
                Trace = RecordStackTrace ? new StackTrace() : null, 
                HasSourceInformation = hasSourceInformation, 
                SourceFile = sourceFile, 
                LineNumber = lineNumber, 
                LinePosition = linePosition, 
            });

            HasHadCriticalError |= isCriticalError;
        }

        /// <summary>
        /// Prints errors to the reporter
        /// </summary>
        /// <returns>System.String.</returns>
        public void ReportErrors(bool fullErrors = false)
        {
            foreach (ErrorLine error in Errors)
            {
                if (fullErrors == false || error.Exception == null)
                {
                    if (error.IsCritical == true)
                    {
                        Reporter.PrintError(error.Message);
                    }
                    else
                    {
                        Reporter.PrintWarning(ReportVerbosity.Emphasized, error.Message);
                    } 
                }
                else
                {
                    Reporter.PrintException(error.Exception, error.Message);
                }

                if (error.HasSourceInformation == true)
                {
                    Reporter.PrintNormal($"{error.SourceFile} at line {error.LineNumber} position {error.LinePosition}");
                }

                Reporter.PrintBlankLine(ReportVerbosity.Emphasized); 
            }
        }
    }
}