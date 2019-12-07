using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techciesto.InstagramScrapper
{
    public class InstaProfile
    {
        public string Name { get; set; }

        public string Username { get; set; }

        public string ProfileUrl { get; set; }

        public string ProfilePicture { get; set; }

        public int FollowerCount { get; set; }

        public int FollowCount { get; set; }

        public int PostCount { get; set; }

        public List<string> PostThumnails { get; set; }
    }
}
