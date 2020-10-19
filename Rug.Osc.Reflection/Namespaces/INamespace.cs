using System.Collections.Generic;
using Rug.Loading;
using Rug.Osc.Reflection;

namespace Rug.Osc.Namespaces
{
    public delegate void NamespaceEvent(INamespace @namespace);

    public interface INamespace : ILoadable, System.Collections.IEnumerable
    {
        string Name { get; set; }

        NamespaceRoot NamespaceRoot { get; }

        int Count { get; } 

        string OscAddress { get; }

        INamespace Parent { get; set; }

        INamespaceObject this[string name] { get; }

        event NamespaceEvent Changed;

        void AddRange(IEnumerable<INamespaceObject> objects);

        void Add(INamespaceObject @object);

        INamespaceObject Create(string typeName, string name);

        INamespaceObject Create(OscType type, string name);

        void Destroy(string name);

        void Attach();

        void Detach();

        void Remove(INamespaceObject @object);

        void State();
    }

    public interface INamespace<T> : IEnumerable<T>, INamespace where T : INamespaceObject
    {
        new T this[string name] { get; }

        void Add(T @object);

        new T Create(string typeName, string name);

        new T Create(OscType type, string name);

        void Remove(T @object);
    }
}