namespace TreatyAutomateSystem.Models;

public class StudentDto
{
    public int Id { get; set; }

    public string Fio { get; set;} = null!;

    public string Group { get; set;} = null!;

    public int Course { get; set;} 

    public string Speciality { get; set;} = null!;

    public string ConditionType { get; set;} = null!;
}