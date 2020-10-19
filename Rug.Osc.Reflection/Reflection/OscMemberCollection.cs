using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rug.Osc.Reflection
{
    public class OscMemberCollection : ReadOnlyCollection<IOscMember>
    {
        public OscMemberCollection(IList<IOscMember> list) : base(list) { }
    }

    public class OscMemberCollection<T> : ReadOnlyCollection<T> where T : IOscMember
    {
        public OscMemberCollection(IList<T> list) : base(list) { }
    }
}
