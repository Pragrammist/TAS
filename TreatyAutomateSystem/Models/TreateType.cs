using System.ComponentModel;

namespace TreatyAutomateSystem.Models;

public enum TreateType
{
    ONE_PROFILE = default,
    MANY_PROFILES = 1,
}

public enum PracticeType
{
    [Description("преддипломная практика")]
    Diploma,
    [Description("учебная практика")]
    Learn,
    [Description("производственная практика")]
    Factory
}
