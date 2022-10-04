namespace Altimit
{
    public interface IBehaviour
    {
        void Awake();
        void Start();
        void Update();
        void LateUpdate();
        void FixedUpdate();
    }
}
