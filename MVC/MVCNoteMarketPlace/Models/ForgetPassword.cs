using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace MVCNoteMarketPlace.Models
{
    public class ForgetPassword
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}