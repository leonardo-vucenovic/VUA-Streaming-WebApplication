﻿using BusinessLayer.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.BLModels
{
    public class BLVideo
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public int GenreId { get; set; }

        public int TotalSeconds { get; set; }

        public string? StreamingUrl { get; set; }

        public int? ImageId { get; set; }

        public virtual BLGenre Genre { get; set; } = null!;

        public virtual BLImage? Image { get; set; }

        public virtual ICollection<BLVideoTag> VideoTags { get; set; } = new List<BLVideoTag>();
    }
}
