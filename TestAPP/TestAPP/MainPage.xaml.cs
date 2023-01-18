using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Plugin.Messaging;
using Syncfusion.ListView.XForms;
using Syncfusion.SfPicker.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Linq;
using MediaManager;

namespace TestAPP
{

    public partial class MainPage : Xamarin.Forms.TabbedPage
    {
        // ObservableCollection<Family> est une collection d'objet de la classe Family utilsé dans l'onglet family afin d'afficher l'expander (family  Tree)
        public ObservableCollection<Family> MyFamily { get => GetFamilyInfo(); }

        //Accesseur et mutateur pour Le message envoyé de base dans le texto
        public string message { get; private set; }
        //Accesseur et mutateur pour Le numero de telephone envoyé de base avec le texto
        public string numero { get; private set; }
        public int NumberOfItems { get; private set; }
        public string LocationJSON { get; private set; }

        //private string videoUrl = "https://sec.ch9.ms/ch9/e68c/690eebb1-797a-40ef-a841-c63dded4e68c/Cognitive-Services-Emotion_high.mp4";
        private string videoUrl = "https://raw.githubusercontent.com/Damien-OLLIER/AppPictures/main/Video/TestVideo1.mp4";
        //https://api.github.com/repos/Damien-OLLIER/AppPictures/contents
        //List contenant des objets de la classe Place afin de creer les Pin Maps
        List<Place> placesList = new List<Place>();

        //Method called when the APP is started
        public MainPage()
        {
            //clef/license pour les fonctionalites de syncfusion (Methodes et bouttons,etc ...)
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTkzMjMwQDMxMzkyZTM0MmUzMGxLakdmUERlTjd4OHdYVnJ2WVlDSkhnSHZUWmFRa2swYmNEa0RnUFhIUGs9");
            InitializeComponent();

            //var test = HomeGrid.RowDefinitions;

            message = "je t'aime !"; // Message de base affiché et envoyé au Num
            numero = "+33632183163"; // Numero selectionne de base

            UpdateMap();

        }

