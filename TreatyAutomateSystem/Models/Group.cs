namespace TreatyAutomateSystem.Models; 


public class UploadTreatyModel
{
    public TreateType TreateType { get; set; }

    public IFormFile TreatyTemplate { get; set; } = null!;
}

public class Group
{
    private Group(){}
    public Group(Speciality speciality, string name, 
        string? courseNum = null, FacultativeType? facultative = null,
        DateTime? prStart = null, DateTime? prEnd = null,
        PracticeType? practiceType = null)
    {
        Speciality = speciality;
        Facultative = facultative;
        Name = name;
        CourseNum = courseNum;
        PracticeEnd = prEnd;
        PracticeStart = prStart;
        PracticeType = practiceType;
    }
    public int Id { get; set; }
    

    public DateTime? PracticeStart { get; set; }

    public DateTime? PracticeEnd { get; set; }


    public PracticeType? PracticeType { get; set; }


    public string Name { get; set; } = null!;

    public Speciality Speciality { get; set; } = null!;

    public string? CourseNum { get; set; } 

    public FacultativeType? Facultative { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
}