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
    public class GetArtistsControllerTests
    {
        private static DbContextOptions<AppDbContext> CreateNewContextOptions(string dbName) =>
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        [TestMethod]
        public async Task GetArtists_ReturnsAllArtists()
        {
            var options = CreateNewContextOptions("GetArtists_All");

            using (var context = new AppDbContext(options))
            {
                context.Artists.Add(new Artist { ArtistId = 1, Name = "A1" });
                context.Artists.Add(new Artist { ArtistId = 2, Name = "A2" });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var controller = new GetArtistsController(context);
                var result = await controller.GetArtists();

                Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
                var ok = result.Result as OkObjectResult;
                var list = ok.Value as List<Artist>;
                Assert.IsNotNull(list);
                Assert.AreEqual(2, list.Count);
                Assert.IsTrue(list.Any(a => a.Name == "A1"));
                Assert.IsTrue(list.Any(a => a.Name == "A2"));
            }
        }

        [TestMethod]
        public async Task GetArtistById_Existing_ReturnsArtist()
        {
            var options = CreateNewContextOptions("GetArtists_ById_Exists");

            using (var context = new AppDbContext(options))
            {
                context.Artists.Add(new Artist { ArtistId = 10, Name = "Existing" });
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var controller = new GetArtistsController(context);
                var result = await controller.GetArtistById(10);

                Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
                var ok = result.Result as OkObjectResult;
                var artist = ok.Value as Artist;
                Assert.IsNotNull(artist);
                Assert.AreEqual(10, artist.ArtistId);
                Assert.AreEqual("Existing", artist.Name);
            }
        }

        [TestMethod]
        public async Task GetArtistById_NotFound_ReturnsNotFound()
        {
            var options = CreateNewContextOptions("GetArtists_ById_NotFound");

            using (var context = new AppDbContext(options))
            {
                // no seed
                context.SaveChanges();
            }

            using (var context = new AppDbContext(options))
            {
                var controller = new GetArtistsController(context);
                var result = await controller.GetArtistById(999);

                Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
            }
        }
    }
}
