using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotCS.HttpResponseTemplates.WebsterThesaurusResponse
{
    public class Target
    {
        public string tuuid { get; set; }
        public string tsrc { get; set; }
    }

    public class Meta
    {
        public string id { get; set; }
        public string uuid { get; set; }
        public string src { get; set; }
        public string section { get; set; }
        public Target target { get; set; }
        public List<string> stems { get; set; }
        public List<List<string>> syns { get; set; }
        public List<List<string>> ants { get; set; }
        public bool offensive { get; set; }
    }

    public class Hwi
    {
        public string hw { get; set; }
    }

    public class Def
    {
        public List<List<List<object>>> sseq { get; set; }
    }

    public class WebsterThesaurusResponse
    {
        public Meta meta { get; set; }
        //public Hwi hwi { get; set; }
        //public string fl { get; set; }
        //public List<Def> def { get; set; }
        //public List<string> shortdef { get; set; }
        //public List<string> sls { get; set; }
    }
}
