
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace TestAPP
{
    public class MapsViewModel : INotifyPropertyChanged
    {
        #region Properties

        public ObservableCollection<Maps> Items { get; set; }
        List<Place> placesList = new List<Place>();

        #endregion

        #region Fields

        Random random = new Random(123456789);

        #endregion

        #region Constructor

        public MapsViewModel()
        {
            // On recupere les Infos du Fichier JSON (Longitude, Latitude, etc...)
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainPage)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("TestAPP.Places.json");

            // On met tout ça en format text 
            string text = string.Empty;
            using (var reader = new StreamReader(stream, Encoding.UTF7))
            {
                text = reader.ReadToEnd();
            }

            // On creer l'objet  "resultObject" qui contient toutes les infos
            var resultObject = JsonConvert.DeserializeObject<Places>(text);

            // Pour chaque iteration de "resultObject", on a acces à ses parametres tels que l'adressem la position, etc...
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
                var MapsInfo = new Maps(placesList[i].PlaceName, placesList[i].Address);
                MapsInfo.Pays = placesList[i].Address;
                Items.Add(MapsInfo);
            }
        }
       
        #endregion

        #region Fields

        //string[] Description = new string[] {
        //    "Voyage en Italie, visite de venise avec la louloute",
        //    "Voyage en Autriche, visite de Innsbruck en Fevrier 2022 avec la louloute",
        //    "Voyage en Vélo, la loire à vélo avec la louloute",
        //    "Voyage en Suisse, visite de Zurich avec la louloute",
        //    "Voyage en Suisse, visite des chutes du Rhin avec la louloute",
        //    "Voyage en Suisse, visite de Lucerne avec la louloute",
        //    "Voyage en Suisse, visite de Thoune avec la louloute",
        //    "Journée ski de fond avec la famille de Camille",
        //    "Voyage au Pays-Bas, visite de Amsterdam en amoureux avec la louloute",
        //    "Nouvel an à la montage, à Morzine avec camilles et ses potes",
        //    "Week-end à la montagne, à Morzine avec Camille et ses potes",
        //    "Photo du confinement avec la louloute",
        //    "Photo de la vie quotidienne avec la louloute en 2022",
        //    "Photo de la vie quotidienne avec la louloute en 2021",
        //    "Photo de la vie quotidienne avec la louloute en 2020",
        //};

        //string[] CountryList = new string[] {
        //    "Italie",
        //    "Autriche",
        //    "Loire",
        //    "Zurich",
        //    "Rhin",
        //    "Lucerne",
        //    "Thoune",            
        //    "SkiDeFond",
        //    "PaysBas",
        //    "MorzineNouvelAn",
        //    "MorzineJuin",
        //    "Confinement",
        //    "2022",
        //    "2021",
        //    "2020",
        //};

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
