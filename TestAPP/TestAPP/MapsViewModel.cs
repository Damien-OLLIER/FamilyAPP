
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using static TestAPP.MainPage;

namespace TestAPP
{
    public class MapsViewModel : INotifyPropertyChanged
    {
        #region Properties
        static public JArray RespoJSON { get; set; }

        public ObservableCollection<Maps> Items { get; set; }
        List<Place> placesList = new List<Place>();

        #endregion

        #region Fields

        Random random = new Random(123456789);

        #endregion

        #region Constructor

        public MapsViewModel()
        {
            placesList.Clear();

            // On recupere les Infos du Fichier JSON (Longitude, Latitude, etc...)
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("TestAPP.Places.json");

            // On met tout ça en format text 
            string text = string.Empty;
            using (var reader = new StreamReader(stream, Encoding.UTF7))
            {
                text = reader.ReadToEnd();
            }

            //On creer l'objet  "resultObject" qui contient toutes les infos
            var resultObject = JsonConvert.DeserializeObject<Places>(text);

            foreach (var place in resultObject.results)
            {
                placesList.Add(new Place
                {
                    PlaceName = place.name,
                    Address = place.vicinity,
                    Location = place.geometry.location,
                    Position = new Position(place.geometry.location.lat, place.geometry.location.lng),
                });
            }

            Items = new ObservableCollection<Maps>();

            for (int i = 0; i < placesList.Count(); i++)
            {
                var MapsInfo = new Maps(placesList[i].PlaceName, placesList[i].Address, "test2");
                MapsInfo.Pays = placesList[i].Address;
                Items.Add(MapsInfo);
            }

            //Pour chaque iteration de "resultObject", on a acces à ses parametres tels que l'adressem la position, etc...
        }
        public MapsViewModel(JArray contents)
        {
            RespoJSON = contents;

            foreach (var file in contents)
            {
                var filetype = (string)file["type"];
                if (filetype == "dir")
                {

                }
                else if (filetype == "file")
                {
                    var downloadurl = (string)file["download_url"];

                    if (downloadurl.Contains("TestVideo1.mp4"))
                    {

                    }
                    else
                    {
                        using (WebClient wc = new WebClient())
                        {
                            Debug.WriteLine("");

                            var b = wc.Encoding;

                            var json = wc.DownloadString(downloadurl);

                            var ob = JsonConvert.DeserializeObject<Places>(json);

                            foreach (var place in ob.results)
                            {
                                placesList.Add(new Place
                                {
                                    PlaceName = place.name,
                                    Address = place.vicinity,
                                    Location = place.geometry.location,
                                    Gitname = place.Gitname,
                                    Position = new Position(place.geometry.location.lat, place.geometry.location.lng),
                                });
                            }

                            Items = new ObservableCollection<Maps>();

                            for (int i = 0; i < placesList.Count(); i++)
                            {
                                var MapsInfo = new Maps(placesList[i].PlaceName, placesList[i].Address, placesList[i].Gitname);
                                MapsInfo.Pays = placesList[i].Address;
                                Items.Add(MapsInfo);
                            }
                        }
                    }
                }
            }            
        }

       
        #endregion

        #region Fields

        #endregion

        #region Interface Member

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
