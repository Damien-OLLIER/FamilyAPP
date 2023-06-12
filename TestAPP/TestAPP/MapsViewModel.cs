
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
using Xamarin.Essentials;
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
        static public List<Place> placesList { get; set; }

    #endregion

        #region Fields

    Random random = new Random(123456789);

        #endregion

        #region Constructor

        public MapsViewModel(JArray contents)
        {
            RespoJSON = contents;

            JArray items = new JArray();

            foreach (var file in contents)
            {
                var filetype = (string)file["type"];
                if (filetype == "dir")
                {
                    var Name = (string)file["name"];

                    if (Name.Contains("Video") || Name.Contains(".vs") || Name.Contains("Video") || Name.Contains("Menu")) 
                    {
                       // RespoJSON.Remove(file);
                    }
                    else 
                    {
                        items.Add(file);
                    }
                }
                else if (filetype == "file")
                {
                    var downloadurl = (string)file["download_url"];

                    if (downloadurl.Contains("TestVideo1.mp4") || downloadurl.Contains("Video") || downloadurl.Contains(".vs") || downloadurl.Contains("Menu") || downloadurl.Contains("Test"))
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

                            placesList = new List<Place>();  


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

            RespoJSON = items;
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
