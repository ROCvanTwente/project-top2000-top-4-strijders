using TemplateJwtProject.Services;

namespace TemplateJwtProject.Data.Seeders
{
    public class ArtistInfoSeeder
    {
        private readonly AppDbContext _context;
        private readonly WikipediaService _wiki;

        public ArtistInfoSeeder(AppDbContext context, WikipediaService wiki)
        {
            _context = context;
            _wiki = wiki;
        }

        public async Task SeedAsync()
        {
            var artists = _context.Artists
                .Where(a => string.IsNullOrEmpty(a.Biography)
                || string.IsNullOrEmpty(a.Wiki)
                || string.IsNullOrEmpty(a.Photo))
                .ToList();

            foreach (var artist in artists)
            {
                var data = await _wiki.GetArtistDataAsync(artist.Name);

                if (data != null)
                {
                    artist.Biography = data.Extract;
                    artist.Wiki = data.WikiUrl;
                    artist.Photo = data.PhotoUrl;

                    Console.WriteLine($"{artist.Name}");
                }
                else
                {
                    Console.WriteLine($"{artist.Name} (not found)");
                }

                await Task.Delay(200);
            }

            await _context.SaveChangesAsync();
        }
    }
    }
