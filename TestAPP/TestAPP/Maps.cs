using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestAPP
{
    public class Maps //: INotifyPropertyChanged
    {
        private string CountryList;
        private string Description;
        private string pays;

        /*
        private ImageSource image;
        private ImageSource contactType;
        */

        public Maps(string name, string number)
        {
            CountryList = name;
            Description = number;
        }


        public string countryList
        {
            get { return CountryList; }
           
            /*set
            {
                if (contactName != value)
                {
                    contactName = value;
                    this.RaisedOnPropertyChanged("ContactName");
                }
            }*/
        }

        public string description
        {
            get { return Description; }
            /*
            set
            {
                if (contactNumber != value)
                {
                    contactNumber = value;
                    this.RaisedOnPropertyChanged("ContactNumber");
                }
            }*/
        }

        public string Pays
        {
            get { return pays; }
            set
            {
                if (pays != value)
                {
                    pays = value;
                    //this.RaisedOnPropertyChanged("ContactNumber");
                }
            }
        }       

        /*
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisedOnPropertyChanged(string _PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(_PropertyName));
            }
        }*/
    }
}
