using UnityEngine;

namespace PartialDeathdrop
{
    public struct RegionSphere : Region
    {
        public Vector3 Center;
        public float Radius;

        public RegionSphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public bool InsideFor(Vector3 position)
        {
            var displace = position - Center;
            return displace.sqrMagnitude < Radius;
        }
    }
}