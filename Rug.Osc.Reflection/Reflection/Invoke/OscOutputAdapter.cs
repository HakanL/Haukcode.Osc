using Rug.Osc.Connection;

namespace Rug.Osc.Reflection
{
    internal class OscOutputAdapter : IOscMemberAdapter
    {
        public readonly string OscAddress;
        public readonly object Instance;
        public readonly IOscOutput OscOutput;

        public OscOutputAdapter(string oscAddress, object instance, IOscOutput output)
        {
            OscAddress = oscAddress;

            Instance = instance;

            OscOutput = output;
        }

        public void Dispose()
        {

        }

        public void State()
        {

        }

        public void Invoke(OscMessage message)
        {
            OscConnection.Send(OscMessages.ObjectError(OscAddress, "Output members cannot be invoked.")); 
        }

        public virtual void Usage()
        {
            OscConnection.Send(OscMessages.Usage(OscAddress, OscOutput.Usage, OscMessages.MemberAccess.Output, Instance.GetType(), OscOutput.ArgumentTypes, OscOutput.ArgumentNames));
        }
        
        public void TypeOf()
        {
            OscConnection.Send(OscMessages.TypeOf(OscAddress, typeof(void)));
        }
    }
}
