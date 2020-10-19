using System;
using System.Collections.Generic;
using System.Text;
using Rug.Loading;

namespace Rug.Osc.Connection
{
    public enum BindingErrorType
    {
        ObjectNotFound,
        ObjectInvalidType,
        Unspecified,
    }

    public static class OscMessages
    {
        [Flags]
        public enum MemberAccess : byte { Method = 0, Output = 1, Get = 2, Set = 4, GetSet = Get | Set };

        public const string BindingErrorOscAddress = "/error/binding";
        public const string ErrorOscAddress = "/error";
        public const string MethodDescriptorOscAddress = "/method";
        public const string OutputDescriptorOscAddress = "/output";
        public const string PropertyDescriptorOscAddress = "/property";
        public const string TypeDescriptorOscAddress = "/type";
        public const string TypeOfOscAddress = "/type-of";
        public const string UsageOscAddress = "/usage";

        private const string ArgumentTypeArrayMismatch = "Argument types and argument names arrays do not have the same length.";
        private const string BindingObjectDoesNotExist = "Could not bind because no object exists at address.";
        private const string BindingIncompatibleTypes = "Could not bind because the object at the destination is of an incompatible type. Expected {0} found {1}.";
        private const string BindingUnspecified = "Could not bind for an unspecified reason.";
        private const string FriendlyNameType = "type";
        private const string FriendlyNameMethod = "method";
        private const string FriendlyNameOutput = "output";
        private const string FriendlyNameProperty = "property";
        private const string FriendlyNamePropertyGet = "get";
        private const string FriendlyNamePropertySet = "set";
        private const string FriendlyNamePropertyGetSet = "get-set";

        public static OscMessage BindingError(BindingErrorType errorType, string fromAddress, Type expectedType, string toAddress, Type foundType)
        {
            switch (errorType)
            {
                case BindingErrorType.ObjectNotFound:
                    return new OscMessage(ErrorOscAddress, errorType.ToString(), BindingObjectDoesNotExist, fromAddress, toAddress);

                case BindingErrorType.ObjectInvalidType:
                    return new OscMessage(ErrorOscAddress, errorType.ToString(), string.Format(BindingIncompatibleTypes, Loader.GetTypeName(expectedType), Loader.GetTypeName(foundType)), fromAddress, toAddress, Loader.GetTypeName(expectedType), Loader.GetTypeName(foundType));

                default:
                    return new OscMessage(ErrorOscAddress, BindingErrorType.Unspecified.ToString(), BindingUnspecified, fromAddress, toAddress);
            }
        }

        public static OscMessage Error(string messageFormat, params object[] arguments)
        {
            return new OscMessage(ErrorOscAddress, "", string.Format(messageFormat, arguments));
        }

        public static OscMessage MethodDescriptor(string typeName, string memberAddress, IList<Type> argumentTypes, IList<string> argumentNames)
        {
            if (argumentTypes.Count != argumentNames.Count)
            {
                throw new OscCommunicationException(ArgumentTypeArrayMismatch);
            }

            string[] arguments = new string[2 + argumentTypes.Count];

            int argumentIndex = 0;

            arguments[argumentIndex++] = typeName;
            arguments[argumentIndex++] = memberAddress;

            for (int i = 0; i < argumentTypes.Count; i++)
            {
                arguments[argumentIndex++] = Loader.GetTypeName(argumentTypes[i]) + ":" + argumentNames[i];
            }

            return new OscMessage(MethodDescriptorOscAddress, arguments);
        }

        public static OscMessage OutputDescriptor(string typeName, string memberAddress, IList<Type> argumentTypes, IList<string> argumentNames)
        {
            if (argumentTypes.Count != argumentNames.Count)
            {
                throw new OscCommunicationException(ArgumentTypeArrayMismatch);
            }

            string[] arguments = new string[2 + argumentTypes.Count];

            int argumentIndex = 0;

            arguments[argumentIndex++] = typeName;
            arguments[argumentIndex++] = memberAddress;

            for (int i = 0; i < argumentTypes.Count; i++)
            {
                arguments[argumentIndex++] = Loader.GetTypeName(argumentTypes[i]) + ":" + argumentNames[i];
            }

            return new OscMessage(OutputDescriptorOscAddress, arguments);
        }

        public static OscMessage ObjectError(string objectAddress, string messageFormat, params object[] arguments)
        {
            return new OscMessage(ErrorOscAddress, objectAddress, string.Format(messageFormat, arguments));
        }

        public static OscMessage PropertyDescriptor(string typeName, string memberAddress, Type propertyType, bool canRead, bool canWrite)
        {
            MemberAccess propertyAccess = (MemberAccess)(
                (canRead ? MemberAccess.Get : 0) |
                (canWrite ? MemberAccess.Set : 0)
                );

            return new OscMessage(PropertyDescriptorOscAddress, typeName, memberAddress, Loader.GetTypeName(propertyType), propertyAccess.ToString());
        }

