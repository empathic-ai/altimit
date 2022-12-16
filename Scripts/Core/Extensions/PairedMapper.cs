namespace Altimit
{
    public class PairedMapper<T> : PairedDictionary<int, T>
    {
        public T GetByID(int id)
        {
            T value;
            if (!base.TryGetByFirst(id, out value))
                return default(T);

            return value;
        }

        public int GetByValue(T value)
        {
            int id;
            if (value == null || !base.TryGetBySecond(value, out id))
                return -1;

            return id;
        }
    }
}