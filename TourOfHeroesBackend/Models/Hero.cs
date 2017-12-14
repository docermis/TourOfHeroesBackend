using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourOfHeroesBackend.Models
{
    public class Hero
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Power { get; set; }
        public string Identity { get; set; }
    }
}
