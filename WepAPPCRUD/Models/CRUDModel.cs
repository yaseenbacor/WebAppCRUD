using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WepAPPCRUD.Models
{
    public class CRUDModel
    {

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }
        
        [Required(ErrorMessage = "You must provide a cell phone number")]
        [Display(Name = "Cell Phone")]
        [RegularExpression(@"^(\d{11})$", ErrorMessage = "Wrong mobile")]
        public string CellphoneNo { get; set; }

        public bool IsEdit { get; set; }

    }
}