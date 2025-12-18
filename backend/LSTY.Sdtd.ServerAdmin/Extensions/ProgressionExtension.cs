namespace LSTY.Sdtd.ServerAdmin.Extensions
{
    internal static class ProgressionExtension
    {
        public static List<PlayerSkillDto> ToPlayerSkills(this Progression progression, Language language)
        {
            var result = new List<PlayerSkillDto>();

            // var attributesMap = Progression.ProgressionClasses.Where(i => i.Value.Type == ProgressionType.Attribute);
            foreach (var item in Progression.ProgressionClasses.Values)
            {
                if (item.Type != ProgressionType.Attribute)
                {
                    continue;
                }

                var progressionValue = progression.GetProgressionValue(item.Name);
                var playerSkill = new PlayerSkillDto()
                {
                    Name = item.Name,
                    LocalizationName = Utils.GetLocalization(item.NameKey, language, true),
                    LocalizationDesc = Utils.GetLocalization(item.DescKey, language, true),
                    LocalizationLongDesc = Utils.GetLocalization(item.LongDescKey, language, true),
                    Level = progressionValue.Level,
                    MinLevel = item.MinLevel,
                    MaxLevel = item.MaxLevel,
                    CostForNextLevel = progressionValue.costForNextLevel,
                    IconName = item.Icon,
                    Type = item.Type.ToString(),
                    Children = GetChildren(progression, item, language),
                };
                result.Add(playerSkill);
            }

            return result;
        }

        private static List<PlayerSkillDto> GetChildren(Progression progression, ProgressionClass parent, Language language)
        {
            var result = new List<PlayerSkillDto>();
            foreach (var child in Progression.ProgressionClasses.Values)
            {
                if (child.ParentName != null && child.ParentName == parent.Name)
                {
                    var childProgressionValue = progression.GetProgressionValue(child.Name);
                    var childPlayerSkill = new PlayerSkillDto()
                    {
                        Name = child.Name,
                        LocalizationName = Utils.GetLocalization(child.NameKey, language, true),
                        LocalizationDesc = Utils.GetLocalization(child.DescKey, language, true),
                        LocalizationLongDesc = Utils.GetLocalization(child.LongDescKey, language, true),
                        Level = childProgressionValue.Level,
                        MinLevel = child.MinLevel,
                        MaxLevel = child.MaxLevel,
                        CostForNextLevel = childProgressionValue.costForNextLevel,
                        IconName = child.Icon,
                        Type = child.Type.ToString(),
                        Children = GetChildren(progression, child, language),
                    };
                    result.Add(childPlayerSkill);
                }
            }

            return result;
        }
    }
}
