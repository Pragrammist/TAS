namespace TreatyAutomateSystem.Models; 

public class Course 
{
    Course(){}

    public Course(string num)
    {
        Num = num;
    }

    public int Id { get; set; }

    public string Num { get; set; } = null!;
}