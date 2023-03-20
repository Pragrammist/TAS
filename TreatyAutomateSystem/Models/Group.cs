namespace TreatyAutomateSystem.Models; 

public class Group
{
    private Group(){}
    public Group(Speciality speciality, Course course, string name)
    {
        Speciality = speciality;
        Course = course;
        Name = name;
    }
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public Speciality Speciality { get; set; } = null!;

    public Course Course { get; set; } = null!;
}