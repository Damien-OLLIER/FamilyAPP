
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestAPP
{
    public class MapsViewModel : INotifyPropertyChanged
    {
        #region Properties

        public ObservableCollection<Maps> Items { get; set; }

        #endregion

        #region Fields

        Random random = new Random(123456789);

        #endregion

        #region Constructor

        public MapsViewModel()
        {
            Items = new ObservableCollection<Maps>();

            for (int i = 0; i < CountryList.Count(); i++)
            {
                var MapsInfo = new Maps(CountryList[i], Description[i]);
                MapsInfo.Pays = Description[i];
                Items.Add(MapsInfo);
            }
        }
       
        #endregion

        #region Fields

        string[] Description = new string[] {
            "Voyage en Italie, visite de venise avec la louloute",
            "Voyage en Autriche, visite de Innsbruck en Fevrier 2022 avec la louloute",
            "Voyage en Vélo, la loire à vélo avec la louloute",
            "Voyage en Suisse, visite de Zurich avec la louloute",
            "Voyage en Suisse, visite des chutes du Rhin avec la louloute",
            "Voyage en Suisse, visite de Lucerne avec la louloute",
            "Voyage en Suisse, visite de Thoune avec la louloute",
            "Journée ski de fond avec la famille de Camille",
            "Voyage au Pays-Bas, visite de Amsterdam en amoureux avec la louloute",
            "Nouvel an à la montage, à Morzine avec camilles et ses potes",
            "Week-end à la montagne, à Morzine avec Camille et ses potes",
            "Photo du confinement avec la louloute",
            "Photo de la vie quotidienne avec la louloute en 2022",
            "Photo de la vie quotidienne avec la louloute en 2021",
            "Photo de la vie quotidienne avec la louloute en 2020",
        };

        string[] CountryList = new string[] {
            "Italie",
            "Autriche",
            "Loire",
            "Zurich",
            "Rhin",
            "Lucerne",
            "Thoune",            
            "SkiDeFond",
            "PaysBas",
            "MorzineNouvelAn",
            "MorzineJuin",
            "Confinement",
            "2022",
            "2021",
            "2020",
        };

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
