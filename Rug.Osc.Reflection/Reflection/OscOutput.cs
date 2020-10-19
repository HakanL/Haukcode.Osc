using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rug.Osc.Connection;

namespace Rug.Osc.Reflection
{
    internal class OscOutput : IOscOutput
    {
        private readonly OscOutputAttribute oscOutputAttribute;

        public IList<string> ArgumentNames { get; }

        public IList<Type> ArgumentTypes { get; }

        public MemberInfo MemberInfo => null;

        public string MemberName { get; }

        public string OscAddress { get; }

        public OscType OscType { get; }

        public string Usage { get; }

        public OscOutput(OscType type, OscOutputAttribute oscOutputAttribute, MethodInfo methodInfo)
        {
            this.oscOutputAttribute = oscOutputAttribute;

            OscType = type;

            MemberName = methodInfo.Name;
            //Method = methodInfo;

            Usage = oscOutputAttribute.Usage;

            OscAddress = string.IsNullOrEmpty(oscOutputAttribute.OscAddress) ? "/" + Helper.ToLowerSplitWithHyphen(methodInfo.Name) : oscOutputAttribute.OscAddress;

            List<Type> types = new List<Type>();
            List<string> names = new List<string>();

            foreach (ParameterInfo parameter in methodInfo.GetParameters())
            {
                names.Add(parameter.Name);
                types.Add(parameter.ParameterType);
            }

            ArgumentTypes = Array.AsReadOnly(types.ToArray());
            ArgumentNames = Array.AsReadOnly(names.ToArray());
        }

        public void Describe()
        {
            OscConnection.Send(OscMessages.OutputDescriptor(OscType.TypeName, OscAddress, ArgumentTypes, ArgumentNames));
        }

        public override string ToString()
        {
            return OscMessages.OutputDescriptor(OscType.TypeName, OscAddress, ArgumentTypes, ArgumentNames).ToString();
        }
    }
}
