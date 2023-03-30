using System.Collections.Generic;

namespace DogGo.Models.ViewModels
{
    public class WalkerProfileViewModel
    {
        public Walker Walker { get; set; }
        public List<Walk> RecentWalks { get; set; }
    }
}
