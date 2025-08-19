namespace LSTY.Sdtd.ServerAdmin.Extensions
{
    internal static class PlayerProfileExtension
    {
        public static Shared.Models.PlayerProfile ToModel(this PlayerProfile playerProfile)
        {
            return new Shared.Models.PlayerProfile()
            {
                BeardName = playerProfile.BeardName,
                ChopsName = playerProfile.ChopsName,
                EntityClassName = playerProfile.EntityClassName,
                EyeColor = playerProfile.EyeColor,
                HairColor = playerProfile.HairColor,
                HairName = playerProfile.HairName,
                IsMale = playerProfile.IsMale,
                MustacheName = playerProfile.MustacheName,
                ProfileArchetype = playerProfile.ProfileArchetype,
                RaceName = playerProfile.RaceName,
                VariantNumber = playerProfile.VariantNumber
            };
        }
    }
}
