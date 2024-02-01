using System.Text;

namespace PartialDeathdrop
{
    public class RegionPolicy
    {
        public RegionSphere ValidRegion;
        public Policy PolicyUsing;

        public RegionPolicy()
        {
            
        }
        
        public RegionPolicy(RegionSphere validRegion)
        {
            ValidRegion = validRegion;
            PolicyUsing = Policy.FromServerPresent();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Region:");
            sb.Append(ValidRegion.ToString());
            sb.AppendLine("Policy:");
            sb.Append(PolicyUsing.ToString());
            return sb.ToString();
        }
    }
}