        public static OscMessage TypeDescriptor(string typeName)
        {
            return new OscMessage(TypeDescriptorOscAddress, typeName);
        }

        public static OscMessage TypeOf(string address, Type type)
        {
            return new OscMessage(TypeOfOscAddress, address, Loader.GetTypeName(type));
        }

        public static OscMessage Usage(string objectAddress, string helpText, MemberAccess propertyAccess, Type objectType, IList<Type> argumentTypes, IList<string> argumentNames)
        {
            if (argumentTypes.Count != argumentNames.Count)
            {
                throw new OscCommunicationException(ArgumentTypeArrayMismatch);
            }

            string[] arguments = new string[3 + argumentTypes.Count];

            int argumentIndex = 0;

            arguments[argumentIndex++] = objectAddress;
            arguments[argumentIndex++] = string.IsNullOrEmpty(helpText) ? string.Empty : helpText;

            switch (propertyAccess)
            {
                case MemberAccess.Method:
                    arguments[argumentIndex++] = FriendlyNameMethod;
                    break;
                case MemberAccess.Output:
                    arguments[argumentIndex++] = FriendlyNameOutput;
                    break;
                case MemberAccess.Get:
                    arguments[argumentIndex++] = FriendlyNamePropertyGet;
                    break;
                case MemberAccess.Set:
                    arguments[argumentIndex++] = FriendlyNamePropertySet;
                    break;
                case MemberAccess.GetSet:
                    arguments[argumentIndex++] = FriendlyNamePropertyGetSet;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyAccess), propertyAccess, null);
            }

            for (int i = 0; i < argumentTypes.Count; i++)
            {
                arguments[argumentIndex++] = Loader.GetTypeName(argumentTypes[i]) + ":" + argumentNames[i];
            }

