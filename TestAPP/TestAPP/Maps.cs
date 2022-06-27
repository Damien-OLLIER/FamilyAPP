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
        private string pays;

        public Maps(string name, string number)
        {
            CountryList = name;
            Description = number;
        }

        public string countryList
        {
            get { return CountryList; }
        }

        public string description
        {
            get { return Description; }
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
