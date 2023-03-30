using System.ComponentModel;

namespace TreatyAutomateSystem.Models;

public enum StudyConditionType
{
    [Description("бесплатное")]
    Ste = default,
    [Description("платное")]
    Pd  = 1
}
