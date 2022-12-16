using System;
using UnityEngine;

#if UNITY_2017_1_OR_NEWER
namespace Altimit.UI.Unity
{
    [ExecuteInEditMode]
    public class ParentObserver : MonoBehaviour
    {

        public Action<GameObject> onUpdate;
        public Action<GameObject> onUpdateSingle;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        public void OnTransformParentChanged()
        {
            if (onUpdate != null)
                onUpdate(gameObject);
            if (transform.parent != null && onUpdateSingle != null)
            {
                onUpdateSingle(gameObject);
                onUpdateSingle = null;
            }
        }
    }
}
#endif
