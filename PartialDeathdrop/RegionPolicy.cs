namespace PartialDeathdrop
{
    public struct RegionPolicy
    {
        public Region ValidRegion;
        public Policy PolicyUsing;

        public RegionPolicy(Region validRegion)
        {
            ValidRegion = validRegion;
            PolicyUsing = Policy.FromServerPresent();
        }
    }
}