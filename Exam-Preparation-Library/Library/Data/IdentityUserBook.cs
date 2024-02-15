﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Data
{
    [Comment("User Books")]
    public class IdentityUserBook
    {
        [Comment("Book Collector")]
        public string CollectorId { get; set; } = null!;

        [Comment("Collector")]
        [ForeignKey(nameof(CollectorId))]
        public IdentityUser Collector { get; set; } = null!;

        [Comment("Book Id")]
        public int BookId { get; set; }

        [Comment("Book")]
        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; } = null!;
    }
}
