using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.MicrosoftExtensions;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;
using TemplateJwtProject.Models.DTOVanStatistieken;

namespace TemplateJwtProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatistiekenController : Controller
{
    private readonly AppDbContext _context;

    public StatistiekenController(AppDbContext context)
    {
        _context = context;
    }
    
    /* API Calls
     * alle nummers die zijn gezakt:
       /api/Statistieken/Dalers?year=(jaar)
       
     * alle nummers die zijn gestegen:
       /api/Statistieken/Stijgers?year=(jaar)
       
     * alle liedjes die in alle edities van de TOP2000 in de lijst hebben gestaan:
       /api/Statistieken/GetEntriesOfAllTheYears
       
     * alle liedjes die in een op te geven jaar nieuw zijn binnengekomen in de lijst (die er
       dus het jaar ervoor niet in stonden):
       /api/Statistieken/GetNewComers?year=(jaar)
       
     * alle liedjes die in een op te geven jaar verdwenen zijn uit de lijst:
       /api/Statistieken/GetDisappearedSongs?year=(jaar)
       
     * alle liedjes die in een op te geven jaar opnieuw zijn binnengekomen in de lijst.
       /api/Statistieken/GetReturnedSongs?year=(jaar)
       
     * alle liedjes die in een op te geven jaar op dezelfde plek zijn blijven staan:
       /api/Statistieken/GetSongsInTheSamePosition?year=(jaar)
       
     * alle liedjes van artiesten die op 2 of meer aansluitende posities in de lijst van een
       op te geven jaar staan:
       /api/Statistieken/GetSongsWithConsecutivePositions?year=(year)
       
     * alle nummers die slechts één keer in de TOP2000 hebben gestaan:
       /api/Statistieken/GetSongsOnlyOnceOnTop2000
       
     * de artiesten met de meeste nummers in een gegeven jaar:
       /api/Statistieken/GetArtistWithMostSongsOnYear?year=(jaar)&amount=(aantal)
       -- amount is standaard 3
     */
    
    
    //1
    [HttpGet("Dalers")]
    public async Task<ActionResult<List<DalersDTO>>> Dalers(int year)
    {
        try
        {
            if (year > 1999 && year < 2025)
            {
                List<DalersDTO> allDalers = new List<DalersDTO>();
                
                List<Top2000Entry> entriesTop2000 = await _context.Top2000Entry
                    .Where(e => e.Year == year)
                    .Include(e => e.Songs)
                    .ThenInclude(s => s.Artist)
                    .ToListAsync();
            
                List<Top2000Entry> entriesTop2000YearBefore = await _context.Top2000Entry
                    .Where(e => e.Year == year - 1).ToListAsync();

                foreach (Top2000Entry entry in entriesTop2000)
                {
                    Top2000Entry? entryYearBefore = entriesTop2000YearBefore.FirstOrDefault(e => e.SongId == entry.SongId);
                    if (entryYearBefore != null)
                    {
                        if (entryYearBefore.Position < entry.Position)
                        {
                            int gedaald = entry.Position - entryYearBefore.Position;
                            Console.WriteLine($"{entry.Songs.Titel} is Gedaald met {gedaald}: Vorig jaar was {entryYearBefore.Position} en dit jaar is {entry.Position}");
                            
                            DalersDTO daler = new DalersDTO()
                            {
                                SongId = entry.SongId,
                                Title = entry.Songs.Titel,
                                ArtistName = entry.Songs.Artist.Name,
                                ReleaseYear = entry.Songs.ReleaseYear,
                                Position = entry.Position,
                                PositionYearBefore = entryYearBefore.Position,
                                Gedaald = gedaald
                            };
                            allDalers.Add(daler);
                        } else Console.WriteLine($"{entry.Songs.Titel} Zelfde gebleven of gestegen");
                    } else Console.WriteLine($"{entry.Songs.Titel} Heeft vorig jaar niet gespeeld");
                }
                List<DalersDTO> allDatersSorted = allDalers.OrderByDescending(d => d.Gedaald).ToList();
                return Ok(allDatersSorted);
            } else return BadRequest("Mag alleen tussen de 2000 en 2024");
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    //2
    [HttpGet("Stijgers")]
    public async Task<ActionResult<List<StijgersDTO>>> Stijgers(int year)
    {
        try
        {
            if (year > 1999 && year < 2025)
            {
                List<StijgersDTO> allStijgers = new List<StijgersDTO>();
                
                List<Top2000Entry> entriesTop2000 = await _context.Top2000Entry
                    .Where(e => e.Year == year)
                    .Include(e => e.Songs)
                    .ThenInclude(s => s.Artist)
                    .ToListAsync();
            
                List<Top2000Entry> entriesTop2000YearBefore = await _context.Top2000Entry
                    .Where(e => e.Year == year - 1).ToListAsync();

                foreach (Top2000Entry entry in entriesTop2000)
                {
                    Top2000Entry? entryYearBefore = entriesTop2000YearBefore.FirstOrDefault(e => e.SongId == entry.SongId);
                    if (entryYearBefore != null)
                    {
                        if (entry.Position < entryYearBefore.Position)
                        {
                            int gestegen = entryYearBefore.Position - entry.Position;
                            Console.WriteLine($"{entry.Songs.Titel} is Gedaald met {gestegen}: Vorig jaar was {entryYearBefore.Position} en dit jaar is {entry.Position}");
                            
                            StijgersDTO stijger = new StijgersDTO()
                            {
                                SongId = entry.SongId,
                                Title = entry.Songs.Titel,
                                ArtistName = entry.Songs.Artist.Name,
                                ReleaseYear = entry.Songs.ReleaseYear,
                                Position = entry.Position,
                                PositionYearBefore = entryYearBefore.Position,
                                Gestegen = gestegen
                            };
                            allStijgers.Add(stijger);
                        } else Console.WriteLine($"{entry.Songs.Titel} Zelfde gebleven of gedaald");
                    } else Console.WriteLine($"{entry.Songs.Titel} Heeft vorig jaar niet gespeeld");
                }
                List<StijgersDTO> allStijgersSorted = allStijgers.OrderByDescending(d => d.Gestegen).ToList();
                return Ok(allStijgersSorted);
            } else return BadRequest("Mag alleen tussen de 2000 en 2024");
        }
        catch(Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    //3
    [HttpGet("GetEntriesOfAllTheYears")]
    public async Task<ActionResult<List<SongOnAllYearDTO>>> GetEntriesOfAllTheYears()
    {
        try
        {
            List<SongOnAllYearDTO> allSongsOnAllYears = new List<SongOnAllYearDTO>();
            List<int> songIds = await _context.Top2000Entry
                .GroupBy(e => e.SongId)
                .Where(g => g.Count() == 26)
                .Select(g => g.Key)
                .ToListAsync();
        
            List<Songs> songs = await _context.Songs
                .Where(s => songIds.Contains(s.SongId))
                .Include(s => s.Artist)
                .OrderBy(s => s.Titel)
                .ToListAsync();

            foreach (Songs song in songs)
            {
                SongOnAllYearDTO songOnAllYears = new SongOnAllYearDTO()
                {
                    SongID = song.SongId,
                    Title = song.Titel,
                    ArtistName = song.Artist.Name,
                    ReleaseYear = song.ReleaseYear,
                };
                allSongsOnAllYears.Add(songOnAllYears);
            }
            return Ok(allSongsOnAllYears);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    //4
    [HttpGet("GetNewComers")]
    public async Task<ActionResult<List<NewComersDTO>>> GetNewComers(int year)
    {
        try
        {
            if (year > 1999 && year < 2025)
            {
                List<NewComersDTO> allNewComers = new List<NewComersDTO>();
                List<Top2000Entry> entriesTop2000 = await _context.Top2000Entry
                    .Where(e => e.Year == year)
                    .Include(e => e.Songs)
                    .ThenInclude(s => s.Artist)
                    .OrderBy(e => e.Position )
                    .ToListAsync();
                List<Top2000Entry> entriesTop2000YearBefore = await _context.Top2000Entry
                    .Where(e => e.Year == year - 1).ToListAsync();

                foreach (Top2000Entry entry in entriesTop2000)
                {
                    Top2000Entry? entryYearBefore = entriesTop2000YearBefore.FirstOrDefault(e => e.SongId == entry.SongId);
                    if (entryYearBefore == null)
                    {
                        NewComersDTO newComer = new NewComersDTO()
                        {
                            SongId = entry.SongId,
                            Position = entry.Position,
                            ArtistName = entry.Songs.Artist.Name,
                            Title = entry.Songs.Titel,
                            ReleaseYear = entry.Songs.ReleaseYear,
                        };
                        allNewComers.Add(newComer);
                        Console.WriteLine($"{entry.Songs.Titel} is Nieuw Binnen gekomen");
                    } else Console.WriteLine($"{entry.Songs.Titel} was ook vorige jaar op Top2000");
                }
                return Ok(allNewComers);
            } else return BadRequest("Mag alleen tussen de 2000 en 2024");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    //5
    [HttpGet("GetDisappearedSongs")]
    public async Task<ActionResult<List<DisappearedSongsDTO>>> GetDisappearedSongs(int year)
    {
        try
        {
            if (year > 1999 && year < 2025)
            {
                List<NewComersDTO> allDisappearedSongs = new List<NewComersDTO>();
                List<Top2000Entry> entriesTop2000YearBefore = await _context.Top2000Entry
                    .Where(e => e.Year == year - 1)
                    .Include(e => e.Songs)
                    .ThenInclude(s => s.Artist)
                    .OrderBy(e => e.Position )
                    .ToListAsync();
                List<Top2000Entry> entriesTop2000 = await _context.Top2000Entry
                    .Where(e => e.Year == year).ToListAsync();

                foreach (Top2000Entry entryYearBefore in entriesTop2000YearBefore)
                {
                    Top2000Entry? entryYear = entriesTop2000.FirstOrDefault(e => e.SongId == entryYearBefore.SongId);
                    if (entryYear == null)
                    {
                        NewComersDTO newComer = new NewComersDTO()
                        {
                            SongId = entryYearBefore.SongId,
                            Position = entryYearBefore.Position,
                            ArtistName = entryYearBefore.Songs.Artist.Name,
                            Title = entryYearBefore.Songs.Titel,
                            ReleaseYear = entryYearBefore.Songs.ReleaseYear,
                        };
                        allDisappearedSongs.Add(newComer);
                        Console.WriteLine($"{entryYearBefore.Songs.Titel} is Verdweneen");
                    } else Console.WriteLine($"{entryYearBefore.Songs.Titel} is ook dit jaar op Top2000");
                }
                return Ok(allDisappearedSongs);
            } else return BadRequest("Mag alleen tussen de 2000 en 2024");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    //6
    [HttpGet("GetReturnedSongs")]
    public async Task<ActionResult<List<ReturnedSongsDTO>>> GetReturnedSongs(int year)
    {
        try
        {
            if (year > 2000 && year < 2025)
            {
                List<int> songIds = new List<int>();
                List<ReturnedSongsDTO> allReturnedSongs = new List<ReturnedSongsDTO>();
                
                List<Top2000Entry> entries = await _context.Top2000Entry
                    .Where(e => e.Year == year)
                    .Include(e => e.Songs)
                    .ThenInclude(s => s.Artist)
                    .OrderBy(e => e.Position)
                    .ToListAsync();
                List<Top2000Entry> entriesYearBefore = await _context.Top2000Entry
                    .Where(e => e.Year == year - 1)
                    .ToListAsync();

                foreach (Top2000Entry entry in entries)
                {
                    Top2000Entry? entryYearBefore = entriesYearBefore.FirstOrDefault(e => e.SongId == entry.SongId);
                    if (entryYearBefore == null)
                    {
                        songIds.Add(entry.SongId);
                    }
                }
                
                
                var groups = await _context.Top2000Entry
                    .Where(e => songIds.Contains(e.SongId))
                    .GroupBy(e => e.SongId)
                    .Select(g => new
                    {
                        SongId = g.Key,
                        Entries = g.Where(e => e.Year < year - 1).ToList()
                    })
                    .ToListAsync();

                foreach (var group in groups)
                {
                    Console.WriteLine($"{group.SongId} heeft {group.Entries.Count}x zonder dit jaar en ook niet vorige jaar in de top2000");
                    if (group.Entries.Count != 0)
                    {
                        Top2000Entry correctEntry = entries.Where(e => e.SongId == group.SongId).FirstOrDefault();
                        ReturnedSongsDTO returnedSongsDTO = new ReturnedSongsDTO()
                        {
                            SongId = correctEntry.SongId,
                            Position = correctEntry.Position,
                            ArtistName = correctEntry.Songs.Artist.Name,
                            Title = correctEntry.Songs.Titel,
                            ReleaseYear = correctEntry.Songs.ReleaseYear
                        };
                        allReturnedSongs.Add(returnedSongsDTO);
                    }
                }

                List<ReturnedSongsDTO> allReturnedSongsSorted = allReturnedSongs.OrderBy(e => e.Position).ToList();
                return Ok(allReturnedSongsSorted);
            } else return BadRequest("Mag alleen tussen de 2001 en 2024");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    //7
    [HttpGet("GetSongsInTheSamePosition")]
    public async Task<ActionResult<List<SongsInTheSamePositionDto>>> GetSongsInTheSamePosition(int year)
    {
        try
        {
            if (year > 1999 && year < 2025)
            {
                List<SongsInTheSamePositionDto> allSongsInTheSamePosition = new List<SongsInTheSamePositionDto>();
                
                List<Top2000Entry> entries = await _context.Top2000Entry
                    .Where(e => e.Year == year)
                    .Include(e => e.Songs)
                    .ThenInclude(s => s.Artist)
                    .OrderBy(e => e.Position)
                    .ToListAsync();
                List<Top2000Entry> entriesYearBefore = await _context.Top2000Entry
                    .Where(e => e.Year == year - 1)
                    .ToListAsync();

                foreach (Top2000Entry entry in entries)
                {
                    Top2000Entry entryOnSamePosition = entriesYearBefore.FirstOrDefault(e => e.SongId == entry.SongId && e.Position == entry.Position);
                    if (entryOnSamePosition != null)
                    {
                        Console.WriteLine($"{entry.SongId} is op de zelfde positie als vorige jaar");
                        SongsInTheSamePositionDto SongsInTheSamePosition = new SongsInTheSamePositionDto()
                        {
                            SongId = entry.SongId,
                            Position = entry.Position,
                            ArtistName = entry.Songs.Artist.Name,
                            Title = entry.Songs.Titel,
                            ReleaseYear = entry.Songs.ReleaseYear
                        };
                        allSongsInTheSamePosition.Add(SongsInTheSamePosition);
                    }
                }
                return Ok(allSongsInTheSamePosition);
            } else return BadRequest("Mag alleen tussen de 2000 en 2024");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    //8
    [HttpGet("GetSongsWithConsecutivePositions")]
    public async Task<ActionResult<List<SongsWithConsecutivePositionsDto>>> GetSongsWithConsecutivePositions(int year)
    {
        try
        {
            if (year >= 1999 && year < 2025)
            {
                List<SongsWithConsecutivePositionsDto> allSongsWithConsecutivePositions = new List<SongsWithConsecutivePositionsDto>();
                
                List<Top2000Entry> entries = await _context.Top2000Entry
                    .Where(e => e.Year == year)
                    .Include(e => e.Songs)
                    .ThenInclude(s => s.Artist)
                    .OrderBy(e => e.Position)
                    .ToListAsync();
                int lastArtistId = 0;
                for (int i = 1; i <= entries.Count; i++)
                {
                    if (i < entries.Count - 1)
                    {
                        bool isSameArtist = entries[i].Songs.Artist.ArtistId == entries[i + 1].Songs.ArtistId;
                        if (isSameArtist)
                        {
                            Console.WriteLine($"{entries[i].SongId} is op de zelfde artist {entries[i].Songs.Artist.ArtistId} als {entries[i + 1].SongId}");
                            if (lastArtistId != entries[i].Songs.ArtistId)
                            {
                                SongsWithConsecutivePositionsDto songsWithConsecutivePositions = new SongsWithConsecutivePositionsDto()
                                {
                                    SongId = entries[i].SongId,
                                    Position = entries[i].Position,
                                    ArtistName = entries[i].Songs.Artist.Name,
                                    Title = entries[i].Songs.Titel,
                                    ReleaseYear = entries[i].Songs.ReleaseYear
                                };
                                
                                SongsWithConsecutivePositionsDto songsWithConsecutivePositionsNext = new SongsWithConsecutivePositionsDto()
                                {
                                    SongId = entries[i + 1].SongId,
                                    Position = entries[i + 1].Position,
                                    ArtistName = entries[i + 1].Songs.Artist.Name,
                                    Title = entries[i + 1].Songs.Titel,
                                    ReleaseYear = entries[i + 1].Songs.ReleaseYear
                                };
                                allSongsWithConsecutivePositions.Add(songsWithConsecutivePositions);
                                allSongsWithConsecutivePositions.Add(songsWithConsecutivePositionsNext);
                            }
                            else
                            {
                                SongsWithConsecutivePositionsDto songsWithConsecutivePositionsNext = new SongsWithConsecutivePositionsDto()
                                {
                                    SongId = entries[i + 1].SongId,
                                    Position = entries[i + 1].Position,
                                    ArtistName = entries[i + 1].Songs.Artist.Name,
                                    Title = entries[i + 1].Songs.Titel,
                                    ReleaseYear = entries[i + 1].Songs.ReleaseYear
                                };
                                allSongsWithConsecutivePositions.Add(songsWithConsecutivePositionsNext);
                            }
                        }
                        lastArtistId = entries[i].Songs.ArtistId;
                    }
                }
                return Ok(allSongsWithConsecutivePositions);
            } else return BadRequest("Mag alleen tussen de 1999 en 2024");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    //9
    [HttpGet("GetSongsOnlyOnceOnTop2000")]
    public async Task<ActionResult<List<SongsOnlyOnceOnTop2000Dto>>> GetSongsOnlyOnceOnTop2000()
    {
        try
        {
            List<SongsOnlyOnceOnTop2000Dto> allSongsOnlyOnceOnTop2000 = new List<SongsOnlyOnceOnTop2000Dto>();
            
            List<Top2000Entry> allEntries = await _context.Top2000Entry
                .Include(e => e.Songs)
                .ThenInclude(s => s.Artist)
                .OrderBy(e => e.Position)
                .ToListAsync();
            
            var groups = await _context.Top2000Entry
                .GroupBy(e => e.SongId)
                .Select(g => new
                {
                    SongId = g.Key,
                    Count = g.Count()
                })
                .Take(3000)
                .ToListAsync();

            foreach (var group in groups)
            {
                Console.WriteLine($"{group.SongId} heeft {group.Count}x");
                if (group.Count == 1)
                {
                    Top2000Entry entry = allEntries.Where(e => e.SongId == group.SongId).FirstOrDefault();
                    SongsOnlyOnceOnTop2000Dto SongsOnlyOnceOnTop2000 = new SongsOnlyOnceOnTop2000Dto()
                    {
                        SongId = entry.SongId,
                        Position = entry.Position,
                        ArtistName = entry.Songs.Artist.Name,
                        Title = entry.Songs.Titel,
                        ReleaseYear = entry.Songs.ReleaseYear,
                        Top2000Year = entry.Year
                    };
                    allSongsOnlyOnceOnTop2000.Add(SongsOnlyOnceOnTop2000);
                }
            }
            return Ok(allSongsOnlyOnceOnTop2000);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    //10
    [HttpGet("GetArtistWithMostSongsOnYear")]
    public async Task<ActionResult<List<ArtistWithMostSongsOnYearDto>>> GetArtistWithMostSongsOnYear(int year, int amount = 3)
    {
        try
        {
            if (year > 1998 && year < 2025)
            {
                List<ArtistWithMostSongsOnYearDto> listArtistWithMostSongsOnYear = new List<ArtistWithMostSongsOnYearDto>();
                List<Top2000Entry> entriesTop2000 = await _context.Top2000Entry
                    .Where(e => e.Year == year)
                    .Include(e => e.Songs)
                    .ThenInclude(s => s.Artist)
                    .ToListAsync();
                List<Artist> artists = entriesTop2000.Select(e => e.Songs.Artist).Distinct().ToList();

                foreach (Artist artist in artists)
                {
                    int totalSongs = 0;
                    int averagePosition = 0;
                    int highestPosition = 2000;
                    bool artistExist = false;
                    foreach (Top2000Entry entry in entriesTop2000)
                    {
                        if (entry.Songs.ArtistId == artist.ArtistId)
                        {
                            totalSongs++;
                            averagePosition += entry.Position;
                            if (highestPosition > entry.Position) highestPosition = entry.Position;
                            artistExist = true;
                        }
                    }

                    if (artistExist)
                    {
                        averagePosition /= totalSongs;
                        Console.WriteLine($"ArtistId: {artist.ArtistId}. Songs: {totalSongs}. Gemiddelde: {averagePosition}, Highest: {highestPosition}");
                        ArtistWithMostSongsOnYearDto artistWithMostSongsOnYear = new ArtistWithMostSongsOnYearDto()
                        {
                            ArtistId = artist.ArtistId,
                            ArtistName = artist.Name,
                            TotalSongs = totalSongs,
                            Average = averagePosition,
                            Highest = highestPosition
                        };
                        listArtistWithMostSongsOnYear.Add(artistWithMostSongsOnYear);
                    }
                }
                List<ArtistWithMostSongsOnYearDto> ordered = listArtistWithMostSongsOnYear
                    .OrderByDescending(a => a.TotalSongs)
                    .ToList();
                ArtistWithMostSongsOnYearDto lastArtistWithMostSongsOnYear = ordered[amount - 1];
                while (lastArtistWithMostSongsOnYear.TotalSongs == ordered[amount].TotalSongs)
                {
                    amount++;
                }
                List<ArtistWithMostSongsOnYearDto> sorted = ordered.Take(amount).ToList();
                return Ok(sorted);
            } else return  BadRequest("Mag alleen tussen de 1999 en 2024");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest(e.Message);
        }
        
    }
}