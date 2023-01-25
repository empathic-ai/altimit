using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if UNITY
using UnityEngine;
using UnityEngine.UI;
using Altimit.UI.Unity;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions;

namespace Altimit.UI
{
    [AType]
    public class Line : Control
    {
        public AList<Vector3> Points = new AList<Vector3>();
        protected UILineRenderer lineRenderer { get; private set; }

        public Line() : base()
        {
            lineRenderer = GameObject.AddComponent<UILineRenderer>();
            lineRenderer.LineThickness = 10;

            Points.OnChanged += OnPointsChanged;
        }

        private void OnPointsChanged()
        {
            lineRenderer.Points = Points.Select(x=>new UnityEngine.Vector2(x.x, x.y)).ToArray();
        }
    }
}
#endif