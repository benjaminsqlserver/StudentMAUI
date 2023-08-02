using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMAUI.Models.ConData
{
    [Table("Genders", Schema = "dbo")]
    public partial class Gender
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GenderID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string GenderName { get; set; }

        public ICollection<Student> Students { get; set; }

    }
}