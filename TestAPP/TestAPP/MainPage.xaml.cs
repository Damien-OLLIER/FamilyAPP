using AndroidX.Lifecycle;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.Media;
using Plugin.Messaging;
using Syncfusion.ListView.XForms;
using Syncfusion.SfPicker.XForms;
using Syncfusion.XForms.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

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
        public int NumberOfItemsMaps { get; private set; }

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
        private void OnTapGestureRecognizerTapped(object sender, EventArgs e)
        {
            //Ligne qui permet d'afficher la liste déroulante contenue dans la PopUp. La liste doit contenir tout les pays/ville disponible à afficher
            this.BindingContext = new MapsViewModel();

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
            var PinAddress = pin.Address; // on recupere le commentaire

            // Suivant le commentaire, on affiche les images en relations avec le Pin cliqué
            //To modify: si tu veux ameliorer la suite de if/Else if je suis preneur
            if (PinAddress.Contains("louloute à Innsbruck"))
            {
                //On instancie la liste "ObservableCollection" contenant des objets de la classe "Image"
                var Innsbruck = new ObservableCollection<Image>();

                for (int i = 1; i < 47; i++)
                {
                    // On ajoute les images en rapport avec la destination choisi
                    Innsbruck.Add(new Image { Name = "Autriche" + i + ".JPG" });
                    //on return donc une liste de "Name"(string) contenant enfaite le nom de l'image 
                }

                //On donne cette liste "ObservableCollection" contenant des objets de la classe "Image" comme item source au carousel view.
                CardImage.ItemsSource = Innsbruck;

                NumberOfItemsMaps = 47;
                //Le carousel va donc utiliser cette source pour "Populer"/remplir (To populate) son template qui est enfaite une Image donc la source est "Name"
            }
            else if (PinAddress.Contains("Venise"))
            {
                var ItalieCard = new ObservableCollection<Image>();

                for (int i = 1; i < 88; i++)
                {
                    ItalieCard.Add(new Image { Name = "Italie" + i + ".JPG" });
                }

                CardImage.ItemsSource = ItalieCard;

                NumberOfItemsMaps = 88;
            }
            else if (PinAddress.Contains("de la loire à vélo"))
            {
                var Loire = new ObservableCollection<Image>();

                for (int i = 1; i < 10; i++)
                {
                    Loire.Add(new Image { Name = "Loire" + i + ".JPG" });
                }

                CardImage.ItemsSource = Loire;

                NumberOfItemsMaps = 10;
            }
            else if (PinAddress.Contains("à Zurich en octobre 2021"))
            {
                var Zurich = new ObservableCollection<Image>();

                for (int i = 1; i < 58; i++)
                {
                    Zurich.Add(new Image { Name = "Zurich" + i + ".JPG" });
                }

                CardImage.ItemsSource = Zurich;

                NumberOfItemsMaps = 58;
            }
            else if (PinAddress.Contains("Visite des chutes du Rhin"))
            {
                var Rhin = new ObservableCollection<Image>();

                for (int i = 1; i < 58; i++)
                {
                    Rhin.Add(new Image { Name = "Rhin" + i + ".JPG" });
                }

                CardImage.ItemsSource = Rhin;

                NumberOfItemsMaps = 58;
            }
            else if (PinAddress.Contains("Voyage à Lucerne"))
            {
                var Lucerne = new ObservableCollection<Image>();

                for (int i = 1; i < 151; i++)
                {
                    Lucerne.Add(new Image { Name = "Lucerne" + i + ".JPG" });
                }

                CardImage.ItemsSource = Lucerne;

                NumberOfItemsMaps = 151;
            }
            else if (PinAddress.Contains("Visite de Thoune"))
            {
                var Thoune = new ObservableCollection<Image>();

                for (int i = 1; i < 126; i++)
                {
                    Thoune.Add(new Image { Name = "Thoune" + i + ".JPG" });
                }

                CardImage.ItemsSource = Thoune;

                NumberOfItemsMaps = 126;
            }
            else if (PinAddress.Contains("Ski de fond à Chichilianne"))
            {
                var SkiDeFond = new ObservableCollection<Image>();

                for (int i = 1; i < 20; i++)
                {
                    SkiDeFond.Add(new Image { Name = "SkiDeFond" + i + ".JPG" });
                }

                CardImage.ItemsSource = SkiDeFond;

                NumberOfItemsMaps = 20;
            }
            else if (PinAddress.Contains("Voyage à Amsterdam"))
            {
                var PaysBas = new ObservableCollection<Image>();

                for (int i = 1; i < 125; i++)
                {
                    PaysBas.Add(new Image { Name = "PaysBas" + i + ".JPG" });
                }

                CardImage.ItemsSource = PaysBas;

                NumberOfItemsMaps = 125;
            }
            else if (PinAddress.Contains("Nouvel an à Morzine"))
            {
                var MorzineNouvelAn = new ObservableCollection<Image>();

                for (int i = 1; i < 29; i++)
                {
                    MorzineNouvelAn.Add(new Image { Name = "MorzineNouvelAn" + i + ".JPG" });
                }

                CardImage.ItemsSource = MorzineNouvelAn;

                NumberOfItemsMaps = 29;
            }
            else if (PinAddress.Contains("Morzine avec les amis"))
            {
                var MorzineJuin = new ObservableCollection<Image>();

                for (int i = 1; i < 18; i++)
                {
                    MorzineJuin.Add(new Image { Name = "MorzineJuin" + i + ".JPG" });
                }

                CardImage.ItemsSource = MorzineJuin;

                NumberOfItemsMaps = 18;
            }
            else if (PinAddress.Contains("Lieu de villégiature"))
            {
                var Confinement = new ObservableCollection<Image>();

                for (int i = 1; i < 78; i++)
                {
                    Confinement.Add(new Image { Name = "Confinement" + i + ".JPG" });
                }

                CardImage.ItemsSource = Confinement;

                NumberOfItemsMaps = 78;
            }
            else if (PinAddress.Contains("Photos relatant notre année 2022 ensemble"))
            {
                var Photo2022 = new ObservableCollection<Image>();

                for (int i = 1; i < 51; i++)
                {
                    Photo2022.Add(new Image { Name = "Photo2022" + i + ".JPG" });
                }

                CardImage.ItemsSource = Photo2022;

                NumberOfItemsMaps = 51;
            }
            else if (PinAddress.Contains("Photos relatant notre année 2021 ensemble"))
            {
                var Photo2021 = new ObservableCollection<Image>();

                for (int i = 1; i < 223; i++)
                {
                    Photo2021.Add(new Image { Name = "Photo2021" + i + ".JPG" });
                }

                CardImage.ItemsSource = Photo2021;

                NumberOfItemsMaps = 223;
            }
            else if (PinAddress.Contains("Photos relatant notre année 2020 ensemble"))
            {
                var Photo2020 = new ObservableCollection<Image>();

                for (int i = 1; i < 79; i++)
                {
                    Photo2020.Add(new Image { Name = "Photo2020" + i + ".JPG" });
                }

                CardImage.ItemsSource = Photo2020;

                NumberOfItemsMaps = 79;
            }

            CardImageCounter.Text = 1 + "/" + (NumberOfItemsMaps - 1).ToString();
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
                    numero = SelectedItem;
                }
            }// sinon il ne se passe rien outre le fait que la fenetre PopUp se ferme
        }

        //To Modify: Boutton pas important car invisible donc potentiellement à supprimer.Il devait me servir à la base pour les tests afin d'ouvrir la PopUp
        private void isOpenButton_Clicked(object sender, EventArgs e)
        {
            popupLayout.Show();
            this.BindingContext = new MapsViewModel();
        }

        // La méthode est appelée quand la fenetre PopUp se ferme apres avoir choisi une nouvelle destination (dans le premier onglet apres le double tap)
        private void popupLayout_Closed(object sender, EventArgs e)
        {
            //Je sais que si on ne met pas ça, ça ne marche pas (rien ne s'affiche dans le caroussel)
            //To Do: à re tester
            this.BindingContext = this;
        }

        // La méthode est appelée apres que l'utilisateur ai choisi un nouveau voyage à afficher dans la PopUp (quand elle se ferme)
        private void listView_SelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            //On recupere l'info sur la destination choisi
            var SelectedItem = e.AddedItems;
            var SelectedItem0 = SelectedItem[0];

            //On converti l'objet reçu en Maps avec ses attributs
            var maps = SelectedItem0 as Maps;

            //On cherche à recuprer sa description et la Destination
            var Lieu = maps.countryList;
            var Description = maps.description;

            // Suivant le Lieu, on affiche les images en relations avec le Pin cliqué
            //To modify: si tu veux ameliorer la suite de if/Else if je suis preneur
            if (Lieu == "Venise")
            {
                //On instancie la liste "ObservableCollection" contenant des objets de la classe "Image"
                var ItalyPicture = new ObservableCollection<Image>();

                for (int i = 1; i < 88; i++)
                {
                    // On ajoute les images en rapport avec la destination choisi
                    ItalyPicture.Add(new Image { Name = "Italie" + i + ".JPG" });
                    //on return donc une liste de "Name"(string) contenant enfaite le nom de l'image
                }

                //On donne cette liste "ObservableCollection" contenant des objets de la classe "Image" comme item source au carousel view.
                Carousel.ItemsSource = ItalyPicture;

                //Le carousel va donc utiliser cette source pour "Populer"/remplir (To populate) son template qui est enfaite une Image donc la source est "Name"

                //Close the PopUp Layout
                popupLayout.IsOpen = false;

                //Mise à jour de la description de voyage en dessous du carousel view du premier onglet
                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 88;
            }
            else if (Lieu == "Autriche")
            {
                var AutrichePicture = new ObservableCollection<Image>();

                for (int i = 1; i < 47; i++)
                {
                    AutrichePicture.Add(new Image { Name = "Autriche" + i + ".JPG" });
                }

                Carousel.ItemsSource = AutrichePicture;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 47;
            }
            else if (Lieu == "Loire à vélo")
            {
                var Loire = new ObservableCollection<Image>();

                for (int i = 1; i < 10; i++)
                {
                    Loire.Add(new Image { Name = "Loire" + i + ".JPG" });
                }

                Carousel.ItemsSource = Loire;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 10;
            }
            else if (Lieu == "Zurich")
            {
                var Zurich = new ObservableCollection<Image>();

                for (int i = 1; i < 58; i++)
                {
                    Zurich.Add(new Image { Name = "Zurich" + i + ".JPG" });
                }

                Carousel.ItemsSource = Zurich;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 58;
            }
            else if (Lieu == "Chutes du Rhin")
            {
                var Rhin = new ObservableCollection<Image>();

                for (int i = 1; i < 58; i++)
                {
                    Rhin.Add(new Image { Name = "Rhin" + i + ".JPG" });
                }

                Carousel.ItemsSource = Rhin;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 58;
            }
            else if (Lieu == "Lucerne")
            {
                var Lucerne = new ObservableCollection<Image>();

                for (int i = 1; i < 151; i++)
                {
                    Lucerne.Add(new Image { Name = "Lucerne" + i + ".JPG" });
                }

                Carousel.ItemsSource = Lucerne;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 151;
            }
            else if (Lieu == "Thoune")
            {
                var Thoune = new ObservableCollection<Image>();

                for (int i = 1; i < 126; i++)
                {
                    Thoune.Add(new Image { Name = "Thoune" + i + ".JPG" });
                }

                Carousel.ItemsSource = Thoune;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 126;
            }
            else if (Lieu == "Chichilianne")
            {
                var SkiDeFond = new ObservableCollection<Image>();

                for (int i = 1; i < 20; i++)
                {
                    SkiDeFond.Add(new Image { Name = "SkiDeFond" + i + ".JPG" });
                }

                Carousel.ItemsSource = SkiDeFond;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 20;
            }
            else if (Lieu == "Pays-Bas")
            {
                var PaysBas = new ObservableCollection<Image>();

                for (int i = 1; i < 125; i++)
                {
                    PaysBas.Add(new Image { Name = "PaysBas" + i + ".JPG" });
                }

                Carousel.ItemsSource = PaysBas;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 125;
            }
            else if (Lieu == "Nouvel an à Morzine")
            {
                var MorzineNouvelAn = new ObservableCollection<Image>();

                for (int i = 1; i < 29; i++)
                {
                    MorzineNouvelAn.Add(new Image { Name = "MorzineNouvelAn" + i + ".JPG" });
                }

                Carousel.ItemsSource = MorzineNouvelAn;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 29;
            }
            else if (Lieu == "Morzine")
            {
                var MorzineJuin = new ObservableCollection<Image>();

                for (int i = 1; i < 18; i++)
                {
                    MorzineJuin.Add(new Image { Name = "MorzineJuin" + i + ".JPG" });
                }

                Carousel.ItemsSource = MorzineJuin;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 18;
            }
            else if (Lieu == "Appartement des loulous")
            {
                var Confinement = new ObservableCollection<Image>();

                for (int i = 1; i < 78; i++)
                {
                    Confinement.Add(new Image { Name = "Confinement" + i + ".JPG" });
                }

                Carousel.ItemsSource = Confinement;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 78;
            }
            else if (Lieu == "Année 2022")
            {
                var Photo2022 = new ObservableCollection<Image>();

                for (int i = 1; i < 51; i++)
                {
                    Photo2022.Add(new Image { Name = "Photo2022" + i + ".JPG" });
                }

                Carousel.ItemsSource = Photo2022;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 51;
            }
            else if (Lieu == "Année 2021")
            {
                var Photo2021 = new ObservableCollection<Image>();

                for (int i = 1; i < 223; i++)
                {
                    Photo2021.Add(new Image { Name = "Photo2021" + i + ".JPG" });
                }

                Carousel.ItemsSource = Photo2021;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 223;
            }
            else if (Lieu == "Année 2022")
            {
                var Photo2020 = new ObservableCollection<Image>();

                for (int i = 1; i < 79; i++)
                {
                    Photo2020.Add(new Image { Name = "Photo2020" + i + ".JPG" });
                }

                Carousel.ItemsSource = Photo2020;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 79;
            }

            LabelIndicatorView.Text = 1 + "/" + (NumberOfItems - 1).ToString();

        }

        // La méthode est appelée lorsque l'app apparait à l'écran
        private void TabbedPage_Appearing(object sender, EventArgs e)
        {
            // genère un nombre aléatoire afin d'afficher une destination au hasard lors de la premiere ouverture de l'app (ou apres l'avoir fermé complétement)
            Random rnd = new Random();

            int RandNumber = rnd.Next(1, 16); // creates a number between 1 and 12 //To modify: je ne crois pas non

            // On creer une intsance de "MapsViewModel" afin d'avoir acces a la liste des pays possible
            var mapsViewModel = new MapsViewModel();

            // On récupère la description
            var Description = mapsViewModel.Items[RandNumber - 1].description;

            // On récupère la destination
            var Destination = mapsViewModel.Items[RandNumber - 1].countryList;

            //cf "listView_SelectionChanged"
            if (Destination == "Venise")
            {
                var ItalyPicture = new ObservableCollection<Image>();

                for (int i = 1; i < 88; i++)
                {
                    ItalyPicture.Add(new Image { Name = "Italie" + i + ".JPG" });
                }

                Carousel.ItemsSource = ItalyPicture;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 88;
            }
            else if (Destination == "Innsbruck")
            {
                var AutrichePicture = new ObservableCollection<Image>();

                for (int i = 1; i < 47; i++)
                {
                    AutrichePicture.Add(new Image { Name = "Autriche" + i + ".JPG" });
                }

                Carousel.ItemsSource = AutrichePicture;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 47;
            }
            else if (Destination == "Loire à vélo")
            {
                var Loire = new ObservableCollection<Image>();

                for (int i = 1; i < 10; i++)
                {
                    Loire.Add(new Image { Name = "Loire" + i + ".JPG" });
                }

                Carousel.ItemsSource = Loire;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 10;
            }
            else if (Destination == "Zurich")
            {
                var Zurich = new ObservableCollection<Image>();

                for (int i = 1; i < 58; i++)
                {
                    Zurich.Add(new Image { Name = "Zurich" + i + ".JPG" });
                }

                Carousel.ItemsSource = Zurich;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 58;
            }
            else if (Destination == "Chutes du Rhin")
            {
                var Rhin = new ObservableCollection<Image>();

                for (int i = 1; i < 58; i++)
                {
                    Rhin.Add(new Image { Name = "Rhin" + i + ".JPG" });
                }

                Carousel.ItemsSource = Rhin;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 58;
            }
            else if (Destination == "Lucerne")
            {
                var Lucerne = new ObservableCollection<Image>();

                for (int i = 1; i < 151; i++)
                {
                    Lucerne.Add(new Image { Name = "Lucerne" + i + ".JPG" });
                }

                Carousel.ItemsSource = Lucerne;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 151;
            }
            else if (Destination == "Thoune")
            {
                var Thoune = new ObservableCollection<Image>();

                for (int i = 1; i < 126; i++)
                {
                    Thoune.Add(new Image { Name = "Thoune" + i + ".JPG" });
                }

                Carousel.ItemsSource = Thoune;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 126;
            }
            else if (Destination == "Chichilianne")
            {
                var SkiDeFond = new ObservableCollection<Image>();

                for (int i = 1; i < 20; i++)
                {
                    SkiDeFond.Add(new Image { Name = "SkiDeFond" + i + ".JPG" });
                }

                Carousel.ItemsSource = SkiDeFond;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 20;
            }
            else if (Destination == "Pays-Bas")
            {
                var PaysBas = new ObservableCollection<Image>();

                for (int i = 1; i < 125; i++)
                {
                    PaysBas.Add(new Image { Name = "PaysBas" + i + ".JPG" });
                }

                Carousel.ItemsSource = PaysBas;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 125;
            }
            else if (Destination == "Nouvel an à Morzine")
            {
                var MorzineNouvelAn = new ObservableCollection<Image>();

                for (int i = 1; i < 29; i++)
                {
                    MorzineNouvelAn.Add(new Image { Name = "MorzineNouvelAn" + i + ".JPG" });
                }

                Carousel.ItemsSource = MorzineNouvelAn;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 29;
            }
            else if (Destination == "Morzine")
            {
                var MorzineJuin = new ObservableCollection<Image>();

                for (int i = 1; i < 18; i++)
                {
                    MorzineJuin.Add(new Image { Name = "MorzineJuin" + i + ".JPG" });
                }

                Carousel.ItemsSource = MorzineJuin;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 18;
            }
            else if (Destination == "Appartement des loulous")
            {
                var Confinement = new ObservableCollection<Image>();

                for (int i = 1; i < 78; i++)
                {
                    Confinement.Add(new Image { Name = "Confinement" + i + ".JPG" });
                }

                Carousel.ItemsSource = Confinement;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 78;
            }
            else if (Destination == "Année 2022")
            {
                var Photo2022 = new ObservableCollection<Image>();

                for (int i = 1; i < 51; i++)
                {
                    Photo2022.Add(new Image { Name = "Photo2022" + i + ".JPG" });
                }

                Carousel.ItemsSource = Photo2022;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 51;
            }
            else if (Destination == "Année 2021")
            {
                var Photo2021 = new ObservableCollection<Image>();

                for (int i = 1; i < 223; i++)
                {
                    Photo2021.Add(new Image { Name = "Photo2021" + i + ".JPG" });
                }

                Carousel.ItemsSource = Photo2021;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 223;
            }
            else if (Destination == "Année 2020")
            {
                var Photo2020 = new ObservableCollection<Image>();

                for (int i = 1; i < 79; i++)
                {
                    Photo2020.Add(new Image { Name = "Photo2020" + i + ".JPG" });
                }

                Carousel.ItemsSource = Photo2020;

                popupLayout.IsOpen = false;

                LabelDescription.Text = "Description : " + Description;

                NumberOfItems = 79;
            }


            LabelIndicatorView.Text = 1 + "/" + (NumberOfItems - 1).ToString();

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
                Colors.Add("Arrete ou je te pete dessus");
                Colors.Add("J'ai hate de rentrer sur Lyon");
                Colors.Add("Oui je veux aussi un boubou");
                Colors.Add("Oui je veux aussi deux boubou");
                Colors.Add("Oui je veux aussi trois boubou");
                Colors.Add("Tu veux combien d'animaux ?");
                Colors.Add("T'es sur que tu veux pas de baleine");
                Colors.Add("Ton application est trop bien t'es le meilleure je t'aime de tout mon coeur !");
                Colors.Add("J'ai trouvé la première surprise, hate de la partager avec toi !");
                Colors.Add("Je suis prête et j'adore la deuxième surprise !");
                Colors.Add("J'arrive !");
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
                Number.Add("+33632183163");
                Number.Add("+33695790868");
            }
        }

        private void Carousel_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
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
            CardImageCounter.Text = (e.CurrentPosition + 1).ToString() + "/" + (NumberOfItemsMaps - 1).ToString();
        }

        private void ContentPage_Appearing_2(object sender, EventArgs e)
        {
            //var PictireListe = new List<string>
            //{
            //    "https://raw.githubusercontent.com/Damien-OLLIER/TestAPPgit/NewFeatures/TestAPP/TestAPP.Android/Resources/drawable/Autriche1.JPG",
            //    "https://raw.githubusercontent.com/Damien-OLLIER/TestAPPgit/NewFeatures/TestAPP/TestAPP.Android/Resources/drawable/Autriche2.JPG",
            //    "https://raw.githubusercontent.com/Damien-OLLIER/TestAPPgit/NewFeatures/TestAPP/TestAPP.Android/Resources/drawable/Autriche3.JPG",

            //};

            //TheCarousel.ItemsSource = PictireListe;

            var TestList = new List<string>
            { };

            for (int i = 1; i < 48; i++)
            {
                TestList.Add("https://raw.githubusercontent.com/Damien-OLLIER/TestAPPgit/NewFeatures/TestAPP/TestAPP.Android/Resources/drawable/Autriche/Autriche" + i + ".JPG");
                Debug.WriteLine(TestList[i - 1]);
            }

            TheCarousel.ItemsSource = TestList;
        }

        private void TheCarousel_CurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
        {
            Indicator.Text = indicatorview.Position.ToString();
        }

        private void OnTapGestureRecognizerTappedTest(object sender, EventArgs e)
        {
            popupLayoutTest.Show();
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

            foreach (var file in contents)
            {
                var fileType = (string)file["type"];
                if (fileType == "dir")
                {
                    var directoryContentsUrl = (string)file["url"];
                    // use this URL to list the contents of the folder
                    Debug.WriteLine($"DIR: {directoryContentsUrl}");
                }
                else if (fileType == "file")
                {
                    var downloadUrl = (string)file["download_url"];
                    // use this URL to download the contents of the file
                    Debug.WriteLine($"DOWNLOAD: {downloadUrl}");
                }
            }

            Debug.WriteLine("Done");
        }
    }
}

