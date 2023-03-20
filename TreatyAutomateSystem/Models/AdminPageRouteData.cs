using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;

namespace TreatyAutomateSystem.Models;

public class AdminPageRouteDataModel
{
    public AdminPageType PageType { get; set; }

    public string? Group { get; set; }
}