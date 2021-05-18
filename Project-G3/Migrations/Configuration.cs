namespace Project_G3.Migrations
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Project_G3.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            decimal[] priceList = { 25M, 50M, 99.9M, 150M, 200M };
            Random r = new Random(1);
            string jsonString = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "..\\SeedData100.min.json"));
            List<SeedModel> movieList = JsonConvert.DeserializeObject<List<SeedModel>>(jsonString);
            List<Star> starList = context.Stars.ToList();
            List<Genre> genreList = context.Genres.ToList();
            List<Director> directorList = context.Directors.ToList();
            foreach (SeedModel mo in movieList)
            {
                List<Star> stars = new List<Star>();
                List<Genre> genres = new List<Genre>();
                foreach (string star in mo.stars)
                {
                    Star starSelected = starList.Find(s => s.StarName.Equals(star));
                    if (starSelected == null)
                    {
                        starList.Add(starSelected = new Star { StarName = star });
                        stars.Add(starSelected);
                    }
                }
                foreach (string genre in mo.genres)
                {
                    Genre genreSelected = genreList.Find(g => g.GenreName.Equals(genre));
                    if (genreSelected == null)
                    {
                        genreList.Add(genreSelected = new Genre { GenreName = genre });
                        genres.Add(genreSelected);
                    }
                }
                Director directorSelected = directorList.Find(d => d.DirectorName.Equals(mo.director));
                if (directorSelected == null) directorList.Add(directorSelected = new Director { DirectorName = mo.director });
                Movie movieObj = new Movie
                {
                    MovieTitle = mo.tittle,
                    MovieDuration = mo.duration,
                    MovieDescription = mo.text,
                    MoviePosters = mo.cover,
                    MovieReleaseYear = mo.releasYear,
                    MoviePrice = priceList[r.Next(0, 5)],
                    Director = directorSelected,
                    Stars = stars,
                    Genres = genres
                };
                if (!context.Movies.Any(m => m.MovieTitle.Equals(mo.tittle)))
                    context.Movies.Add(movieObj);
            }
            context.SaveChanges();
        }
    }
}
