using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using NhaTroTot.Public;

namespace NhaTroTot.Usercontrols
{
    public partial class USCustomPushpin : UserControl
    {
        private bool _marker_selected;
        public bool Marker_Selected
        {
            get { return _marker_selected; }
            set
            {
                _marker_selected = value;
                FillMarker_Selected();
            }
        }

        private void FillMarker_Selected()
        {
            if(Marker_Selected==true)
            {
                imgborder.Background = new SolidColorBrush(Color.FromArgb(255, 4, 180, 180));
                imgpath.Fill = new SolidColorBrush(Color.FromArgb(255, 4, 180, 180));
            }
            else
            {
                if (ColorMarker == "#dd581e")
                {
                    imgborder.Background = new SolidColorBrush(Color.FromArgb(255, 221, 88, 30));
                    imgpath.Fill = new SolidColorBrush(Color.FromArgb(255, 221, 88, 30));
                }
                else
                {
                    imgborder.Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 243));
                    imgpath.Fill = new SolidColorBrush(Color.FromArgb(255, 241, 241, 243));
                }
            }
        }
        private string _price;
        public String Price 
        {
            get { return _price; }
            set {
                double temp = double.Parse(value);
                string price = (temp / 1000000).ToString() + "tr";
                _price=price;
            }
        }

        private string _tagId;
        public string TagId 
        {
            get { return _tagId;}
            set { _tagId=value;}
        }

        private string _colorMarker;
        public String ColorMarker
        {
            get {return _colorMarker ;}
            set 
            { 
                int temp = int.Parse(value);
                if (temp == 1)
                { _colorMarker = "#dd581e"; }
                else 
                {
                    _colorMarker = "#f1f1f3";
                }
            } 
        }

        private String _pathStar;
        public String PathStar
        {
            get{return _pathStar;}
            set
            { 
                int temp = int.Parse(value);
                if(temp==3)
                {
                    _pathStar = "/Assets/Images/i_star-y32.png";
                }
                else
                {
                    _pathStar = "";
                }
            }
        }
        public void FillMarker()
        {
            txtPrice.Text = Price;
            imgborder.Tag = TagId;
            if (ColorMarker == "#dd581e")
            {
                imgborder.Background = new SolidColorBrush(Color.FromArgb(255, 221, 88, 30));
                imgpath.Fill = new SolidColorBrush(Color.FromArgb(255, 221, 88, 30));
            }
            else
            {
               
                imgborder.Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 243));
                imgpath.Fill = new SolidColorBrush(Color.FromArgb(255, 241, 241, 243));
                    
                //else
                //{
                //    imgborder.Background = new SolidColorBrush(Color.FromArgb(255, 4, 180, 180));
                //    imgpath.Fill = new SolidColorBrush(Color.FromArgb(255, 4, 180, 180));
                //}
                    
            }
            imgStar.Source = new BitmapImage(new Uri(PathStar,UriKind.Relative));
        }
        public USCustomPushpin()
        {
            InitializeComponent();
            Loaded += USCustomPushpin_Loaded;
        }

        void USCustomPushpin_Loaded(object sender, RoutedEventArgs e)
        {   
            if(AppManagement._flagFillMarker==false)
            {
                FillMarker();
            }
        }

    }
}
