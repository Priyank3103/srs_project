using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using MVCNoteMarketPlace.Models;
using System.Web.Security;


namespace MVCNoteMarketPlace.Controllers
{
    public class AccountController : Controller
    {

        // GET: Account
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(User user)
        {
            bool Status = false;
            string Message = "";

            if (ModelState.IsValid)
            {
                if (user.Password == user.Confirmpassword)
                {
                    var IsExist = IsEmailExist(user.EmailID);

                    if (IsExist)
                    {
                        ModelState.AddModelError("EmailExist", "Email already exist");
                        return View(user);
                    }


                    user.ActivationCode = Guid.NewGuid();
                    //user.Password = Crypto.Hash(user.Password);
                    //user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                    user.IsActive = true;
                    user.IsEmailVerified = false;
                    user.RoleID = 1;
                    using (NoteMarketPlaceEntities entities = new NoteMarketPlaceEntities())
                    {
                        try
                        {

                            entities.Users.Add(user);

                            entities.SaveChanges();


                        }
                        catch (Exception e)
                        {

                            return View(e.Message);
                        }

                        SendverificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
                        Message = "Registration successfully done.check your email for verification";
                        Status = true;

                        ModelState.Clear();

                    }
                }


                else
                {
                    ViewBag.password = "password does not match";
                }

            }
            else
            {
                Message = "Invalid Request";
            }
            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View(user);
        }

        public ActionResult VerifyAccount(string ID)
        {
            bool Status = false;
            using (NoteMarketPlaceEntities dc = new NoteMarketPlaceEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;
                var v = dc.Users.Where(a => a.ActivationCode == new Guid(ID)).FirstOrDefault();
                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login login, string ReturnUrl = "")
        {
            string message = "";
            using (NoteMarketPlaceEntities entities = new NoteMarketPlaceEntities())
            {
                var v = entities.Users.Where(a => a.EmailID == login.Email).FirstOrDefault();
                if (v != null)
                {
                    if (string.Compare(login.Password, v.Password) == 0)
                    {
                        int timeout = login.Checkbox ? 525600 : 15;
                        var ticket = new FormsAuthenticationTicket(login.Email, login.Checkbox, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);


                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("DashBoard", "Home");
                        }
                    }
                    else
                    {
                        message = "Invalid credential provider";
                    }

                }
                else
                {
                    message = "Invalid credential provider";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        [HttpGet]
        public ActionResult ForgetPassword()
        {

            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(string Email)
        {

            string message = "";
            //bool status = false;
            using (NoteMarketPlaceEntities entities = new NoteMarketPlaceEntities())
            {

                var account = entities.Users.Where(a => a.EmailID == Email).FirstOrDefault();
                if (account != null)
                {
                    Random ran = new Random();
                    string resetCode = Convert.ToString(ran.Next());
                    //string resetCode = Guid.NewGuid().ToString();
                    SendverificationLinkEmail(account.EmailID, resetCode, "ResetPassword");
                    account.ResetPassword = resetCode;
                    //var temp_pass = Convert.ToString(resetCode);
                    account.Password = resetCode;
                    entities.Configuration.ValidateOnSaveEnabled = false;
                    entities.SaveChanges();
                    message = "Reset password link has been sent to your email id";

                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        [Authorize]
        public ActionResult ResetPassword()
        {
            return View();

        }

        [HttpPost]

        public ActionResult ResetPassword(ResetPassword model)
        {
            string userId = User.Identity.Name;
            using (NoteMarketPlaceEntities entities = new NoteMarketPlaceEntities())
            {
                var v = entities.Users.Where(a => a.EmailID == userId).FirstOrDefault();
                string OldPassword = v.Password;

                if (model.OldPassword == OldPassword)
                {


                    string new_password = Convert.ToString(model.NewPassword);
                    v.Password = new_password;
                    try
                    {
                        entities.Configuration.ValidateOnSaveEnabled = false;
                        entities.SaveChanges();
                        ViewBag.Message = "Password Change !!! ";
                    }
                    catch (Exception e)
                    {
                        return View(e.Message);
                    }

                }
                else
                {
                    ViewBag.Message = "Old Password is not matching";
                }



            }
            return View();
        }

        [NonAction]
        public void SendverificationLinkEmail(string email, string activationCode, string emailfor = "VerifyAccount")
        {
            var verifyUrl = "/Account/" + emailfor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("priyankjitensanghavi@gmail.com", "priyank");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "priyank1999"; //Password
            string subject = "";
            string body = "";


            if (emailfor == "VerifyAccount")
            {
                subject = "Your Account is successfully Created!";

                body = "<table style='margin - top:30px; margin - left:30px;'>" +
                 "<tr>  <th scope = 'col' ><img src = '~/img/logo.png' alt = 'logo' ></th> </tr >" +
                 "<tr>  <td ><p  style=' font - family: sans - serif;font - size: 26px ;font - weight: 600;line - height: 30px; color: #6255a5; margin - bottom:30px;'>Email Verification</p></td></tr >" +
                 "<tr><td><p  style=' font-family: sans-serif;font-size: 18px; font-weight: 600;line-height: 22px;color: #333333;margin-bottom:20px ;'> Dear " + email + "</p></td></tr>" +
                 "<tr><td><p  style ='font-family: sans-serif; font-size: 16px;font-weight: 400;line-height: 20px;color: #333333;'>Thanks for Signing up!</p><p style ='font-family: sans-serif; font-size: 16px;font-weight: 400;line-height: 20px;color: #333333;'>Simply click below for email verification</p></td></tr>" +
                 "<tr><td><button  style='background: #6255a5;color: #ffffff;height: 50px;width: 540px;border-radius: 3px;font-family: sans-serif;font-weight: 600;font-size: 18px;line-height: 22px;border: 1px solid #d1d1d1;'> <a href= '" + link + "' style='color:#ffffff'>VERIFY EMAIL ADDRESS</a></button></td></tr></table>";



            }

            else if (emailfor == "ResetPassword")
            {

                subject = "Reset Password";
                body = "Hi,<br/>We got request for rreset your account password.<br/>" +
                    "your Password is:" + activationCode + "";
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)


            };
            using (var Message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })

                smtp.Send(Message);
        }

        [NonAction]
        public bool IsEmailExist(string Email)
        {
            using (NoteMarketPlaceEntities entities = new NoteMarketPlaceEntities())
            {
                var v = entities.Users.Where(a => a.EmailID == Email).FirstOrDefault();
                return v == null ? false : true;
            }
        }

    }
}