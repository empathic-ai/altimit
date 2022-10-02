namespace Altimit {

    public class Updateable : IUpdateable
    {
        public Updateable()
        {
            Updater.Instance.AddUpdateable(this);
        }

        public virtual void Update()
        {
        }
    }
}