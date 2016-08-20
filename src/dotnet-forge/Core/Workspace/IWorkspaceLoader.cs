using Microsoft.CodeAnalysis;

namespace Forge
{
    public interface IWorkspaceLoader<T> where T : Workspace
    {
        T Load();
    }
}