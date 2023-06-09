namespace TreatyAutomateSystem.Models;



public class Speciality
{
    private Speciality(){}
    public Speciality(string name, string code)
    {
        Code = code;
        Name = name;
    }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;
}
