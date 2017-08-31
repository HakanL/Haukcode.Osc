using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using Rug.Loading;
using Rug.Osc.Reflection;

namespace Rug.Osc.Namespaces.Tests
{
    public class DebugReporter : IReporter
    {
        public IOscMessageFilter OscMessageFilter { get; set; }

        public ReportVerbosity ReportVerbosity { get; set; }

        public void PrintBlankLine(ReportVerbosity verbosity)
        {
            Debug.Print("");
        }

        public void PrintDebug(string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintDebug(Direction direction, string ident, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintDebug(Direction direction, IPEndPoint endPoint, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintDetail(string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintDetail(Direction direction, string ident, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintDetail(Direction direction, IPEndPoint endPoint, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintEmphasized(string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintEmphasized(Direction direction, string ident, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintEmphasized(Direction direction, IPEndPoint endPoint, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintError(string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintError(Direction direction, string ident, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintError(Direction direction, IPEndPoint endPoint, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintException(Exception ex, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintNormal(string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintNormal(Direction direction, string ident, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintNormal(Direction direction, IPEndPoint endPoint, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintOscPackets(Direction direction, params OscPacket[] packets)
        {
            //Debug.Print(format, args);
        }

        public void PrintOscPackets(Direction direction, IPEndPoint endPoint, params OscPacket[] packets)
        {
            //Debug.Print(format, args);
        }

        public void PrintWarning(ReportVerbosity verbosity, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintWarning(ReportVerbosity verbosity, Direction direction, string ident, string format, params object[] args)
        {
            Debug.Print(format, args);
        }

        public void PrintWarning(ReportVerbosity verbosity, Direction direction, IPEndPoint endPoint, string format, params object[] args)
        {
            Debug.Print(format, args);
        }
    }
}
