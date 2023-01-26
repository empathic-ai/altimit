using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Altimit;
#if UNITY_64
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