using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TestFinal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TestFinal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;

    // here we can "inject" our context service into the constructor
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }
    public IActionResult Index()

    {

        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Register");
        }
        int id = (int)HttpContext.Session.GetInt32("userId");
        // ViewBag.zjarrte= _context.Movies.Include(e=>e.Fansat).ThenInclude(e=> e.Type).Where(e=> e.Fa).OrderBy(e => e.Fansat).Take(3).ToList(); 
        ViewBag.iLoguari = _context.Users.FirstOrDefault(e => e.UserId == id);
        ViewBag.movies = _context.Movies.Include(e => e.Creator).Include(e => e.Fansat).ThenInclude(e => e.UseriQePelqen).OrderByDescending(e => e.CreatedAt).ToList();

        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet("Register")]
    public IActionResult Register()
    {


        if (HttpContext.Session.GetInt32("userId") == null)
        {

            return View();
        }

        return RedirectToAction("Index");

    }
    [HttpPost("Register")]
    public IActionResult Register(User user)
    {
        // Check initial ModelState
        if (ModelState.IsValid)
        {
            // If a User exists with provided email
            if (_context.Users.Any(u => u.UserName == user.UserName))
            {
                // Manually add a ModelState error to the Email field, with provided
                // error message
                ModelState.AddModelError("UserName", "UserName already in use!");

                return View();
                // You may consider returning to the View at this point
            }
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            user.Password = Hasher.HashPassword(user, user.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("userId", user.UserId);
            return RedirectToAction("Index");
        }
        return View();
    }

    [HttpPost("Login")]
    public IActionResult LoginSubmit(LoginUser userSubmission)
    {
        if (ModelState.IsValid)
        {
            // If initial ModelState is valid, query for a user with provided email
            var userInDb = _context.Users.FirstOrDefault(u => u.UserName == userSubmission.UserName);
            // If no user exists with provided email
            if (userInDb == null)
            {
                // Add an error to ModelState and return to View!
                ModelState.AddModelError("User", "Invalid UserName/Password");
                return View("Register");
            }

            // Initialize hasher object
            var hasher = new PasswordHasher<LoginUser>();

            // verify provided password against hash stored in db
            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

            // result can be compared to 0 for failure
            if (result == 0)
            {
                ModelState.AddModelError("Password", "Invalid Password");
                return View("Register");
                // handle failure (this should be similar to how "existing email" is handled)
            }
            HttpContext.Session.SetInt32("userId", userInDb.UserId);

            return RedirectToAction("Index");
        }

        return View("Register");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {

        HttpContext.Session.Clear();
        return RedirectToAction("register");
    }



    // MoviePart

    [HttpGet("Movie/Add")]
    public IActionResult MovieAdd()
    {

        return View();
    }
    [HttpPost("/Movie/Add")]
    public IActionResult MessagesCreate(Movie marrNgaView)
    {
        if (ModelState.IsValid)
        {
            int id = (int)HttpContext.Session.GetInt32("userId");

            if (_context.Movies.Any(u => u.Tittle == marrNgaView.Tittle))
            {
                // Manually add a ModelState error to the Email field, with provided
                // error message
                ModelState.AddModelError("Tittle", "This movie already in use!");

                return View("MovieAdd");
                // You may consider returning to the View at this point
            }
            marrNgaView.UserId = id;
            _context.Movies.Add(marrNgaView);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View("MovieAdd");
    }

    [HttpGet("Movie/BehuFans/{id}")]
    public IActionResult FansAdd(int id)
    {
        ViewBag.id = id;
        return View();
    }

    [HttpPost("Movie/BehuFans/{id}")]
    public IActionResult BehuFans(int id, string type)
    {
        int idFromSession = (int)HttpContext.Session.GetInt32("userId");
        Fans fansIRI = new Fans()
        {
            UserId = idFromSession,
            MovieId = id,
            Type = type
        };
        _context.Fansat.Add(fansIRI);
        _context.SaveChanges();
        return RedirectToAction("index");

    }
    [HttpGet("Movie/HiqeFans/{id}")]
    public IActionResult FansRemove(int id)
    {
        int idFromSession = (int)HttpContext.Session.GetInt32("userId");
        Fans hiqFans = _context.Fansat.First(e => e.FansId == id);
        _context.Remove(hiqFans);
        _context.SaveChanges();
        return RedirectToAction("index");
    }
    [HttpGet("Movie/Edit/{id}")]
    public IActionResult MovieEdit(int id)
    {

        Movie editMovie = _context.Movies.First(e => e.MovieId == id);

        return View(editMovie);
    }
    [HttpPost("Movie/Edit/{id}")]
    public IActionResult MovieUpdate(int id, Movie marrNgaView)
    {

        Movie editMovie = _context.Movies.First(e => e.MovieId == id);
        if (ModelState.IsValid)
        {
         if (_context.Movies.Any(u => u.Tittle == marrNgaView.Tittle))
            {
                // Manually add a ModelState error to the Email field, with provided
                // error message
                ModelState.AddModelError("Tittle", "This movie already in use!");

                return View("MovieEdit",editMovie);
                // You may consider returning to the View at this point
            }
        editMovie.Tittle = marrNgaView.Tittle;
        editMovie.Description = marrNgaView.Description;
        _context.SaveChanges();

        return RedirectToAction("Index");}
        
        return View("MovieEdit",editMovie);
    }
    [HttpGet("Movie/{id}")]
    public IActionResult Movie(int id)
    {


        if (HttpContext.Session.GetInt32("userId") == null)
        {

            return RedirectToAction("Register");
        }
        ViewBag.Movie = _context.Movies.Include(e => e.Creator).Include(e => e.Fansat).ThenInclude(e=> e.UseriQePelqen).First(e => e.MovieId== id);

        return View();

    }

    [HttpGet("Movie/Delete/{id}")]
    public IActionResult MovieDelete(int id)
    {


        if (HttpContext.Session.GetInt32("userId") == null)
        {

            return RedirectToAction("Register");
        }
        Movie fshiMovie = _context.Movies.First(e => e.MovieId== id);
        _context.Movies.Remove(fshiMovie);
        _context.SaveChanges();

        return RedirectToAction("index");

    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
