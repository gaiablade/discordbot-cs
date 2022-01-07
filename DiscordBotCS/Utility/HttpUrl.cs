using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBotCS.Utility
{
    public enum HttpProtocol
    {
        Http,
        Https
    }

    public class HttpUrl
    {
        private static Regex urlRegex = new Regex(@"(https://|http://)?(www\.)?([\d\w@:._\+~#=]{1,256}\.[\w\d()]{1,6})\b([\w\d()@:%_\+.~#?&//=]*)");
        private static Regex pathRegex = new Regex(@"([\w\d()@:%_\+.~#//]*)");
        private static Regex paramRegex = new Regex(@"[&?]([\w\d_]+)=([\w\d_]+)");

        public HttpProtocol Protocol { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public KeyValuePair<string, string>[] Parameters { get; set; } = new KeyValuePair<string, string>[] { };

        public static HttpUrl? ParseUrl(string url)
        {
            var match = urlRegex.Match(url);
            if (!match.Success)
            {
                return null;
            }
            HttpUrl httpUrl = new HttpUrl();

            if (!match.Groups[1].Success)
            {
                httpUrl.Protocol = HttpProtocol.Https;
            }
            else
            {
                httpUrl.Protocol = match.Groups[1].Value switch
                {
                    "https://" => HttpProtocol.Https,
                    "http://" => HttpProtocol.Http,
                    _ => HttpProtocol.Https
                };
            }

            httpUrl.Url = match.Groups[3].Value;

            var pathMatch = pathRegex.Match(match.Groups[4].Value);
            if (!pathMatch.Success)
            {
                httpUrl.Path = pathMatch.Groups[1].Value;
            }

            var paramMatches = paramRegex.Matches(match.Groups[4].Value);
            if (paramMatches.Count > 0)
            {
                foreach (var param in paramMatches.ToList())
                {
                    if (param.Success)
                    {
                        httpUrl.Parameters.Append(new KeyValuePair<string, string>(param.Groups[1].Value, param.Groups[2].Value));
                    }
                }
            }

            return httpUrl;
        }
    }
}
