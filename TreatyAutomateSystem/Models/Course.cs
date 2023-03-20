namespace TreatyAutomateSystem.Models; 

public class Course 
{
    Course(){}

    public Course(int num)
    {
        Num = num;
    }

    public string Id { get; set; } = null!;

    public int Num { get; set; }
}