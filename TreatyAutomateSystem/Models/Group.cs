namespace TreatyAutomateSystem.Models; 

public class Group
{
    private Group(){}
    public Group(Speciality speciality, string name, 
        string? courseNum = null, FacultativeType? facultative = null,
        DateTime? prStart = null, DateTime? prEnd = null,
        string? practiceType = null)
    {
        Speciality = speciality;
        Facultative = facultative;
        Name = name;
        CourseNum = courseNum;
        PracticeEnd = prEnd;
        PracticeStart = prStart;
        PracticeType = PracticeType;
    }
    public int Id { get; set; }
    

    public DateTime? PracticeStart { get; set; }

    public DateTime? PracticeEnd { get; set; }


    public string? PracticeType { get; set; }


    public string Name { get; set; } = null!;

    public Speciality Speciality { get; set; } = null!;

    public string? CourseNum { get; set; } 

    public FacultativeType? Facultative { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
}