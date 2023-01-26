using System.Collections;

#if UNITY_64
using TMPro;
using UnityEngine;

namespace Altimit.UI.Unity
{
    public class InputFix : MonoBehaviour
    {
        TMP_InputField input;

        // Use this for initialization
        void Start()
        {
            input = gameObject.AddOrGet<TMP_InputField>();
            //StartCoroutine(CoFix());
        }

        // Update is called once per frame
        void Update()
        {
        }

        IEnumerator CoFix()
        {
            TMP_InputField.LineType oldType = input.lineType;
            input.lineType = oldType.Equals(TMP_InputField.LineType.SingleLine) ? TMP_InputField.LineType.MultiLineNewline : TMP_InputField.LineType.SingleLine;
            yield return new WaitForEndOfFrame();
            input.lineType = oldType;
        }
    }
}
#endif