using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCNoteMarketPlace.Models
{
    public class Notedetail
    {
        public SellerNote sellernote { get; set; }
        public List<SellerNotesReview> review { get; set; }
        public SellerNotesReportedIssue issue { get; set; }
        public UserProfile up { get; set; }
        public SellerNotesAttachement sellerNotesAttachement { get; set; }
    }
}