namespace Altimit
{
    public interface IInitializable
    {

        bool IsInitialized { get; }
        void Init();
    }
}