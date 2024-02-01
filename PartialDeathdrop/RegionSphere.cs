using System.Text;
using UnityEngine;

namespace PartialDeathdrop
{
    public struct RegionSphere
    {
        public Vector3 Center;
        public float Radius;

        public RegionSphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Sphere region");
            sb.AppendFormat("Center: {0}\n", Center);
            sb.AppendFormat("Radius: {0:0.000}\n", Radius);
            return sb.ToString();
        }

        public bool InsideFor(Vector3 position)
        {
            var displace = position - Center;
            return displace.sqrMagnitude < Radius * Radius;
        }
    }
}