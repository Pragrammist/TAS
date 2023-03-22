namespace TreatyAutomateSystem.Models; 

public class Group
{
    private Group(){}
    public Group(Speciality speciality, Course course, string name, FacultativeType facultative)
    {
        Speciality = speciality;
        Course = course;
        Facultative = facultative;
        Name = name;
    }
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public Speciality Speciality { get; set; } = null!;

    public Course Course { get; set; } = null!;

    public FacultativeType Facultative { get; set; }
}