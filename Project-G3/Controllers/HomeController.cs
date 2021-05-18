﻿using Project_G3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_G3.Controllers
{
    public class HomeController : Controller
    {        
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            //ApplicationDbContext _db = new ApplicationDbContext();
            //ViewBag.Movies = _db.Movies; //= _db.Movies; //.Select(m => m.MovieTitle.ToLower().Contains("g")).ToArray();
            
           // _db.SaveChanges();

            return View(db.Movies.ToArray());
        }
        public ActionResult Cart()
        {
            List<Movie> CartList = HttpContext.Session["ShoppingCart"] != null ? (List<Movie>)HttpContext.Session["ShoppingCart"] : new List<Movie>();

            return View(CartList);
        }
        
        public ActionResult AddToCart(int Id)
        {
            //ApplicationDbContext _db = new ApplicationDbContext();
            List<Movie> CartList = HttpContext.Session["ShoppingCart"]!=null ? (List<Movie>) HttpContext.Session["ShoppingCart"] : new List<Movie>();
            Movie movie = db.Movies.First(m => m.MovieId == Id);
            if (!CartList.Any(m => m.MovieId == movie.MovieId)) CartList.Add(movie);
            HttpContext.Session["ShoppingCart"] = CartList;

            return RedirectToAction("Index");
        }
        public ActionResult DeleteFromCart(int Id)
        {
            List<Movie> CartList = HttpContext.Session["ShoppingCart"] != null ? (List<Movie>)HttpContext.Session["ShoppingCart"] : new List<Movie>();
            if (CartList.Any(m => m.MovieId == Id)) CartList.Remove(CartList.First(m => m.MovieId == Id));
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
            //ApplicationDbContext db = new ApplicationDbContext();
            Movie movie = db.Movies.FirstOrDefault(m => m.MovieId == id);
            return View(movie);
        }  
        public ActionResult Genre()
        {
            return View(db.Genres.ToList());
        }
    }
}