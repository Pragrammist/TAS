namespace TreatyAutomateSystem.Models; 

public class Group
{
    private Group(){}
    public Group(Speciality speciality, string name, string courseNum,FacultativeType facultative)
    {
        Speciality = speciality;
        Facultative = facultative;
        Name = name;
        CourseNum = courseNum;
    }
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public Speciality Speciality { get; set; } = null!;

    public string CourseNum { get; set; } = null!;

    public FacultativeType Facultative { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
}