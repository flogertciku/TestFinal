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
         // Pjesa me request

        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Register");
        }
        int id = (int)HttpContext.Session.GetInt32("userId");

       

        //Marrim gjithe perdoruesit e tjere
        List<Request> rq =  _context.Requests.Include(e=>e.Reciver).Include(e=>e.Sender).Where(e => e.ReciverId == id).Where(e => e.Accepted == false).ToList();

        ViewBag.perdoruesit2 = _context.Users.Include(e=>e.Requests).Where(e=> e.UserId != id).Where(e=>(e.Requests.Any(f=> f.SenderId == id) == false) && (e.Requests.Any(f=> f.ReciverId == id) == false) ).ToList();
        
        
        //List me request
        List<User> LIST4= _context.Users.Include(e=>e.Requests).Where(e=> e.UserId != id).Where(e=>(e.Requests.Any(f=> f.SenderId == id) == false) && (e.Requests.Any(f=> f.ReciverId == id) == false) ).ToList();
        //list me miqte
        List<Request> miqte =_context.Requests.Where(e => (e.SenderId == id) || (e.ReciverId == id)).Include(e=>e.Reciver).Include(e=>e.Sender).Where(e=>e.Accepted ==true).ToList();
        
        //Filtorjme listen e gjithe userave ne menyre qe miqte dhe ata qe u kemi nisur ose na kane nisur request te mos dalin tek users e tjere.
        for (int i = 0; i < LIST4.Count; i++)
        {
            var test22= LIST4[i].Requests.Except(rq);
            
            for (int j = 0; j < rq.Count; j++)
            {
                if (rq[j].SenderId == LIST4[i].UserId || rq[j].ReciverId == LIST4[i].UserId )
            {
                LIST4.Remove(LIST4[i]);
            }
            }
            for (int z = 0; z < miqte.Count; z++)
            {
                if (miqte[z].SenderId == LIST4[i].UserId || miqte[z].ReciverId == LIST4[i].UserId )
            {
                LIST4.Remove(LIST4[i]);
            }

            }

            
        }
        
        // lista e filtruar ruhet ne viewbag
        ViewBag.perdoruesit= LIST4;
        
        //shfaqim gjith requests
        ViewBag.requests = _context.Requests.Include(e=>e.Reciver).Include(e=>e.Sender).Where(e => e.ReciverId == id).Where(e => e.Accepted == false).ToList();

        // shfaq gjith miqte

        ViewBag.miqte = _context.Requests.Where(e => (e.SenderId == id) || (e.ReciverId == id)).Include(e=>e.Reciver).Include(e=>e.Sender).Where(e=>e.Accepted ==true).ToList();
        //Marr te loguarin me te dhena
        ViewBag.iLoguari = _context.Users.FirstOrDefault(e => e.UserId == id);


        //Mbaron ketu pjesa me request
        ViewBag.movies = _context.Movies.Include(e => e.Creator).Include(e => e.Fansat).ThenInclude(e => e.UseriQePelqen).OrderByDescending(e => e.CreatedAt).ToList();
       
       
        //Perfshi dhe return
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

    // Pjesa me request


    [HttpGet("SendR/{id}")]
    public IActionResult SendR(int id)
    {
        int idFromSession = (int)HttpContext.Session.GetInt32("userId");
        Request newRequest = new Models.Request()
        {
            SenderId = idFromSession,
            ReciverId = id,
          
        };
        _context.Requests.Add(newRequest);
        _context.SaveChanges();
        // User dbUser = _context.Users.Include(e=>e.Requests).First(e=> e.UserId == idFromSession);
        // dbUser.Requests.Add(newRequest);
        _context.SaveChanges();
        return RedirectToAction("index");

    }
    [HttpGet("AcceptR/{id}")]
    public IActionResult AcceptR(int id)
    {
        
        Request requestii = _context.Requests.First(e => e.RequestId == id);
        requestii.Accepted=true;
        // _context.Remove(hiqFans);
        _context.SaveChanges();
        return RedirectToAction("index");
    }
     [HttpGet("DeclineR/{id}")]
    public IActionResult Decline(int id)
    {
        
        Request requestii = _context.Requests.First(e => e.RequestId == id);
         _context.Remove(requestii);
        _context.SaveChanges();
        return RedirectToAction("index");
    }
    [HttpGet("RemoveF/{id}")]
    public IActionResult RemoveF(int id)
    {
        
        Request requestii = _context.Requests.First(e => e.RequestId == id);
         _context.Remove(requestii);
        _context.SaveChanges();
        return RedirectToAction("index");
    }

    //Perfundon Request










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
