using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TemplateJwtProject.Controllers;
using TemplateJwtProject.Data;
using TemplateJwtProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace TestProject1.TestFiles
{
    [TestClass]
    public class GetTop2000EntriesControllerTests
    {
        private static DbContextOptions<AppDbContext> CreateNewContextOptions(string dbName) =>
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        [TestMethod]
        public async Task GetYearEntries_ValidYear_ReturnsEntriesOrdered()
        {
            var options = CreateNewContextOptions("GetTop2000Entries_Valid");

            using (var context = new AppDbContext(options))
            {
                var artist = new Artist { ArtistId = 1, Name = "Artist1" };
                context.Artists.Add(artist);

                var song1 = new Songs { SongId = 10, Titel = "SongA", ReleaseYear = 2000, ArtistId = artist.ArtistId, Artist = artist };
                var song2 = new Songs { SongId = 20, Titel = "SongB", ReleaseYear = 2001, ArtistId = artist.ArtistId, Artist = artist };
                context.Songs.AddRange(song1, song2);

                context.Top2000Entry.Add(new Top2000Entry { SongId = song1.SongId, Year = 2005, Position = 2, Songs = song1 });
                context.Top2000Entry.Add(new Top2000Entry { SongId = song2.SongId, Year = 2005, Position = 1, Songs = song2 });

                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var controller = new GetTop2000EntriesController(context);
                var result = await controller.GetYearEntries(2005);

                Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
                var ok = result.Result as OkObjectResult;
                var list = ok.Value as List<Top2000Entry>;
                Assert.IsNotNull(list);
                Assert.AreEqual(2, list.Count);

                // Confirm ordering by Position (ascending)
                Assert.AreEqual(1, list[0].Position);
                Assert.AreEqual(2, list[1].Position);

                // Navigations should be populated because controller includes Songs and Artist
                Assert.IsNotNull(list[0].Songs);
                Assert.IsNotNull(list[0].Songs.Artist);
                Assert.AreEqual("Artist1", list[0].Songs.Artist.Name);
            }
        }

        [TestMethod]
        public async Task GetYearEntries_InvalidYear_ReturnsBadRequest()
        {
            var options = CreateNewContextOptions("GetTop2000Entries_Invalid");

            using (var context = new AppDbContext(options))
            {
                // no seed required
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var controller = new GetTop2000EntriesController(context);
                var result = await controller.GetYearEntries(1998);

                Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
                var bad = result.Result as BadRequestObjectResult;
                Assert.IsNotNull(bad.Value);
                Assert.AreEqual("jaar moet tussen de 1999 en 2024 zijn", bad.Value);
            }
        }
    }
}
