using KidPrograming.Repositories.Base;
using Microsoft.EntityFrameworkCore;

public class SeedData
{
    private readonly KidProgramingDbContext _context;

    public SeedData(KidProgramingDbContext context)
    {
        _context = context;
    }

    public async Task Initialise()
    {
        try
        {
            if (_context.Database.IsSqlServer())
            {
                bool dbExists = _context.Database.CanConnect();
                if (!dbExists)
                {
                    _context.Database.Migrate();
                }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            _context.Dispose();
        }
    }
}
