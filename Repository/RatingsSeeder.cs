using Ratings.Helpers;
using Ratings.Models;

namespace Ratings.Repository
{
    public class RatingsSeeder
    {
        public static async Task SeedDatabase2(RatingsRepository repository, ILogger<RatingsSeeder> logger)
        {
            if (repository.GetEntitiesCount<Artist>() == 0 && repository.GetEntitiesCount<Work>() == 0 && repository.GetEntitiesCount<Rating>() == 0)
            {
                logger.LogInformation("Baza danych pusta - zasilanie początkowymi danymi");

                Artist a1 = new Artist
                {
                    Surname = "Schubert",
                    FirstName = "Franz",
                    Bio = "Ur. 31 stycznia 1797 w Himmelpfortgrund, zm. 19 listopada 1828 w Wiedniu – austriacki kompozytor, prekursor romantyzmu w muzyce",
                    Photo = await AppHelper.GetImageBytes(new FileStream("wwwroot/img/sample_images/schubert.jpg", FileMode.Open))
                };

                Artist a2 = new Artist
                {
                    Surname = "Varèse",
                    FirstName = "Edgard",
                    Bio = "Amerykański kompozytor francuskiego pochodzenia. Przedstawiciel modernizmu",
                    Photo = await AppHelper.GetImageBytes(new FileStream("wwwroot/img/sample_images/varese.jpg", FileMode.Open))
                };

                Artist a3 = new Artist
                {
                    Surname = "Herzog",
                    FirstName = "Werner",
                    Bio = "Ur. 5 września 1942 w Monachium – niemiecki reżyser filmowy, teatralny i operowy; scenarzysta, producent filmowy, aktor, pisarz i poeta pochodzenia chorwackiego",
                    Photo = await AppHelper.GetImageBytes(new FileStream("wwwroot/img/sample_images/herzog.jpg", FileMode.Open))
                };

                Artist a4 = new Artist
                {
                    Surname = "Jarmusch",
                    FirstName = "Jim",
                    Bio = "Ur. 22 stycznia 1953 w Cuyahoga Falls – amerykański reżyser, scenarzysta, aktor i muzyk",
                    Photo = await AppHelper.GetImageBytes(new FileStream("wwwroot/img/sample_images/jarmusch.jpg", FileMode.Open))
                };

                await repository.AddWorks(
                    new Work
                    {
                        Name = "Piano Trio No. 2 in E flat major Op. 100 / D. 929",
                        Year = 1827,
                        Artist = a1,
                    },
                    new Work
                    {
                        Name = "Symphony No. 9 in C major D. 944",
                        Year = 1826,
                        Artist = a1,
                    },
                    new Work
                    {
                        Name = "String Quintet in C major Op. 163 / D. 956",
                        Year = 1828,
                        Artist = a1,
                    },
                    new Work
                    {
                        Name = "Ameriques",
                        Year = 1921,
                        Artist = a2,
                    },
                    new Work
                    {
                        Name = "Ionisation",
                        Year = 1931,
                        Artist = a2,
                    },
                    new Work
                    {
                        Name = "Aguirre, gniew boży",
                        Year = 1972,
                        Artist = a3,
                    },
                    new Work
                    {
                        Name = "Fitzcarraldo",
                        Year = 1982,
                        Artist = a3,
                    },
                    new Work
                    {
                        Name = "Mój najlepszy szatan",
                        Year = 1999,
                        Artist = a3,
                    },
                    new Work
                    {
                        Name = "Inaczej niż w raju",
                        Year = 1984,
                        Artist = a4,
                    },
                    new Work
                    {
                        Name = "Truposz",
                        Year = 1995,
                        Artist = a4,
                    },
                    new Work
                    {
                        Name = "Ghost Dog: Droga samuraja",
                        Year = 1999,
                        Artist = a4,
                    });

                logger.LogInformation("Zasilanie zakończone");
            }
        }
    }
}
