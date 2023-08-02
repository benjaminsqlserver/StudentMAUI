using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentMAUI.Models.ConData
{
    [Table("Students", Schema = "dbo")]
    public partial class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string FirstName { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string LastName { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string EmailAddress { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int GenderID { get; set; }

        public Gender Gender { get; set; }

    }
}