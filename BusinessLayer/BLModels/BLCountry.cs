using BusinessLayer.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.BLModels
{
    public class BLCountry
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public virtual ICollection<BLUser> Users { get; set; } = new List<BLUser>();
    }
}
