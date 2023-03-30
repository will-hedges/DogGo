using System;

namespace DogGo.Models
{
    public class Walk
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Duration { get; set; }
        public int WalkerId { get; set; }
        public int DogId { get; set; }
        public Owner Owner { get; set; }
    }
}
