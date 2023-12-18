using UnityEngine;

namespace PartialDeathdrop
{
    public interface Region
    {
        bool InsideFor(Vector3 position);
    }
}