using System;

namespace Rug.Osc.Reflection
{
    public static class OscMemberAdapterFactory
    {
        public static IOscMemberAdapter Create(string oscAddress, object instance, IOscMember member)
        {
            if (member is IOscOutput)
            {
                return CreateOutputAdapter(oscAddress, instance, member as IOscOutput);
            }
            else if (member is IOscMethod)
            {
                return CreateMethodAdapter(oscAddress, instance, member as IOscMethod);
            }
            else if (member is IOscGetSetReference)
            {
                return CreateGetSetReferenceAdapter(oscAddress, instance, member as IOscGetSetReference);
            }
            else if (member is IOscGetSet)
            {
                return CreateGetSetAdapter(oscAddress, instance, member as IOscGetSet);
            }

            throw new NotImplementedException();
        }

        private static IOscMemberAdapter CreateOutputAdapter(string oscAddress, object instance, IOscOutput oscOutput)
        {
            return new OscOutputAdapter(oscAddress, instance, oscOutput); 
        }

        private static IOscMemberAdapter CreateMethodAdapter(string oscAddress, object instance, IOscMethod oscMethod)
        {
            int argumentCount = oscMethod.ArgumentTypes.Count;

            Type[] types = new Type[argumentCount];

            oscMethod.ArgumentTypes.CopyTo(types, 0);

            switch (argumentCount)
            {
                case 0:
                    return (IOscMemberAdapter)new OscMethodAdapter(oscAddress, instance, oscMethod);

                case 1:
                    return (IOscMemberAdapter)Activator.CreateInstance(typeof(OscMethodAdapter<>).MakeGenericType(types), oscAddress, instance, oscMethod);

                case 2:
                    return (IOscMemberAdapter)Activator.CreateInstance(typeof(OscMethodAdapter<,>).MakeGenericType(types), oscAddress, instance, oscMethod);

                case 3:
                    return (IOscMemberAdapter)Activator.CreateInstance(typeof(OscMethodAdapter<,,>).MakeGenericType(types), oscAddress, instance, oscMethod);

                case 4:
                    return (IOscMemberAdapter)Activator.CreateInstance(typeof(OscMethodAdapter<,,,>).MakeGenericType(types), oscAddress, instance, oscMethod);

                case 5:
                    return (IOscMemberAdapter)Activator.CreateInstance(typeof(OscMethodAdapter<,,,,>).MakeGenericType(types), oscAddress, instance, oscMethod);

                case 6:
                    return (IOscMemberAdapter)Activator.CreateInstance(typeof(OscMethodAdapter<,,,,,>).MakeGenericType(types), oscAddress, instance, oscMethod);

                case 7:
                    return (IOscMemberAdapter)Activator.CreateInstance(typeof(OscMethodAdapter<,,,,,,>).MakeGenericType(types), oscAddress, instance, oscMethod);

                case 8:
                    return (IOscMemberAdapter)Activator.CreateInstance(typeof(OscMethodAdapter<,,,,,,,>).MakeGenericType(types), oscAddress, instance, oscMethod);

                default:
                    throw new NotImplementedException("Command has too many arguments.");
            }
        }

        private static IOscMemberAdapter CreateGetSetAdapter(string oscAddress, object instance, IOscGetSet oscProperty)
        {
            return (IOscMemberAdapter)Activator.CreateInstance(typeof(OscGetSetAdapter<>).MakeGenericType(oscProperty.ValueType), oscAddress, instance, oscProperty, true);
        }

        private static IOscMemberAdapter CreateGetSetReferenceAdapter(string oscAddress, object instance, IOscGetSetReference oscReferenceProperty)
        {
            return (IOscMemberAdapter)Activator.CreateInstance(typeof(OscGetSetReferenceAdapter<>).MakeGenericType(oscReferenceProperty.ValueType), oscAddress, instance, oscReferenceProperty);
        }
    }
}