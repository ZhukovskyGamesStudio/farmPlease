#if YG_PLATFORM
namespace YG.Insides
{
    public partial class OptionalPlatform
    {
        public void LoadRewardedAdv() => YG2.iPlatform.LoadRewardedAdv();
    }
}
#endif