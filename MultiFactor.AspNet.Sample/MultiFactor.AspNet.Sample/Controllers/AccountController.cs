using JWT;
using JWT.Algorithms;
using JWT.Builder;
using MultiFactor.AspNet.Sample.Models;
using MultiFactor.AspNet.Sample.Services;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;

namespace MultiFactor.AspNet.Sample.Controllers
{
    public class AccountController : Controller
    {
        private MultiFactorSettings _settings = new MultiFactorSettings();
        
        public ActionResult Login()
        {
            return View(new LoginModel
            {
                Email = "user@example.com", //test credentials
            });
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                //check here login and password
                if (model.Email == "user@example.com" && model.Password == "123")
                {
                    return RedirectToMfa(model.Email);
                }

                ModelState.AddModelError(string.Empty, "Invalid credentials");
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult PostbackFromMfa(string accessToken)
        {
            //Postback from Multifactor

            try
            {
                var userParams = new JwtBuilder()
                    .WithSecret(_settings.ApiSecret)
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .MustVerifySignature()
                    .Decode<IDictionary<string, object>>(accessToken);

                var login = userParams["sub"] as string;

                FormsAuthentication.SetAuthCookie(login, false);
                return RedirectToAction("Index", "Home");
            }
            catch (TokenExpiredException)
            {
                //log: Token has expired
                return RedirectToAction("Login");
            }
            catch (SignatureVerificationException)
            {
                //log: Token has invalid signature
                return RedirectToAction("Login");
            }
        }

        private ActionResult RedirectToMfa(string login)
        {
            var postbackUrl = Url.Action("PostbackFromMfa", "Account", null, Request.Url.Scheme);

            var client = new MultiFactorWebClient(_settings);
            var url = client.CreateRequest(login, postbackUrl);

            return RedirectPermanent(url);
        }
    }
}