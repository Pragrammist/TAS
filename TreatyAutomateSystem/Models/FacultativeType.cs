using System.ComponentModel;

namespace TreatyAutomateSystem.Models;

public enum FacultativeType
{
    [Description("СПО")]
    Sec = default, // secondary
    [Description("ВО")]
    Hgh = 1 // higher
}