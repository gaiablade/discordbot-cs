using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

namespace DiscordBot
{
    public class Activity : IActivity
    {
        private string details;

        public string Name => "Name";

        public ActivityType Type => ActivityType.Playing;

        public ActivityProperties Flags => ActivityProperties.None;

        public string Details => details;

        public Activity(string text)
        {
            details = text;
        }
    }
}
