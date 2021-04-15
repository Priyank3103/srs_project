using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity; 

namespace MVCNoteMarketPlace.Models
{
    public class Addnote
    {
        public SellerNote SellerNote { get; set; }
        public SellerNotesAttachement SellerNotesAttachement { get; set; }
    }
}