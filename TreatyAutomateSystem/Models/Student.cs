using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.ValueGeneration;

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
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = null!;

    public string Fio { get; set; } = null!;

    public StudyConditionType StdCond { get; set; } 

    public Group Group { get; set; } = null!; 
}
