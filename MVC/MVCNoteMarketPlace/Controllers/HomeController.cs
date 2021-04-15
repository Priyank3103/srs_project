using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MVCNoteMarketPlace.Models;
using System.Net.Mail;
using System.Web.Security;
using System.Configuration;
using System.Web.Hosting;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity;
using System.Runtime.CompilerServices;
using System.Web.UI;
using System.IO;

namespace MVCNoteMarketPlace.Controllers
{
    public class HomeController : Controller
    {
        NoteMarketPlaceEntities entities = new NoteMarketPlaceEntities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(string Name, string Email, string Subject, string Comment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var senderEmail = new MailAddress("priyankjitensanghavi@gmail.com", Name);
                    var receiverEmail = new MailAddress("priyankjitensanghavi@gmail.com", "Receiver");
                    var password = "priyank1999";
                    var sub = Subject;
                    var body = Name;
                    var cmnt = Comment;
                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new System.Net.NetworkCredential(senderEmail.Address, password)
                    };
                    using (var mess = new MailMessage(senderEmail, receiverEmail)
                    {
                        Subject = Subject,
                        Body = Email + "\n " + body + " \n" + cmnt

                    })
                    {
                        smtp.Send(mess);
                    }
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.Error = "Some Error";
            }
            return View();
        }

            public void ApplySorting(string Sortorder, string SortBy, List<SellerNote> data)
        {
            switch (SortBy)
            {
                case "Added Date":
                    {
                        switch (Sortorder)
                        {
                            case "Asc":
                                {
                                    data = data.OrderBy(a => a.CreatedDate).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    data = data.OrderByDescending(a => a.CreatedDate).ToList();
                                    break;
                                }
                            default:
                                {
                                    data = data.OrderBy(a => a.CreatedDate).ToList();
                                    break;
                                }
                        }

                        break;
                    }
                case "Title":
                    {
                        switch (Sortorder)
                        {
                            case "Asc":
                                {
                                    data = data.OrderBy(a => a.Title).ToList();
                                    break;
                                }
                            case "Desc":
                                {
                                    data = data.OrderByDescending(a => a.Title).ToList();
                                    break;
                                }
                            default:
                                {
                                    data = data.OrderBy(a => a.Title).ToList();
                                    break;
                                }
                        }
                        break;
                    }
            }
        }

        public List<SellerNote> ApplyPagination(List<SellerNote> data, int Pagenumber)
        {

            ViewBag.TotalPages = Math.Ceiling(data.Count / 5.0);

            ViewBag.PageNumber = Pagenumber;

            data = data.Skip((Pagenumber - 1) * 5).Take(5).ToList();

            return data;
        }


        [Authorize]
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult DashBoard(string searchTxt, string Sortorder, string SortBy, int Pagenumber = 1)
        {
            ViewBag.Sortorder = Sortorder;
            ViewBag.SortBy = SortBy;

            string userId = User.Identity.Name;
            var v = entities.Users.Where(a => a.EmailID == userId).FirstOrDefault();
            var sellerID = v.ID;
            var data = entities.SellerNotes.Where(a => a.SellerID == sellerID).ToList();

            if (searchTxt != null)
            {
                data = entities.SellerNotes.Where(a => a.Title.Contains(searchTxt)).ToList();
                ApplySorting(Sortorder, SortBy, data);
                data = ApplyPagination(data, Pagenumber);
            }
            else
            {
                ApplySorting(Sortorder, SortBy, data);
                data = ApplyPagination(data, Pagenumber);
            }

            return View(data);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Addnote()
        {
            var notetype = entities.NoteTypes.ToList();
            ViewBag.Notetype = new SelectList(notetype, "ID", "Name");

            var notecategory = entities.NoteCategories.ToList();
            ViewBag.Notecategory = new SelectList(notecategory, "ID", "Name");

            var country = entities.Countries.ToList();
            ViewBag.country = new SelectList(country, "ID", "Name");

            
            return View();
        }

        [Authorize]
        [HttpPost]

        public ActionResult AddNote(Addnote add)
        {
            string filename = Path.GetFileNameWithoutExtension(add.SellerNote.DisplayFile.FileName);

            string extension = Path.GetExtension(add.SellerNote.DisplayFile.FileName);

            filename = filename + DateTime.Now.ToString("yymmssff") + extension;
            filename = Path.Combine(Server.MapPath("~/img/pre-login"), filename);
            add.SellerNote.DisplayFile.SaveAs(filename);

            string userId = User.Identity.Name;
            var v = entities.Users.Where(a => a.EmailID == userId).FirstOrDefault();
            var sellerID = v.ID;

            //bool paid;

            //if (add.SellerNote.IsPaid == true)
            //{
            //    paid = true;
            //}
            //else
            //{
            //    paid = false;
            //}
            SellerNote seller = new SellerNote()
            { 
                SellerID = sellerID,
                Status = 4,
                PublishedDate = DateTime.Now,
                Title = add.SellerNote.Title,
                Category = add.SellerNote.Category,
                NoteType = add.SellerNote.NoteType,
                NumberofPages = add.SellerNote.NumberofPages,
                Description = add.SellerNote.Description,
                UniversityName = add.SellerNote.UniversityName,
                Country = add.SellerNote.Country,
                Course = add.SellerNote.Course,
                IsPaid = add.SellerNote.IsPaid,
                SellingPrice = add.SellerNote.SellingPrice,
                CourseCode = add.SellerNote.CourseCode,
                Professor = add.SellerNote.Professor,
                IsActive = true,
                DisplayPicture = filename,
                CreatedDate = DateTime.Now,
                Createdby = sellerID
            };
            
            entities.SellerNotes.Add(seller);
            entities.SaveChanges();
            

            var noteid = entities.SellerNotes.Where(a => a.DisplayPicture == filename).FirstOrDefault();

            string filename_pdf = Path.GetFileNameWithoutExtension(add.SellerNotesAttachement.UserImageFile.FileName);

            string extension_pdf = Path.GetExtension(add.SellerNotesAttachement.UserImageFile.FileName);

            filename_pdf = filename_pdf + DateTime.Now.ToString("yymmssff") + extension_pdf;
            string full_filename_pdf = Path.Combine(Server.MapPath("~/img/"), filename_pdf);
            add.SellerNotesAttachement.UserImageFile.SaveAs(full_filename_pdf);

            SellerNotesAttachement sellernoteattachement = new SellerNotesAttachement()
            {
                NoteID = noteid.ID,
                FileName = filename_pdf,
                FilePath = full_filename_pdf,
                IsActive = true,
                CreatedDate = DateTime.Now,
                CreatedBy = sellerID
            };

            entities.SellerNotesAttachements.Add(sellernoteattachement);
            entities.SaveChanges();
           
            ModelState.Clear();

            var notetype = entities.NoteTypes.ToList();
            ViewBag.Notetype = new SelectList(notetype, "ID", "Name");

            var notecategory = entities.NoteCategories.ToList();
            ViewBag.Notecategory = new SelectList(notecategory, "ID", "Name");

            var country = entities.Countries.ToList();
            ViewBag.country = new SelectList(country, "ID", "Name");

            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult EditNote(int id)
        {
            var note = entities.SellerNotes.Where(x => x.ID == id).FirstOrDefault();


            var notetype = entities.NoteTypes.ToList();
            ViewBag.Notetype = new SelectList(notetype, "ID", "Name");

            var notecategory = entities.NoteCategories.ToList();
            ViewBag.Notecategory = new SelectList(notecategory, "ID", "Name");

            var country = entities.Countries.ToList();
            ViewBag.country = new SelectList(country, "ID", "Name");

            Addnote add = new Addnote()
            {
                SellerNote = note
            };

            return View(add);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditNote(Addnote add)
        {
            var noteID = entities.SellerNotes.Where(x => x.ID == add.SellerNote.ID).FirstOrDefault();

            string filename = Path.GetFileNameWithoutExtension(add.SellerNote.DisplayFile.FileName);

            string extension = Path.GetExtension(add.SellerNote.DisplayFile.FileName);

            filename = filename + DateTime.Now.ToString("yymmssff") + extension;
            filename = Path.Combine(Server.MapPath("~/img/pre-login"), filename);
            add.SellerNote.DisplayFile.SaveAs(filename);

            string userId = User.Identity.Name;
            var v = entities.Users.Where(a => a.EmailID == userId).FirstOrDefault();
            var sellerID = v.ID;


            noteID.SellerID = sellerID;
            noteID.Status = 4;
            noteID.PublishedDate = DateTime.Now;
            noteID.Title = add.SellerNote.Title;
            noteID.Category = add.SellerNote.Category;
            noteID.NoteType = add.SellerNote.NoteType;
            noteID.NumberofPages = add.SellerNote.NumberofPages;
            noteID.Description = add.SellerNote.Description;
            noteID.UniversityName = add.SellerNote.UniversityName;
            noteID.Country = add.SellerNote.Country;
            noteID.Course = add.SellerNote.Course;
            noteID.CourseCode = add.SellerNote.CourseCode;
            noteID.Professor = add.SellerNote.Professor;
            noteID.IsActive = true;
            noteID.DisplayPicture = filename;
            noteID.CreatedDate = DateTime.Now;
            noteID.Createdby = sellerID;

            entities.SaveChanges();


            var attachment = entities.SellerNotesAttachements.Where(x => x.NoteID == add.SellerNote.ID).FirstOrDefault();

            string filename_pdf = Path.GetFileNameWithoutExtension(add.SellerNotesAttachement.UserImageFile.FileName);

            string extension_pdf = Path.GetExtension(add.SellerNotesAttachement.UserImageFile.FileName);

            filename_pdf = filename_pdf + DateTime.Now.ToString("yymmssff") + extension_pdf;
            string full_filename_pdf = Path.Combine(Server.MapPath("~/img/"), filename_pdf);
            add.SellerNotesAttachement.UserImageFile.SaveAs(full_filename_pdf);


            attachment.NoteID = noteID.ID;
            attachment.FileName = filename_pdf;
            attachment.FilePath = full_filename_pdf;
            attachment.IsActive = true;
            attachment.CreatedDate = DateTime.Now;
            attachment.CreatedBy = sellerID;

            entities.SaveChanges();

            ModelState.Clear();

            var notetype = entities.NoteTypes.ToList();
            ViewBag.Notetype = new SelectList(notetype, "ID", "Name");

            var notecategory = entities.NoteCategories.ToList();
            ViewBag.Notecategory = new SelectList(notecategory, "ID", "Name");

            var country = entities.Countries.ToList();
            ViewBag.country = new SelectList(country, "ID", "Name");

            return RedirectToAction("DashBoard", "Home");
        }
        
        public ActionResult Delete(int id)
        {
            var NoteId = entities.SellerNotes.Where(x => x.ID == id).FirstOrDefault();
            var SellerNoteAttachement = entities.SellerNotesAttachements.Where(x => x.NoteID == id).FirstOrDefault();
            

            if (SellerNoteAttachement != null)
            {
                entities.SellerNotesAttachements.Remove(SellerNoteAttachement);
            }
            

            entities.SellerNotes.Remove(NoteId);
            entities.SaveChanges();
            return RedirectToAction("DashBoard", "Home");
        }

        public FileResult DownloadFile(int id)
        {

            var f = entities.SellerNotesAttachements.Where(x => x.NoteID == id).FirstOrDefault();
            string filepath = f.FilePath;
            return File(filepath, "application/pdf", "Test.pdf");
        }

        public ActionResult Notedetails(int id)
        {
            if (Request.IsAuthenticated)
            {
                string email = User.Identity.Name;
                ViewBag.Email = email;
            }
            else
            {
                ViewBag.Email = null;
            }

            var data = entities.SellerNotes.Where(x => x.ID == id).FirstOrDefault();
            ViewBag.Id = id;
            ViewBag.price = '$' + Convert.ToString(data.SellingPrice);

            ViewBag.Issue = entities.SellerNotesReportedIssues.Where(x => x.NoteID == id).Count();



            if (ViewBag.Issue != 0)
            {
                var Review_Text = entities.SellerNotesReviews.Where(x => x.NoteID == id).ToList();
                Notedetail nd = new Notedetail()
                {
                    sellernote = data,
                    review = Review_Text

                };
                return View(nd);
            }
            else
            {
                Notedetail nd = new Notedetail()
                {
                    sellernote = data
                };
                return View(nd);
            }
        }

        public ActionResult AddBuyerRequest(int id, string buyer)
        {
            var user = entities.Users.Where(x => x.EmailID == buyer).FirstOrDefault();
            var Down_Id = user.ID;
            var seller = entities.SellerNotes.Where(x => x.ID == id).FirstOrDefault();
            var seller_id = seller.SellerID;
            var file = entities.SellerNotesAttachements.Where(x => x.NoteID == id).FirstOrDefault();
            var path = file.FilePath;

            Download down = new Download()
            {
                NoteID = id,
                Seller = seller_id,
                Downloader = Down_Id,
                IsSellerHasAllowedDownload = false,
                AttachmentPath = path,
                IsAttachmentDownloaded = false,
                IsPaid = true,
                PurchasedPrice = seller.SellingPrice,
                NoteTitle = seller.Title,
                NoteCategory = seller.NoteCategory1.Name,
                CreatedDate = DateTime.Now,
                CreatedBy = Down_Id

            };
            entities.Downloads.Add(down);
            entities.SaveChanges();

            MailMessage mm = new MailMessage();
            SmtpClient smtp = new SmtpClient();

            mm.To.Add(user.EmailID);
            mm.From = new MailAddress("priyankjitensanghavi@gmail.com");
            mm.Subject = "Note Marketplace";
            mm.Body = "Hello " + user.FirstName +
                " Thank you so much for buying the item. you can download your item soon.";
                
            mm.IsBodyHtml = false;

            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = true;
            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("priyankjitensanghavi@gmail.com", "priyank1999");
            smtp.Send(mm);

            return RedirectToAction("Notedetails", "Home", new { Id = id});
        }

        [Authorize]
        public ActionResult BuyerRequest()
        {
            string userId = User.Identity.Name;
            var v = entities.Users.Where(a => a.EmailID == userId).FirstOrDefault();
            var sellerID = v.ID;
            var data = entities.Downloads.Where(a => (a.Seller == sellerID) && (a.IsSellerHasAllowedDownload == false)).ToList();

            return View(data);
        }

        public ActionResult Faq()
        {
            return View();
        }

    }
}