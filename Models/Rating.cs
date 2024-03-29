﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ratings.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Range(1, 5, ErrorMessage = "Musisz ocenić utwór")]
        [Column(TypeName = "decimal(3, 2)")]
        public decimal RatingValue { get; set; }

        [MaxLength(4096, ErrorMessage = "Recenzja musi się składać z mniej niż 4096 znaków")]
        public string Review { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public int WorkId { get; set; }

        public Work Work { get; set; }
    }
}
