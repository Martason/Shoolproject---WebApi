namespace GeoComment.Services.Data
{
    public class DatabaseHandler
    {
        private readonly GeoCommetDBContext _context;

        public DatabaseHandler(GeoCommetDBContext context)
        {
            _context = context;
        }
        public async Task Recreate()
        {
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
        }
        public async Task CreateIfNotExist()
        {
            await _context.Database.EnsureCreatedAsync();
        }

    }
}
