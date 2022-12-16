using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Altimit;
#if UNITY_2017_1_OR_NEWER
using UnityEngine;

namespace Altimit.UI
{
    [AType]
    public class NodeMonoBehaviour : MonoBehaviour
    {
        [AProperty]
        public Node Node { get; set; }
    }
}
#endif