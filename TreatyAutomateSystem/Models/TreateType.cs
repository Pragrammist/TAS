using System.ComponentModel;

namespace TreatyAutomateSystem.Models;

public enum TreatyType
{
    [Description("Однопрофильный")]
    ONE_PROFILE = default,
    [Description("Многопрофильный")]
    MANY_PROFILES = 1,
}
