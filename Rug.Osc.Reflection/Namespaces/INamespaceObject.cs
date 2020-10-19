using Rug.Loading;

namespace Rug.Osc.Namespaces
{
    public interface INamespaceObject : ILoadable
    {
        Name Name { get; }

        void State();
    }
}