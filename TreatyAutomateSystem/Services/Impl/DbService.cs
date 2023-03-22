using TreatyAutomateSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace TreatyAutomateSystem.Services;

public class DbService
{
    public class SupperContext : DbContext
    {
        public SupperContext()
        {
            
        }
    }
    
    public DbService()
    {
        
    }

    public void AddOrUpdateGroup(Group group)
    {

    }
    
    
}