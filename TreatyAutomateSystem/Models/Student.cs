namespace TreatyAutomateSystem.Models;

public class Student 
{
    private Student(){}
    public Student(string fio, Group group, StudyConditionType stdCond)
    {
        Fio = fio;
        Group = group;
        
        StdCond = stdCond;
    }
    public int Id { get; set; }

    public string Fio { get; set; } = null!;

    public StudyConditionType StdCond { get; set; } 

    public Group Group { get; set; } = null!; 
}
