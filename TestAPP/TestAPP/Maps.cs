using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestAPP
{
    public class Maps 
    {
        private string CountryList;
        private string Description;
        private string Gitname;
        private string pays;

        public Maps(string name, string number, string gitname)
        {
            CountryList = name;
            Description = number;
            Gitname = gitname;
        }

        public string countryList
        {
            get { return CountryList; }
        }

        public string description
        {
            get { return Description; }
        }

        public string gitname
        {
            get { return Gitname; }
        }
        public string Pays
        {
            get { return pays; }
            set
            {
                if (pays != value)
                {
                    pays = value;
                }
            }
        }       
    }
}
