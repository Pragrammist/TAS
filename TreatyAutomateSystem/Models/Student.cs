namespace TreatyAutomateSystem.Models;

public class Student 
{
    private Student(){}
    public Student(string fio, Group group, FacultativeType facultative = default, StudyConditionType stdCond = default)
    {
        Fio = fio;
        Group = group;
        Facultative = facultative;
        StdCond = stdCond;
    }
    public string Id { get; set; } = null!;

    public string Fio { get; set; } = null!;

    public FacultativeType Facultative { get; set; } 

    public StudyConditionType StdCond { get; set; } 

    public Group Group { get; set; } = null!; 
}