        //Methode qui permet d'initialiser le visuel de l'onglet Maps
        private async void UpdateMap()
        {
            try
            {// Pour eviter que l'app crash

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

                //On met tout ça dans une liste que l'on Fourni a "MyMap" comme item Source
                MyMap.ItemsSource = placesList;

                //De base, map nous affiche les etats-unis. On bouge donc au milieu de l'europe à chaque demarrage
                //TO DO: le commentaire du haut est faux, il faut changer la ligne du dessous car je suis con. Depuis le debut je mou fou au etats unis pour me mettre en europe, j'avais oublie l'existance de cette ligne 
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(47.6370891183, -122.123736172), Distance.FromKilometers(100)));
            }
            catch (Exception ex)
            {
                //affichage de l'error dans la console (Output)
                Debug.WriteLine(ex);
            }
        }

        //Méthode est appelée pour ouvrir la caméra frontale
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            //Ici, sont gérées les demandes de Permission à l'utilisateur pour pouvoir acceder à la caméra.
            #region Permission
            var permission = await Permissions.CheckStatusAsync<Permissions.Camera>();

            //si la permission n'a pas encore été accordé, on la demande
            if (permission != PermissionStatus.Granted)
            {
                permission = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (permission != PermissionStatus.Granted)
            {
                // rien ne se passe si la permission n'est pas accordé
                return;
            }
            #endregion

            var opts = new MediaPickerOptions
            {
                Title = "Tu es la plus belle",
            };

            //To do: obliger la caméra frontale à s'ouvrir
            await MediaPicker.CapturePhotoAsync(opts);
        }

        //Méthode est appelée pour envoyer le texto
        private async void Button_Clicked(object sender, EventArgs e)
        {
            // Tache effectuée en fond pour afficher et demarrer le Gif pdt 3.5s puis le cacher et le stopper
            var task = Task.Run(() =>
            {
                Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    heartbgtest.IsAnimationPlaying = true;
                    GifTest.IsAnimationPlaying = true;
                    GifTest.IsVisible = true;

                });
                Thread.Sleep(3500);
                Dispatcher.BeginInvokeOnMainThread(() =>
                {
                    heartbgtest.IsAnimationPlaying = false;
                    GifTest.IsAnimationPlaying = false;
                    GifTest.IsVisible = false;
                });
            });

            try
            {
                //Ici, sont gérées les demandes de Permission à l'utilisateur pour pouvoir envyer des textos en tache de fond.
                #region Permission
                var permission = await Permissions.CheckStatusAsync<Permissions.Sms>();

                //si la permission n'a pas encore été accordé, on la demande
                if (permission != PermissionStatus.Granted)
                {
                    permission = await Permissions.RequestAsync<Permissions.Sms>();
                }

                if (permission != PermissionStatus.Granted)
                {
                    // rien ne se passe si la permission n'est pas accordé
                    return;
                }
                #endregion

                //On creer un objet de la classe SmsMessenger
                var smsMessenger = CrossMessaging.Current.SmsMessenger;

                //Si SmsMessenger est disponible, on envoie le "message" au "numero"
                if (smsMessenger.CanSendSms)
                {
                    smsMessenger.SendSmsInBackground(numero, message);
                }
            }
            catch (Exception ex)
            {
                var test = ex.Message;
            }
        }

        //To DO: to be erased ? usefull ?
        private async Task UploadPhotoAsync()
        {
            await CrossMedia.Current.Initialize();

            var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
                CompressionQuality = 40
            });

            // Convert file to byre array, to bitmap and set it to our ImageView
            var mescouilles = file.Path;
            byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
        }

        // GetFamilyInfo() retourne une collection d'objet de la classe Family utilsé dans l'onglet family afin d'afficher l'expander (family  Tree)
        private ObservableCollection<Family> GetFamilyInfo()
        {
            return new ObservableCollection<Family>
            {
                new Family { Name = "Camille", Color = "#B96CBD", Icon = "GirlIcon.PNG", IsExpanded = false, familyMember = new ObservableCollection<FamilyMember>{ new FamilyMember { BirthDate = "22 Juin 1998", Picture = "CamillePicture.jpg", NegatifPoint = "Elle a tout le temps faim", PositifPoint = "S'énerve seulement quand elle a faim", Description = "Louloute au poil court" } } },

                new Family { Name = "Damien", Color = "#49A24D", Icon = "ManIcon.PNG", IsExpanded = false, familyMember = new ObservableCollection<FamilyMember>{ new FamilyMember { BirthDate = "24 Mars 1998", Picture = "DamienPicture.jpg", NegatifPoint = "Loulou trop calin", PositifPoint = "Loulou très calin", Description = "Loulou tres calme qui est amoureux de Camille" } } },

                new Family { Name = "Coco", Color = "#FDA838", Icon = "RabbitIcon.PNG", IsExpanded = false, familyMember = new ObservableCollection<FamilyMember>{ new FamilyMember { BirthDate = "20 Septembre 2022", Picture = "RabbitPicture.jpg", NegatifPoint = "Cacher tous les câbles électriques", PositifPoint = "Très doux et adore les caresses", Description = "Petite patate qui fait fondre votre cœur" } } },

                new Family { Name = "Bounty",  Color = "#F75355",  Icon = "DogIcon.PNG", IsExpanded = false, familyMember = new ObservableCollection<FamilyMember>{ new FamilyMember { BirthDate = "Prochainement", Picture = "DogPicture.jpg", NegatifPoint = "Ramasser son caca", PositifPoint = "Il peut sauver des vies en mer", Description = "Gros loulou aussi débile que son maitre" } } },

                new Family { Name = "Wasabi",  Color = "#00C6AE", Icon = "CatIcon.PNG", IsExpanded = false, familyMember = new ObservableCollection<FamilyMember>{ new FamilyMember { BirthDate = "Prochainement", Picture = "CatPicture.jpg", NegatifPoint = "Il faut des câlins seulement quand il le souhaite", PositifPoint = "Son ronronnement vous réconfortera", Description = "Petit chat de la famille OLLIER. Très calin, il adorera reveiller camille à 4h du matin" } } },

            };
        }

        //Modification de l'ouverture de l'expander afin de le rendre plus joli
        private async Task OpenAnimation(View view, uint length = 500)
        {
            //c'est un copié collé d'internet
            view.RotationX = -90;
            view.IsVisible = true;
            view.Opacity = 0;
            _ = view.FadeTo(1, length);
            await view.RotateXTo(0, length);
        }

        //Modification de la fermeture de l'expander afin de le rendre plus joli
        private async Task CloseAnimation(View view, uint length = 500)
        {
            //c'est un copié collé d'internet
            _ = view.FadeTo(0, length);
            await view.RotateXTo(-90, length);
            view.IsVisible = false;
        }

        //Méthode est appelée quand l'expander est appuiyé afin d'ouvrir ou fermer suivant l'état initial
        //To DO: ouvrir un seul expander à la fois (fermé les autres donc)
        private async void Expander_Tapped(object sender, EventArgs e)
        {
            // On obtient les infos de l'expander lorsque l'event est trigger 
            var expander = sender as Expander;

            //on essaie d'obtenir les infos sur le detailsview 
            var imageview = expander.FindByName<StackLayout>("imageview");
            var detailsview = expander.FindByName<StackLayout>("detailsview");

            //Ouverture ou fermeture suivant l'etat initial de l'expander
            if (expander.IsExpanded)
            {
                await OpenAnimation(detailsview);
            }
            else
            {
                await CloseAnimation(detailsview);
            }
        }

        // La méthode est appelée quand l'utilisateur appui deux fois sur l'image du caroussel view sur la premiere page
        private async void OnTapGestureRecognizerTapped(object sender, EventArgs e)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("MyApplication", "1"));
            var repo = "Damien-OLLIER/AppPictures";
            var contentsUrl = $"https://api.github.com/repos/{repo}/contents";
            var contentsJson = await httpClient.GetStringAsync(contentsUrl);
            var contents = (JArray)JsonConvert.DeserializeObject(contentsJson);

            this.BindingContext = new MapsViewModel(contents);

            //Affichage de la PopUp qui contient le listing des pays/ville disponible à afficher
            popupLayout.Show();
        }

        // La méthode est appelée quand l'utilisateur appui sur un Pin dans l'onglet Map
        private async void Pin_MarkerClicked(object sender, PinClickedEventArgs e)
        {
            // Permet de fermer le carousel View si l'utilisateur clique sur un autre Pins dans l'onlget Maps
            if (CardImage.IsVisible)
            {
                CardImage.IsVisible = false;
                CardImageCounter.IsVisible = false;
            }

            // le Pin ouvre la description donc il y n'y a rien a faire d'autre
        }

        // La méthode est appelée quand l'utilisateur appui sur l'info window apres avoir cliqué sur le Pin
        private async void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
        {
            // on s'assure que le carousel View dans l'onglet Map est bien fermé
            CardImage.IsVisible = true;
            CardImageCounter.IsVisible = true;

            //IndicatorViewMap.IsVisible = true;
            // On recupere les infos de l'info Window cliqué par l'utilisateur
            var pin = sender as Pin;
            var PinAddress = pin.Label; // on recupere le commentaire

            var GitNamePicture = PinAddress;

            var httpClient = new HttpClient();

            LocationJSON = await httpClient.GetStringAsync("https://raw.githubusercontent.com/Damien-OLLIER/AppPictures/main/Places.json");

            var ob = JsonConvert.DeserializeObject<Root>(LocationJSON);

            var RespoJSON = MapsViewModel.RespoJSON;

            foreach (var result in ob.results)
            {
                if (GitNamePicture == result.name)
                {
                    var GitUrl = "";

                    foreach (var file in RespoJSON)
                    {
                        var Name = (string)file["name"];
                        if (Name == result.GitName) 
                        {
                            GitUrl = (string)file["git_url"];
                        }
                    }

                    string contentsJson1 = await httpClient.GetStringAsync(GitUrl);

                    JObject json = JObject.Parse(contentsJson1);

                    var sha = (string)json["sha"];
                    var tree = json["tree"];

                    var TestList = new List<string>
                    { };

                    foreach (var Tree in tree)
                    {
                        var Path = (string)Tree["path"];
                        TestList.Add("https://raw.githubusercontent.com/Damien-OLLIER/AppPictures/main/" + result.GitName + "/" + Path);
                    }
                    CardImage.ItemsSource = TestList;
                }
            }
        }

        // La méthode est appelée quand l'utilisateur appui sur la map (autre qu'un Pin)
        private void MyMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            // Cache le carousel View afin de pouvoir selectionner un autre voyage
            CardImage.IsVisible = false;
            CardImageCounter.IsVisible = false;

            // les petits boutons savoir le numero de l'image
            //CardImageCounter.IsVisible = false;
            //IndicatorViewMap.IsVisible = false;
        }

        // La méthode est appelée a chaque fois que l'onglet map est selectionné. Ceci dans le but de la cadré sur l'europe et d'afficher les Pins
        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            //Bouge la camera pour se placer en europe
            //To Modify: inutile car je la fou au etats unis avant, CF To DO:
            Position position = new Position(46.818188, 8.227512);//au milieu de l'europe
            MapSpan Startlocation = new MapSpan(position, 46.818188, 8.227512);
            MyMap.MoveToRegion(Startlocation);
        }

        // La méthode est appelée a chaque fois que les deux autres onglets sont sélectionnées
        private void ContentPage_Appearing_1(object sender, EventArgs e)
        {
            //Je sais que si on ne met pas ça, ça ne marche pas (rien ne s'affiche dans le caroussel)
            //To Do: à re tester
            this.BindingContext = this;
        }

        //Quand l'utilisateur appui sur l'engrenage, cela ouvre un menu déroulant où la selection de differents message est possible
        private void SfButton2_Clicked(object sender, EventArgs e)
        {
            //On rend visible, on active et on autorise l'utilisation du picker afin de pouvoir selectionner un message
            picker.IsOpen = true;
            picker.IsVisible = true;
            picker.IsEnabled = true;
            picker.ColumnHeaderText = "Choisi un message";

            // To DO: Rename la classe ColorInfo et ses methodes (ctrl+c/ctrl+v from internet lol)
            //Permet de recuperer la liste des phrases disponibles sous forme d'une "ObservableCollection<string>"
            ColorInfo info = new ColorInfo();
            // assigne cette liste au picker
            picker.ItemsSource = info.Colors;

        }

        //Quand l'utilisateur appui sur le coeur, cela ouvre un menu déroulant où la selection de differents numeros possible
        private void SfButton3_Clicked(object sender, EventArgs e)
        {
            //On rend visible, on active et on autorise l'utilisation du picker afin de pouvoir selectionner un message
            picker2.IsOpen = true;
            picker2.IsVisible = true;
            picker2.IsEnabled = true;
            //Permet de recuperer la liste des numeros disponibles sous forme d'une "ObservableCollection<string>"
            picker2.ColumnHeaderText = "Choisi un numéro de téléphone";
            Telephone info = new Telephone();
            // assigne cette liste au picker
            picker2.ItemsSource = info.Number;
        }

        // La méthode est appelée quand l'utilisateur appui sur le bouton OK de la page PopUp de selection de Message
        private void picker_OkButtonClicked(object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            //Obtention de l'objet selectionné
            var sfPicker = sender as SfPicker;

            //Obtention du message selectionné
            var SelectedItem = sfPicker.SelectedItem.ToString();

            //si l'utilisateur à sélectionné quelque chose
            if (SelectedItem != null)
            {
                if (SelectedItem != "")
                {
                    //On place en mémoire le bon Message selectionné
                    message = SelectedItem;

                    //On change le text dans le button par le text selectionné
                    SfButton.Text = SelectedItem;
                }
            }// sinon il ne se passe rien outre le fait que la fenetre PopUp se ferme
        }

        // La méthode est appelée quand l'utilisateur appui sur le bouton OK de la page PopUp de selection de numéro
        private void picker2_OkButtonClicked(object sender, Syncfusion.SfPicker.XForms.SelectionChangedEventArgs e)
        {
            //Obtention de l'objet selectionné
            var sfPicker = sender as SfPicker;

            //Obtention du numéro selectionné
            var SelectedItem = sfPicker.SelectedItem.ToString();

            //si l'utilisateur à sélectionné quelque chose
            if (SelectedItem != null)
            {
                if (SelectedItem != "")
                {
                    //On place en mémoire le bon numero selectionné
                    if (SelectedItem == "Damien") 
                    {
                        numero = "+33632183163";
                    }
                    else 
                    {
                        numero = "+33695790868";
                    }
                }
            }// sinon il ne se passe rien outre le fait que la fenetre PopUp se ferme
        }

        //To Modify: Boutton pas important car invisible donc potentiellement à supprimer.Il devait me servir à la base pour les tests afin d'ouvrir la PopUp
        private void isOpenButton_Clicked(object sender, EventArgs e)
        {
        }

        // La méthode est appelée quand la fenetre PopUp se ferme apres avoir choisi une nouvelle destination (dans le premier onglet apres le double tap)
        private void popupLayout_Closed(object sender, EventArgs e)
        {
            //Je sais que si on ne met pas ça, ça ne marche pas (rien ne s'affiche dans le caroussel)
            //To Do: à re tester
            this.BindingContext = this;
        }

        // La méthode est appelée apres que l'utilisateur ai choisi un nouveau voyage à afficher dans la PopUp (quand elle se ferme)
        private async void listView_SelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            //On recupere l'info sur la destination choisi
            var SelectedItem = e.AddedItems;
            var SelectedItem0 = SelectedItem[0];

            //On converti l'objet reçu en Maps avec ses attributs
            var maps = SelectedItem0 as Maps;

            var GitNamePicture = maps.gitname;

            var RespoJSON = MapsViewModel.RespoJSON;

            var httpClient = new HttpClient();

            foreach (var file in RespoJSON)
            {
                var fileName = (string)file["name"];
                if (GitNamePicture == fileName)
                {
                    var GitUrl = (string)file["git_url"];

                    string contentsJson1 = await httpClient.GetStringAsync(GitUrl);

                    JObject json = JObject.Parse(contentsJson1);

                    var tree = json["tree"];

                    var TestList = new List<string>
                    { };

                    foreach (var Tree in tree)
                    {
                        var Path = (string)Tree["path"];
                        TestList.Add("https://raw.githubusercontent.com/Damien-OLLIER/AppPictures/main/" + fileName + "/" + Path);
                    }

                    // LabelDescription.Text = "Description : " + Description;
                    Carousel.ItemsSource = TestList;
                    popupLayout.IsOpen = false;

                    LabelDescription.Text = "Description : " + maps.description;
                }
                else
                {

                }
            }
        }

        // La méthode est appelée lorsque l'app apparait à l'écran
        private async void TabbedPage_Appearing(object sender, EventArgs e)
        {
            // genère un nombre aléatoire afin d'afficher une destination au hasard lors de la premiere ouverture de l'app (ou apres l'avoir fermé complétement)
            Random rnd = new Random();
                        
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("MyApplication", "1"));
            var repo = "Damien-OLLIER/AppPictures";
            var contentsUrl = $"https://api.github.com/repos/{repo}/contents";

            try 
            {
                var contentsJson = await httpClient.GetStringAsync(contentsUrl);
                var contents = (JArray)JsonConvert.DeserializeObject(contentsJson);

                // On creer une intsance de "MapsViewModel" afin d'avoir acces a la liste des pays possible
                this.BindingContext = new MapsViewModel(contents);
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                Debug.WriteLine(ex.ToString());
                await DisplayAlert("Internet Error", "You have made too many HTTP requests. Please wait a few hours before using the app again.", "OK");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                await DisplayAlert("Internet Error", "Please restart the App with Internet", "OK");
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }


            var RespoJSON = MapsViewModel.RespoJSON;
     
            var ListName = new List<string>();

            foreach (var file in RespoJSON)
            {
                var fileName = (string)file["name"];

                Debug.WriteLine(fileName);

                if(fileName.Contains(".vs") || fileName.Contains("Video") || fileName.Contains("Places.json")) 
                { 
                }
                else
                {
                    ListName.Add(fileName);
                }
            }
                        
            int RandNumber = rnd.Next(0, ListName.Count);
            var GitFolder = RespoJSON[RandNumber];
            var GitUrl = (string)GitFolder["git_url"];
            var GitName = (string)GitFolder["name"];

            string contentsJson1 = await httpClient.GetStringAsync(GitUrl);

            JObject json = JObject.Parse(contentsJson1);

            var tree = json["tree"];

            var TestList = new List<string>
            { };

            foreach (var Tree in tree)
            {
                var Path = (string)Tree["path"];
                TestList.Add("https://raw.githubusercontent.com/Damien-OLLIER/AppPictures/main/" + GitName + "/" + Path);
            }

            // LabelDescription.Text = "Description : " + Description;
            Carousel.ItemsSource = TestList;
            popupLayout.IsOpen = false;

            LocationJSON = await httpClient.GetStringAsync("https://raw.githubusercontent.com/Damien-OLLIER/AppPictures/main/Places.json");

            var ob = JsonConvert.DeserializeObject<Root>(LocationJSON);

            //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);
            foreach (var result in ob.results)
            {
                if(GitName == result.GitName)
                {
                    LabelDescription.Text = "Description : " + result.vicinity;
                }
            }

            //LabelDescription.Text = "Description : " + maps.description;
        }

        // To do: a supprimer ? potentiellement pas utilisé
        public class CarouselModel
        {
            public CarouselModel(string imageString)
            {
                Image = imageString;
            }
            private ImageSource _image;
            public ImageSource Image
            {
                get { return _image; }
                set { _image = value; }
            }
        }


        //Class Family qui permet de peupler le second onglet family
        public class Family
        {
            public string Name { get; set; } // nom tel que Damien, Camille, etc...
            public bool IsExpanded { get; set; } // est ce que l'expander est déplié au depart
            public ObservableCollection<FamilyMember> familyMember { get; set; }
            public string Color { get; set; } // couleur du trait de la box de gauche
            public string Icon { get; set; } // icon du family NuMber
        }

        public class Image
        {
            public string Name { get; set; }
        }

        // to do: à supprimer ? Pas sur ?
        public class ListViewGalleryInfo : INotifyPropertyChanged
        {
            #region Fields

            private ImageSource gridLayout;

            #endregion

            #region Constructor

            public ListViewGalleryInfo()
            {

            }

            #endregion

            #region Properties

            public ImageSource GridLayout
            {
                get { return gridLayout; }
                set
                {
                    gridLayout = value;
                    OnPropertyChanged("GridLayout");
                }
            }

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

        public class FamilyMember //Anciennement Speaker
        {
            //
            public string BirthDate { get; set; }
            public string Picture { get; set; }
            public string Description { get; set; }
            public string PositifPoint { get; set; }
            public string NegatifPoint { get; set; }
        }

        public class ColorInfo
        {
            private ObservableCollection<string> _color;

            public ObservableCollection<string> Colors

            {

                get { return _color; }

                set { _color = value; }

            }

            public ColorInfo()
            {
                Colors = new ObservableCollection<string>();

                Colors.Add("Je t'aime");
                Colors.Add("Tu me manques");
                Colors.Add("Roulades !");
                Colors.Add("Tipi !");
                Colors.Add("Damien !");
                Colors.Add("Arrete ou je te pete dessus");
                Colors.Add("J'ai hate de rentrer sur Lyon");
                Colors.Add("Oui je veux aussi un boubou");
                Colors.Add("T'es sur que tu veux pas de baleine");
                Colors.Add("T'es le meilleure je t'aime de tout mon coeur !");
            }
        }

        public class Telephone
        {
            private ObservableCollection<string> _number;

            public ObservableCollection<string> Number
            {
                get { return _number; }
                set { _number = value; }
            }

            public Telephone()
            {
                Number = new ObservableCollection<string>();
                Number.Add("Damien"); //+33632183163
                Number.Add("Camille"); //+33695790868
            }
        }


        private void Carousel_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            /*var test = sender as PositionChangedEventArgs;
            var test1 = sender as CarouselView;
            var testt = test1.ItemsSource;
            var bbb = testt.GetEnumerator();
            var Caarousel = Carousel.ItemsSource; */

            LabelIndicatorView.Text = (e.CurrentPosition + 1).ToString() + "/" + (NumberOfItems - 1).ToString();
        }

        private void CardImage_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            var Items = sender as Xamarin.Forms.ItemsView;
            var Current_Position = sender as Xamarin.Forms.CarouselView;

            var ItemList = Items.ItemsSource;

            var i = 0;

            foreach (var item in ItemList)
            {
                i += 1;
            }

            var CurrentPosition = (Current_Position?.Position + 1).ToString();

            CardImageCounter.Text = CurrentPosition + "/" + (i).ToString();

            //CardImageCounter.Text = (e.CurrentPosition + 1).ToString() + "/" + (NumberOfItemsMaps - 1).ToString();
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("MyApplication", "1"));
             var repo = "Damien-OLLIER/AppPictures";
            var contentsUrl = $"https://api.github.com/repos/{repo}/contents";
            var contentsJson = await httpClient.GetStringAsync(contentsUrl);
            var contents = (JArray)JsonConvert.DeserializeObject(contentsJson);

            Debug.WriteLine("Biginning");

            List<string> JSONList = new List<string>();

            try
            {

                foreach (var file in contents)
                {
                    var fileType = (string)file["type"];
                    if (fileType == "dir")
                    {
                        var directoryContentsUrl = (string)file["url"];
                        // use this URL to list the contents of the folder
                        Debug.WriteLine($"DIR: {directoryContentsUrl}");


                        if (directoryContentsUrl.Contains(".vs?ref=main"))
                        {

                        }
                        else
                        {
                            var contentsJson1 = await httpClient.GetStringAsync(directoryContentsUrl);

                            JSONList.Add(contentsJson1);                            
                        }
                    }
                    else if (fileType == "file")
                    {
                        var downloadUrl = (string)file["download_url"];
                        Debug.WriteLine($"DOWNLOAD: {downloadUrl}");

                        using (WebClient wc = new WebClient())
                        {
                            var b = wc.Encoding;

                            var json = wc.DownloadString(downloadUrl);

                            var ob = JsonConvert.DeserializeObject<Root>(json);

                            //Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(json);
                            foreach (var result in ob.results)
                            {
                                Debug.WriteLine(result.name);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                
            }

            Debug.WriteLine("Done");
        }

        public class Geometry
        {
            public Location location { get; set; }
        }

        public class Location
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Result
        {
            public Geometry geometry { get; set; }
            public string name { get; set; }
            public string vicinity { get; set; }
            public string GitName { get; set; }
        }

        public class Root
        {
            public List<object> html_attributions { get; set; }
            public string next_page_token { get; set; }
            public List<Result> results { get; set; }
            public string status { get; set; }
        }

        public class Links
        {
            public string self { get; set; }
            public string git { get; set; }
            public string html { get; set; }
        }
        public class RootPictures
        {
            public string name { get; set; }
            public string path { get; set; }
            public string sha { get; set; }
            public int size { get; set; }
            public string url { get; set; }
            public string html_url { get; set; }
            public string git_url { get; set; }
            public string download_url { get; set; }
            public string type { get; set; }
            public Links _links { get; set; }
        }
        private void Carousel_CurrentItemChanged_1(object sender, CurrentItemChangedEventArgs e)
        {
            var Items = sender as Xamarin.Forms.ItemsView;
            var Current_Position = sender as Xamarin.Forms.CarouselView;

            var ItemList = Items.ItemsSource;

            var i = 0;

            foreach( var item in ItemList)
            {
                i += 1;
            }

            var CurrentPosition = (Current_Position?.Position + 1).ToString();

            LabelIndicatorView.Text = CurrentPosition + "/" + i.ToString();            
        }

        private void ContentPage_Appearing_2(object sender, EventArgs e)
        {

        }


        private void ContentPage_Appearing_3(object sender, EventArgs e)
        {

        }

        //private async void PlayStop_Clicked(object sender, EventArgs e)
        //{

        //    if (PlayPauseButton.Text == "Play")
        //    {
        //        await CrossMediaManager.Current.Play(VideoEntry.Text);

        //        PlayPauseButton.Text = "Stop";
        //    }

        //    else if (PlayPauseButton.Text == "Stop")
        //    {
        //        await CrossMediaManager.Current.Stop();

        //        PlayPauseButton.Text = "Play";
        //    }
        //}

        private void PlayStopButton(object sender, EventArgs e)
        {
            if (PlayStopButtonText.Text == "Play")
            {
                CrossMediaManager.Current.PlayPause();

                PlayStopButtonText.Text = "Stop";
            }
            else if (PlayStopButtonText.Text == "Stop")
            {
                CrossMediaManager.Current.Pause();

                PlayStopButtonText.Text = "Play";
            }
            else if (PlayStopButtonText.Text == "Start")
            {
                CrossMediaManager.Current.Play(videoUrl);

                PlayStopButtonText.Text = "Stop";
            }
        }

        private async void TestButton_Clicked(object sender, EventArgs e)
        {
            var httpClient = new HttpClient();

             var GitFolder = await httpClient.GetStringAsync("https://api.github.com/repos/Damien-OLLIER/AppPictures/contents");
           
            var contents = (JArray)JsonConvert.DeserializeObject(GitFolder);

            var git_url = "";

            foreach (var file in contents)
            {
                var filetype = (string)file["type"];

                if(filetype == "dir") 
                {
                    if( (string)file["name"] == "Video") 
                    {
                        git_url = (string)file["git_url"];
                    }
                }
            }
            
            var VideoFolder = "";

            VideoFolder = await httpClient.GetStringAsync(git_url);

            var ob = JsonConvert.DeserializeObject<VideoFolderObject>(VideoFolder);

            var TreeObject = ob.tree;

            List<string> VideoNameList = new List<string>();

            foreach (var VideoFile in ob.tree)
            {
                VideoNameList.Add(VideoFile.path);
            }

            Random rnd = new Random();

            int RandNumber = rnd.Next(0, VideoNameList.Count);

            //videoUrl = "https://raw.githubusercontent.com/Damien-OLLIER/AppPictures/main/Video/" + VideoNameList[RandNumber];
            Videoview.Source = "https://raw.githubusercontent.com/Damien-OLLIER/AppPictures/main/Video/" + VideoNameList[RandNumber];
            EntryVideoName.Text = Videoview.Source.ToString();

            await CrossMediaManager.Current.Play();

        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class VideoFolderObject
        {
            public string sha { get; set; }
            public string url { get; set; }
            public List<Tree> tree { get; set; }
            public bool truncated { get; set; }
        }

        public class Tree
        {
            public string path { get; set; }
            public string mode { get; set; }
            public string type { get; set; }
            public string sha { get; set; }
            public int size { get; set; }
            public string url { get; set; }
        }
    }
}
