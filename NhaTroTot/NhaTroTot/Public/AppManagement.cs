using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using NhaTroTot.Usercontrols;
using System.Collections.ObjectModel;
using NhaTroTot.Models;
using System.Text.RegularExpressions;
using NhaTroTot.Model;
using Microsoft.Phone.Maps.Controls;
using Windows.Devices.Geolocation;
using System.Windows;
using System.Device.Location;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace NhaTroTot.Public
{
    public class AppManagement
    {
        public static string APIKEY = "AIzaSyCMLilhJVSsa_bsi3aR2zwH5Vk";
        public static string _URI = "http://api.nhatrotot.com/v1/api.php?";
        public static bool _flagStoryBoard = false;
        public static bool _flagnavigateBack = false;
        public static bool _flagFillMarker = false;
        public static bool _flagnavigateTo = true;
        public static bool flaglist = false;
        public static int _flagreload;
        public static bool _flagExitApp = false;
        public static bool _flagHouseSave;
        public static bool _flagclickMarker;
        public static bool _flagsaveNTTrangChu=false;
        public static USCustomPushpin _marker_pre=new USCustomPushpin();
        public static int currentindex;
        public static ICollection<NhaTro> list = new ObservableCollection<NhaTro>();
        public static NhaTro itemview;
        public static MapLayer _layer = new MapLayer();
        public static ICollection<NhaTro> listPivot;
        public static ICollection<NhaTro> _nhadaluu;
        public static ICollection<NhaTro> _nhadaxem;
        public static ICollection<NhaTro> _nhadaxoa;
        public static List<USCustomPushpin> _list_marker;
        public static SoNhaDaLuu _sndl;


        public class SoNhaDaLuu : INotifyPropertyChanged
        {
            private int _sonhadaluu;

            public int SoNhaLuu
            {
                get { return _sonhadaluu; }
                set 
                {
                    if(_sonhadaluu!=value)
                    {
                        _sonhadaluu = value;
                        NotifyPropertyChanged("SoNhaLuu");
                    }
                     
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            // This method is called by the Set accessor of each property. 
            // The CallerMemberName attribute that is applied to the optional propertyName 
            // parameter causes the property name of the caller to be substituted as an argument. 
            private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

        }
        public static class ListInfoExtend
        {
            private static ICollection<InfoExtends> _listInfoExtends = new ObservableCollection<InfoExtends>();

            public static ICollection<InfoExtends> ListInfoExtends
            {
                get { return _listInfoExtends; }
                set { _listInfoExtends = value; }
            }

        }
        public static class ListNhaTroInCircle
        {
            private static ICollection<NhaTro> _list_nt_in_circle = new ObservableCollection<NhaTro>();

            public static ICollection<NhaTro> List_nt_in_circle
            {
                get { return _list_nt_in_circle; }
                set { _list_nt_in_circle = value; }
            }

        }
        public static string GetMd5Hash(string str)
        {
            return MD5.GetHashString(str);
        }
        public static string CalDate(string timestamp)
        {
            var dt=new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds((Math.Round(double.Parse(timestamp)))).ToLocalTime();
            TimeSpan time = DateTime.Now - dt;
            if (time.Days == 0)
                return "Đăng hôm nay.";
            else
                return "Đăng cách đây " + time.Days.ToString() + " ngày.";
        }
        public static bool IsNumber(string pText)
        {
            Regex regex = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");
            return regex.IsMatch(pText);
        }

        public async static void GetGPS(Map myMap)
        {
            Geolocator locator = new Geolocator();
            if (locator.LocationStatus == PositionStatus.Disabled)
            {
                MessageBox.Show("GPS của bạn đang tắt, bất lên để sử dụng tính năng này !!");
            }
            else
            {
                locator.DesiredAccuracyInMeters = 50;
                var mypoint = await locator.GetGeopositionAsync();
                myMap.SetView(new GeoCoordinate(mypoint.Coordinate.Latitude, mypoint.Coordinate.Longitude), 17D);
            }
        }
    }
}
