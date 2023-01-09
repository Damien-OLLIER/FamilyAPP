using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace TestAPP
{
    public class Place
    {
        public string PlaceName { get; set; }
        public string Address { get; set; }
        public string Gitname { get; set; }
        public Position Position { get; set; }
        public Location Location { get; set; }
    }

    public class Places
    {
        public Result[] results { get; set; }
    }

    public class Result
    {
        public Geometry geometry { get; set; }
        public string name { get; set; }      
        public string vicinity { get; set; }
        public string Gitname { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class Location
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }
}
