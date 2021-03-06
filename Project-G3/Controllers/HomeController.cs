using Newtonsoft.Json;
using Project_G3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Project_G3.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public HomeController()
        {
            ViewData["Genres"] = db.Genres.ToList();
            //Hämta data från URL/API
            var webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            var json = webClient.DownloadString(@"https://restcountries.com/v3.1/all");
            Contries[] contry = JsonConvert.DeserializeObject<Contries[]>(json);
            ViewData["Contries"] = contry.Select(c => c.Name).OrderBy(c => c.common);
        }

        //[Authorize (Roles="Admin")]
        public ActionResult Index()
        {
            List<MovieDisplayViewModel> CartList = HttpContext.Session["ShoppingCart"] != null ? (List<MovieDisplayViewModel>)HttpContext.Session["ShoppingCart"] : new List<MovieDisplayViewModel>();
            //ViewBag.Amount = GetCartAmount();
            List<Movie> movies = db.Movies.ToList();
            List<MovieDisplayViewModel> dm = GetAllFlashSales();
            foreach (Movie movie in movies)
            {
                if (!dm.Any(m => m.Movie == movie))
                {
                    dm.Add(new MovieDisplayViewModel
                    {
                        Movie = movie,
                        IsOnSale = false
                    });
                }
            }
            dm.OrderBy(m => m.Movie.MovieId);
            ViewData["RandomMovie"] = dm;
            ViewBag.Amount = GetCartAmount();
            return View(dm);
        }
        public ActionResult Searching()
        {            
            return View();
        }
        [HttpPost]
        public ActionResult Searching(string Searchstring)
        {
            ViewBag.Amount = GetCartAmount();
            List<Movie> movies = db.Movies.ToList();
            List<MovieDisplayViewModel> dm = GetAllFlashSales();
            ViewData["RandomMovie"] = dm;
            foreach (Movie movie in movies)
            {
                if (!dm.Any(m => m.Movie == movie))
                {
                    dm.Add(new MovieDisplayViewModel
                    {
                        Movie = movie,
                        IsOnSale = false
                    });
                }
            }
            //Använd av LINQ query för att hämta data
            var mo = from m in dm
                     where m.Movie.MovieTitle.Contains(Searchstring)
                     select m;
            ViewBag.Message = Searchstring;
            if (!string.IsNullOrEmpty(Searchstring))
            {
                //Leta efter movie title och kolla om det finns någon liknade title som matcha det kunden skrev i sök fälten                
                return View(mo);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult Cart()
        {
            List<MovieDisplayViewModel> CartList = HttpContext.Session["ShoppingCart"] != null ? (List<MovieDisplayViewModel>)HttpContext.Session["ShoppingCart"] : new List<MovieDisplayViewModel>();
            ViewData["ShoppingCart"] = CartList;
            decimal TotalPrice = 0;
            foreach (var item in CartList)
            {
                if (item.IsOnSale == true)
                {
                    TotalPrice += decimal.Parse(item.NewPrice);
                }
                else
                {
                    TotalPrice += item.Movie.MoviePrice;
                }

            }
            ViewBag.Sum = TotalPrice;
            ViewBag.Amount = GetCartAmount();
            return View(CartList);
        }
        public ActionResult AddToCart(int Id)
        {
            //ApplicationDbContext _db = new ApplicationDbContext();
            List<MovieDisplayViewModel> CartList = HttpContext.Session["ShoppingCart"] != null ? (List<MovieDisplayViewModel>)HttpContext.Session["ShoppingCart"] : new List<MovieDisplayViewModel>();
            List<Movie> movies = db.Movies.ToList();
            List<MovieDisplayViewModel> dm = GetAllFlashSales();
            foreach (Movie movie in movies)
            {
                if (!dm.Any(m => m.Movie == movie))
                {
                    dm.Add(new MovieDisplayViewModel
                    {
                        Movie = movie,
                        IsOnSale = false
                    });
                }
            }
            if (!CartList.Any(m => m.Movie.MovieId == Id)) CartList.Add(dm.Find(m => m.Movie.MovieId == Id));
            HttpContext.Session["ShoppingCart"] = CartList;
            Session["Count"] = CartList.Count;
            return RedirectToAction("Index");
        }
        public ActionResult DeleteFromCart(int Id)
        {
            List<MovieDisplayViewModel> CartList = HttpContext.Session["ShoppingCart"] != null ? (List<MovieDisplayViewModel>)HttpContext.Session["ShoppingCart"] : new List<MovieDisplayViewModel>();
            if (CartList.Any(m => m.Movie.MovieId == Id)) CartList.Remove(CartList.First(m => m.Movie.MovieId == Id));
            return RedirectToAction("Cart");
        }
        public ActionResult About()
        {

            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Info(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Movie movie = db.Movies.FirstOrDefault(m => m.MovieId == id);
            //skapa en lista med all genre
            List<Genre> genres = db.Genres.ToList();
            List<Genre> title = new List<Genre>();
            //leta efter vilken genre och hur många genre som finns i den filmen och lägger in i tomt lista
            foreach (var genre in movie.Genres)
            {
                title.Add(genre);
            }
            //lägger in alla genreName i Viewdata
            ViewData["GenreInfo"] = title;
            ViewBag.Amount = GetCartAmount();
            return View(movie);
        }
        public ActionResult Genre(int Id)
        {
            //List<Movie> movies = db.Genres.First(g => g.GenreId == Id).Movies.ToList();
            List<Genre> genre = db.Genres.ToList();
            List<Movie> movies = db.Genres.First(g => g.GenreId == Id).Movies.ToList();
            List<MovieDisplayViewModel> dm = GetAllFlashSales();

            //var mo = dm.Any(m => m.Movie.Genres.Any(ge => ge.GenreId == Id));
            //if (mo == false)
            //{
            //    dm.RemoveAt(0);
            //}
            foreach (Movie movie in movies)
            {
                if (!dm.Any(m => m.Movie == movie))
                {
                    dm.Add(new MovieDisplayViewModel
                    {
                        Movie = movie,
                        IsOnSale = false
                    });
                }
            }
            dm.OrderBy(m => m.Movie.MovieId);
            ViewData["GenreTitle"] = genre.First(g => g.GenreId == Id).GenreName;
            ViewBag.Amount = GetCartAmount();
            return View(dm);
        }
        public ActionResult ConfirmAdress(int Id)
        {
            //ViewBag.Name = "Please select your payment!";
            //ViewBag.ContinueAsGuest = Id;
            FormDetails GetId = new FormDetails();
            GetId.UserType = Id;
            ViewBag.UserId = Id;
            ViewBag.Amount = GetCartAmount();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmAdress(FormDetails form, int id)
        {
            FormDetails CustomInfo = new FormDetails();
            CustomInfo.FirstName = form.FirstName;
            CustomInfo.LastName = form.LastName;
            CustomInfo.StreetAdress = form.StreetAdress;
            CustomInfo.Zipcode = form.Zipcode;
            CustomInfo.City = form.City;
            CustomInfo.Contry = form.Contry;
            CustomInfo.EmailAdress = form.EmailAdress;
            CustomInfo.PhoneNumber = form.PhoneNumber;
            CustomInfo.PaymentName = form.PaymentName;
            CustomInfo.UserType = id;
            if (!ModelState.IsValid)
            {
                return View(CustomInfo);
            }
            return RedirectToAction("PaymentMethod", "Payment", CustomInfo);
        }
        public ActionResult DeleteFromCartAfterReceipt()
        {
            List<MovieDisplayViewModel> CartList = HttpContext.Session["ShoppingCart"] != null ? (List<MovieDisplayViewModel>)HttpContext.Session["ShoppingCart"] : new List<MovieDisplayViewModel>();
            //if (CartList.Any(m => m.Movie.MovieId == Id)) CartList.Remove(CartList.First(m => m.Movie.MovieId == Id));
            if (CartList != null)
            {
                CartList.Clear();
            }
            return RedirectToAction("Index");
        }
        public List<MovieDisplayViewModel> GetAllFlashSales()
        {
            List<MovieDisplayViewModel> movies = new List<MovieDisplayViewModel>();
            List<FlashSale> flashsales = db.FlashSales.ToList();
            foreach (FlashSale item in flashsales)
            {
                decimal newPrice;
                bool isprocentedbased = item.FlashSaleDiscount[item.FlashSaleDiscount.Length - 1] == '%';
                foreach (Movie movie in item.Movies)
                {
                    decimal discount;
                    if (isprocentedbased)
                    {
                        discount = Decimal.Parse(item.FlashSaleDiscount.Substring(0, item.FlashSaleDiscount.Length - 1)) / 100;
                        newPrice = movie.MoviePrice - movie.MoviePrice * discount;
                    }
                    else
                    {
                        discount = decimal.Parse(item.FlashSaleDiscount);
                        newPrice = movie.MoviePrice - discount;
                    }
                    MovieDisplayViewModel Mo = movies.Find(m => m.Movie.MovieId == movie.MovieId);
                    if (Mo == null)
                    {
                        movies.Add(new MovieDisplayViewModel
                        {
                            Movie = movie,
                            IsOnSale = true,
                            NewPrice = newPrice.ToString("#.#0"),
                            FlashSale = item.FlashSaleDiscount
                        });
                    }
                    else if (Mo.Movie.MoviePrice > movie.MoviePrice)
                    {
                        movies.Remove(Mo);
                        movies.Add(new MovieDisplayViewModel
                        {
                            Movie = movie,
                            IsOnSale = true,
                            NewPrice = newPrice.ToString(),
                            FlashSale = item.FlashSaleDiscount
                        });
                    }
                }
            }
            return movies;
        }
        public ActionResult GetFlashSale()
        {
            ViewBag.Amount = GetCartAmount();
            return View(GetAllFlashSales());
        }
        public ActionResult News()
        {
            List<Movie> movies = db.Movies.ToList();
            List<MovieDisplayViewModel> dm = GetAllFlashSales();
            foreach (Movie movie in movies)
            {
                if (Int32.Parse(movie.MovieReleaseYear) > 2018)
                {
                    if (!dm.Any(m => m.Movie == movie))
                    {
                        dm.Add(new MovieDisplayViewModel { Movie = movie, IsOnSale = false });
                    }
                }
            }
            ViewBag.Amount = GetCartAmount();
            return View(dm);
        }
        //hämta antal varor i varokorgen
        public int GetCartAmount()
        {
            List<MovieDisplayViewModel> CartList = HttpContext.Session["ShoppingCart"] != null ? (List<MovieDisplayViewModel>)HttpContext.Session["ShoppingCart"] : new List<MovieDisplayViewModel>();
            int Count = CartList.Count;
            return Count;
        }
    }
}