            return new OscMessage(UsageOscAddress, arguments);
        }

        public static OscMessage Usage(string objectAddress, string helpText, MemberAccess propertyAccess, Type objectType, Type[] argumentTypes, string[] argumentNames)
        {
            if (argumentTypes.Length != argumentNames.Length)
            {
                throw new OscCommunicationException(ArgumentTypeArrayMismatch);
            }

            string[] arguments = new string[3 + argumentTypes.Length];

            int argumentIndex = 0;

            arguments[argumentIndex++] = objectAddress;
            arguments[argumentIndex++] = string.IsNullOrEmpty(helpText) ? string.Empty : helpText;

            switch (propertyAccess)
            {
                case MemberAccess.Method:
                    arguments[argumentIndex++] = FriendlyNameMethod;
                    break;
                case MemberAccess.Output:
                    arguments[argumentIndex++] = FriendlyNameOutput;
                    break;
                case MemberAccess.Get:
                    arguments[argumentIndex++] = FriendlyNamePropertyGet;
                    break;
                case MemberAccess.Set:
                    arguments[argumentIndex++] = FriendlyNamePropertySet;
                    break;
                case MemberAccess.GetSet:
                    arguments[argumentIndex++] = FriendlyNamePropertyGetSet;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(propertyAccess), propertyAccess, null);
            }

            for (int i = 0; i < argumentTypes.Length; i++)
            {
                arguments[argumentIndex++] = Loader.GetTypeName(argumentTypes[i]) + ":" + argumentNames[i];
            }

            return new OscMessage(UsageOscAddress, arguments);
        }

        public static class Print
        {
            public static void BindingError(IReporter reporter, OscMessage message)
            {
                string errorType;
                string messageText;
                string fromAddress;
                string toAddress;
                bool hasTypes;
                string expectedType;
                string foundType;

                if (Parse.BindingError(message, out errorType, out messageText, out fromAddress, out toAddress, out hasTypes, out expectedType, out foundType) == false)
                {
                    reporter.PrintError(Direction.Receive, message.Origin, $@"Error parsing ""{BindingErrorOscAddress}"" message.");
                    return;
                }

                if (hasTypes == false)
                {
                    reporter.PrintError(Direction.Receive, errorType, "{0} ({1} -> {2})", messageText, fromAddress, toAddress);
                }
                else
                {
                    reporter.PrintError(Direction.Receive, errorType, "{0} ([{3}]{1} -> [{4}]{2})", messageText, fromAddress, toAddress, expectedType, foundType);
                }
            }

            public static void Error(IReporter reporter, OscMessage message)
            {
                string messageString, oscAddress;
                bool hasAddress;

                if (Parse.Error(message, out messageString, out hasAddress, out oscAddress) == false)
                {
                    reporter.PrintError(Direction.Receive, message.Origin, $@"Error parsing ""{ErrorOscAddress}"" message.");
                    return;
                }

                if (hasAddress == true)
                {
                    reporter.PrintError(Direction.Receive, oscAddress, messageString);
                }
                else
                {
                    reporter.PrintError(messageString);
                }
            }

            public static void MethodDescriptor(IReporter reporter, OscMessage message)
            {
                string typeName, memberAddress;
                string[] argumentTypes, argumentNames;

                if (Parse.OutputDescriptor(message, out typeName, out memberAddress, out argumentTypes, out argumentNames) == false)
                {
                    reporter.PrintError(Direction.Receive, message.Origin, $@"Error parsing ""{MethodDescriptorOscAddress}"" message.");
                    return;
                }

                string arguments = CreateArgumentString(argumentTypes, argumentNames);

                reporter.PrintEmphasized(Direction.Receive, typeName, "{0} {1} {2}", FriendlyNameMethod, memberAddress, arguments);
            }

            public static void OutputDescriptor(IReporter reporter, OscMessage message)
            {
                string typeName, memberAddress;
                string[] argumentTypes, argumentNames;

                if (Parse.OutputDescriptor(message, out typeName, out memberAddress, out argumentTypes, out argumentNames) == false)
                {
                    reporter.PrintError(Direction.Receive, message.Origin, $@"Error parsing ""{OutputDescriptorOscAddress}"" message.");
                    return;
                }

                string arguments = CreateArgumentString(argumentTypes, argumentNames);

                reporter.PrintEmphasized(Direction.Receive, typeName, "{0} {1} {2}", FriendlyNameOutput, memberAddress, arguments);
            }

            public static void TypeOf(IReporter reporter, OscMessage message)
            {
                string address, typeName;

                if (Parse.TypeOf(message, out address, out typeName) == false)
                {
                    reporter.PrintError(Direction.Receive, message.Origin, $@"Error parsing ""{TypeOfOscAddress}"" message.");
                    return;
                }

                reporter.PrintEmphasized(Direction.Receive, address, typeName);
            }

            public static void PropertyDescriptor(IReporter reporter, OscMessage message)
            {
                string typeName, memberAddress, propertyType;
                MemberAccess memberAccess;

                if (Parse.PropertyDescriptor(message, out typeName, out memberAddress, out propertyType, out memberAccess) == false)
                {
                    reporter.PrintError(Direction.Receive, message.Origin, $@"Error parsing ""{PropertyDescriptorOscAddress}"" message.");
                    return;
                }
                
                reporter.PrintEmphasized(Direction.Receive, typeName, "{0} {2} {1} {3}", FriendlyNameProperty, memberAddress, memberAccess, propertyType);
            }

            public static void TypeDescriptor(IReporter reporter, OscMessage message)
            {
                string typeName;
                if (Parse.TypeDescriptor(message, out typeName) == false)
                {
                    reporter.PrintError(Direction.Receive, message.Origin, $@"Error parsing ""{TypeDescriptorOscAddress}"" message.");
                    return;
                }

                reporter.PrintEmphasized(Direction.Receive, FriendlyNameType, typeName);
            }

            public static void Usage(IReporter reporter, OscMessage message)
            {
                string address, helpText, memberType;
                string[] argumentNames, argumentTypes;

                if (Parse.Usage(message, out address, out helpText, out memberType, out argumentTypes, out argumentNames) == false)
                {
                    reporter.PrintError(Direction.Receive, message.Origin, $@"Error parsing ""{UsageOscAddress}"" message.");
                    return;
                }

                string arguments = CreateArgumentString(argumentTypes, argumentNames); 

                reporter.PrintEmphasized(Direction.Receive, memberType, "{0} {1}, {2}", address, arguments, helpText);
            }

            private static string CreateArgumentString(string[] argumentTypes, string[] argumentNames)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < argumentTypes.Length; i++)
                {
                    sb.Append($"{(i == 0 ? "" : ", ")}{argumentTypes[i]} {argumentNames[i]}");
                }

                return $"({sb})";
            }
        }

        public static class Parse
        {
            public static bool BindingError(OscMessage message,
                out string errorType, out string messageText, out string fromAddress, out string toAddress,
                out bool hasTypes, out string expectedType, out string foundType)
            {
                errorType = null;
                messageText = null;
                fromAddress = null;
                toAddress = null;

                hasTypes = false;
                expectedType = null;
                foundType = null;

                if (message.Count < 4)
                {
                    return false;
                }

                int argumentIndex = 0;

                errorType = message[argumentIndex++].ToString();
                messageText = message[argumentIndex++].ToString();
                fromAddress = message[argumentIndex++].ToString();
                toAddress = message[argumentIndex++].ToString();

                if (message.Count >= 6)
                {
                    hasTypes = true;
                    expectedType = message[argumentIndex++].ToString();
                    foundType = message[argumentIndex++].ToString();
                }

                return true;
            }

            public static bool Error(OscMessage message, out string messageString, out bool hasAddress, out string oscAddress)
            {
                messageString = null;
                hasAddress = false;
                oscAddress = null;

                if (message.Count == 0)
                {
                    return false;
                }

                if (message.Count < 2)
                {
                    messageString = message[0].ToString();

                    return true;
                }

                if (message.Count < 3)
                {
                    hasAddress = true;
                    oscAddress = message[0].ToString();
                    messageString = message[1].ToString();

                    return true;
                }

                return false;
            }

            public static bool MethodDescriptor(OscMessage message, out string typeName, out string memberAddress, out string[] argumentTypes, out string[] argumentNames)
            {
                typeName = null;
                memberAddress = null;
                argumentNames = null;
                argumentTypes = null;

                if (message.Count < 2)
                {
                    return false;
                }

                int argumentIndex = 0;

                typeName = message[argumentIndex++].ToString();
                memberAddress = message[argumentIndex++].ToString();

                argumentNames = new string[message.Count - argumentIndex];
                argumentTypes = new string[message.Count - argumentIndex];

                for (int i = 0, ie = message.Count - argumentIndex; i < ie; i++)
                {
                    string[] parts = message[argumentIndex++].ToString().Split(new[] { ':' }, 2, StringSplitOptions.None);

                    argumentTypes[i] = parts[0];
                    argumentNames[i] = parts[1];
                }

                return true;
            }

            public static bool OutputDescriptor(OscMessage message, out string typeName, out string memberAddress, out string[] argumentTypes, out string[] argumentNames)
            {
                typeName = null;
                memberAddress = null;
                argumentNames = null;
                argumentTypes = null;

                if (message.Count < 2)
                {
                    return false;
                }

                int argumentIndex = 0;

                typeName = message[argumentIndex++].ToString();
                memberAddress = message[argumentIndex++].ToString();

                argumentNames = new string[message.Count - argumentIndex];
                argumentTypes = new string[message.Count - argumentIndex];

                for (int i = 0, ie = message.Count - argumentIndex; i < ie; i++)
                {
                    string[] parts = message[argumentIndex++].ToString().Split(new[] { ':' }, 2, StringSplitOptions.None);

                    argumentTypes[i] = parts[0];
                    argumentNames[i] = parts[1];
                }

                return true;
            }

            public static bool TypeOf(OscMessage message, out string address, out string typeName)
            {
                address = null;
                typeName = null;

                if (message.Count < 2)
                {
                    return false;
                }

                address = message[0].ToString();
                typeName = message[1].ToString();

                return true;
            }

            public static bool PropertyDescriptor(OscMessage message, out string typeName, out string memberAddress, out string propertyType, out MemberAccess propertyAccess)
            {
                typeName = null;
                memberAddress = null;
                propertyType = null;
                propertyAccess = (MemberAccess)0;

                if (message.Count < 4)
                {
                    return false;
                }

                int argumentIndex = 0;

                typeName = message[argumentIndex++].ToString();
                memberAddress = message[argumentIndex++].ToString();
                propertyType = message[argumentIndex++].ToString();

                propertyAccess = (MemberAccess)Enum.Parse(typeof(MemberAccess), message[argumentIndex++].ToString(), true);

                return true;
            }

            public static bool TypeDescriptor(OscMessage message, out string typeName)
            {
                typeName = null;

                if (message.Count < 1)
                {
                    return false;
                }

                typeName = message[0].ToString();

                return true;
            }

            public static bool Usage(OscMessage message, out string address, out string helpText, out string memberType, out string[] argumentTypes, out string[] argumentNames)
            {
                address = null;
                helpText = null;
                memberType = null;
                argumentNames = null;
                argumentTypes = null;

                if (message.Count < 3)
                {
                    return false;
                }

                int argumentIndex = 0;

                address = message[argumentIndex++].ToString();
                helpText = message[argumentIndex++].ToString();
                memberType = message[argumentIndex++].ToString();

                argumentNames = new string[message.Count - argumentIndex];
                argumentTypes = new string[message.Count - argumentIndex];

                for (int i = 0, ie = message.Count - argumentIndex; i < ie; i++)
                {
                    string[] parts = message[argumentIndex++].ToString().Split(new[] { ':' }, 2, StringSplitOptions.None);

                    argumentTypes[i] = parts[0];
                    argumentNames[i] = parts[1];
                }

                return true;
            }
        }
    }
}