namespace TreatyAutomateSystem.Models; 

public class Course 
{
    Course(){}

    public Course(int num)
    {
        Num = num;
    }

    public int Id { get; set; }

    public int Num { get; set; }
}