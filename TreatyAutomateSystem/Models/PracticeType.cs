using System.ComponentModel;

namespace TreatyAutomateSystem.Models;

public enum PracticeType
{
    [Description("преддипломная практика")]
    Diploma = 0,

    [Description("учебная практика")]
    Learn = 1,
    
    [Description("производственная практика")]
    Factory = 2
}
