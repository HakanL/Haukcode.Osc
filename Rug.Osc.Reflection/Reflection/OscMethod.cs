using System;
using System.Collections.Generic;
using System.Reflection;
using Rug.Osc.Connection;

namespace Rug.Osc.Reflection
{
    internal class OscMethod : IOscMethod
    {
        private readonly OscMethodAttribute oscMethodAttribute;

        public IList<string> ArgumentNames { get; }

        public IList<Type> ArgumentTypes { get; }

        public MemberInfo MemberInfo => Method; 

        public string MemberName { get; }

        public MethodInfo Method { get; }

        public string OscAddress { get; }

        public OscType OscType { get; }

        public string Usage { get; }

        public OscMethod(OscType type, OscMethodAttribute oscMethodAttribute, MethodInfo methodInfo)
        {
            this.oscMethodAttribute = oscMethodAttribute;

            OscType = type;

            MemberName = methodInfo.Name;
            Method = methodInfo;

            Usage = oscMethodAttribute.Usage;

            OscAddress = string.IsNullOrEmpty(oscMethodAttribute.OscAddress) ? "/" + Helper.ToLowerSplitWithHyphen(methodInfo.Name) : oscMethodAttribute.OscAddress;

            List<Type> types = new List<Type>();
            List<string> names = new List<string>();

            foreach (ParameterInfo parameter in Method.GetParameters())
            {
                names.Add(parameter.Name);
                types.Add(parameter.ParameterType);
            }

            ArgumentTypes = Array.AsReadOnly(types.ToArray());
            ArgumentNames = Array.AsReadOnly(names.ToArray());
        }

        public void Describe()
        {
            OscConnection.Send(OscMessages.MethodDescriptor(OscType.TypeName, OscAddress, ArgumentTypes, ArgumentNames));
        }

        public override string ToString()
        {
            return OscMessages.MethodDescriptor(OscType.TypeName, OscAddress, ArgumentTypes, ArgumentNames).ToString();
        }
    }
}