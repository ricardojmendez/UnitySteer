namespace UnitySteer.Attributes
{
    public class AngleCosineAttribute: RangeAttribute 
    {
        public AngleCosineAttribute(): base(-360, 360)
        {
        }

        public AngleCosineAttribute(float min, float max): base(min, max)
        {
        }
    }
}