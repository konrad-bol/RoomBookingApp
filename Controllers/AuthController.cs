using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoombookingApp.Data;
using RoombookingApp.Middleware;
using RoombookingApp.Models;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace RoombookingApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        { _context = context; }

        [RequireLoggedOut]
        public IActionResult Register()
        {
            return View();
        }
        [RequireLoggedOut]
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if(await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                Console.WriteLine("jest taki");
                return View(); 
            }

            var (hash, salt) = HashPassword(user.PasswordHash);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            SetSession(user);
            return RedirectToAction("Index", "Home");
        }
        [RequireLoggedOut]
        public IActionResult Login()
        { return View(); }

        [HttpPost]
        [RequireLoggedOut]
        public IActionResult Login(string email,string password)
        {

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user!=null)
            {
                if (VerifyPassword(password, user.PasswordSalt, user.PasswordHash))
                {
                    SetSession(user);
                    return RedirectToAction("Index", "Home");
                }

            }
            ViewBag.Error = "niepoprawne dane";
            return View();
        }
        [RequireLoggedIn]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private void SetSession(User user)
        {
            HttpContext.Session.SetInt32("user_id", user.Id);
            HttpContext.Session.SetString("user_name", user.Name);
        }
        private static (string hash, string salt) HashPassword(string password)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(128 / 8);
            string salt = Convert.ToBase64String(saltBytes);

            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return (hash, salt);
        }
        public static bool VerifyPassword(string password, string salt, string expectedHash)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: saltBytes,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hash == expectedHash;
        }

    }
}
