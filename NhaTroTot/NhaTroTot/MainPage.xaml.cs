using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NhaTroTot.Resources;
using System.Device.Location;
using NhaTroTot.Public;
using Newtonsoft.Json;
using NhaTroTot.Models;
using Microsoft.Phone.Maps.Controls;
using NhaTroTot.Usercontrols;
using NhaTroTot.Model;
using System.Windows.Media.Animation;
using System.Windows.Media;
using NhaTroTot.Framework;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using Microsoft.Phone.Tasks;
using System.Globalization;
using System.Net.Http;
using System.Net.NetworkInformation;
using Microsoft.Phone.Net.NetworkInformation;
using System.Diagnostics;
using NhaTroTot.Controller;

namespace NhaTroTot
{
    public partial class MainPage : PhoneApplicationPage
    {

        private double _dragDistanceToOpen = 75.0;
        private double _dragDistanceToClose = 305.0;
        private double _dragDistanceNegative = -75.0;
        private bool _isSettingsOpen = false;

        private FrameworkElement _feContainer;
        private List<Tinh_ThanhPho> _tinh_tp;
        private LocationAutoComplete.RootObject _autocomplete;
        
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            myMaps.Layers.Add(AppManagement._layer);
            _tinh_tp = new List<Tinh_ThanhPho>();
            AppManagement._list_marker = new List<USCustomPushpin>();
            listNhaTroInCircle.ItemsSource = AppManagement.ListNhaTroInCircle.List_nt_in_circle;
            //listNhaTroInCircleStar.ItemsSource = AppManagement.ListNhaTroInCircle.List_nt_in_circle;
            _feContainer = this.Containter as FrameworkElement;
            btnDanhSach.Tag = "Map";
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
            _autocomplete = new LocationAutoComplete.RootObject();
            AppManagement._sndl = new AppManagement.SoNhaDaLuu();
            ShowSoNhaDaLuu.DataContext = AppManagement._sndl;
            Controller.Controller_Read_Write_NhaTro.ReadNhaLuu();
            Controller.Controller_Read_Write_NhaTro.ReadNhaXem();
            Controller.Controller_Read_Write_NhaTro.ReadNhaXoa(); 
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            BackKeyPress+=MainPage_BackKeyPress;
            //listNhaTroInMap.ItemsSource = AppManagement.list;
            if(AppManagement._flagnavigateBack==true)
            {
                AppManagement._flagnavigateBack = false;
            }
            else
            {
                AppManagement._flagFillMarker = false;
                myMaps.ZoomLevel = 15;
                myMaps.Center = new GeoCoordinate(10.784898, 106.670082);
                if(checkNetworkConnection()==true)
                {
                    var client = new WebClient();
                    client.DownloadStringCompleted += client_DownloadStringCompleted;
                    client.DownloadStringAsync(new Uri(AppManagement._URI + "task=loadcfg"));
                }
                else
                {
                    MessageBox.Show("No internet connection is avaliable. The full functionality of the app isn't avaliable.");
                }
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            BackKeyPress -= MainPage_BackKeyPress;
        }
        private void MainPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
           if(AppManagement._flagExitApp==false)
           {
               Grid2.Visibility = Visibility.Collapsed;
               gridcontext.Visibility = Visibility.Collapsed;
               StoryboardBottom.Stop();
               txtDanhsach.Text = "Danh sách";
               AppManagement._flagExitApp = true;
               AppManagement._flagHouseSave = false;
               
               e.Cancel = true;
           }
           else
           {
               MessageBoxResult result = MessageBox.Show("Bạn có muốn thoát khỏi ứng dụng không?","Thông báo", MessageBoxButton.OKCancel);
               if(result==MessageBoxResult.OK)
               {
                   App.Current.Terminate();
               }
               else
               {
                   e.Cancel = true;
               }
           }
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
             var response = e.Result;
             var data = JsonConvert.DeserializeObject<IDictionary<string, object>>(response)["data"];
             var locations = JsonConvert.DeserializeObject<IDictionary<string, object>>(data.ToString())["locations"];
             if (locations != null)
             {
                 _tinh_tp = JsonConvert.DeserializeObject<List<Tinh_ThanhPho>>(locations.ToString());
                 var top = _tinh_tp.FindAll(r => r.Parent == "top");
                 listContext.ItemsSource = top;
             }
        }
        private void CloseSettings()
        {
            
            var trans = _feContainer.GetHorizontalOffset().Transform;

            trans.Animate(trans.X, 0, TranslateTransform.XProperty, 300, 0, new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            });

            _isSettingsOpen = false;
        }

        private void OpenSettings()
        {
            var trans = _feContainer.GetHorizontalOffset().Transform;
            trans.Animate(trans.X, 380, TranslateTransform.XProperty, 300, 0, new CubicEase
            {
                EasingMode = EasingMode.EaseOut
            });

            _isSettingsOpen = true;
        }
        private void SettingPanel_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
                if (_isSettingsOpen)
                {
                    CloseSettings();
                }
                else
                {
                    OpenSettings();
                }             
        }

        private void ButtonSeach_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CloseSettings();
            if (gridcontext.Visibility == Visibility.Visible)
            {
                gridcontext.Visibility = Visibility.Collapsed;
                AppManagement._flagExitApp =true;
            }
            else
            {
                gridcontext.Visibility = Visibility.Visible;
                AppManagement._flagExitApp = false;
            }
        }

        private void Containter_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {

        }

        private void Containter_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {

        }

        private void GestureListener_OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal && e.HorizontalChange > 0 && !_isSettingsOpen)
            {
                double offset = _feContainer.GetHorizontalOffset().Value + e.HorizontalChange;
                if (offset > _dragDistanceToOpen)
                    this.OpenSettings();
                else
                    _feContainer.SetHorizontalOffset(offset);
            }

            if (e.Direction == System.Windows.Controls.Orientation.Horizontal && e.HorizontalChange < 0 && _isSettingsOpen)
            {
                double offsetContainer = _feContainer.GetHorizontalOffset().Value + e.HorizontalChange;
                if (offsetContainer < _dragDistanceToClose)
                    this.CloseSettings();
                else
                    _feContainer.SetHorizontalOffset(offsetContainer);
            }
        }

        private void GestureListener_OnDragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal && e.HorizontalChange > 0 && !_isSettingsOpen)
            {
                if (e.HorizontalChange < _dragDistanceToOpen)
                    this.ResetLayoutRoot();
                else
                    this.OpenSettings();
            }

            if (e.Direction == System.Windows.Controls.Orientation.Horizontal && e.HorizontalChange < 0 && _isSettingsOpen)
            {
                if (e.HorizontalChange > _dragDistanceNegative)
                    this.ResetLayoutRoot();
                else
                    this.CloseSettings();
            }
        }

        private void ResetLayoutRoot()
        {
            if (!_isSettingsOpen)
                _feContainer.SetHorizontalOffset(0.0);
            else
                _feContainer.SetHorizontalOffset(380.0);
        }

        private void myMaps_ResolveCompleted(object sender, Microsoft.Phone.Maps.Controls.MapResolveCompletedEventArgs e)
        {
            
            if(AppManagement._flagStoryBoard==true||AppManagement._flagclickMarker==true)
            {
                AppManagement._flagStoryBoard = false;
                AppManagement._flagclickMarker = false;
            }
            else
            {
                if (checkNetworkConnection() == true)
                {
                    if (StoryboardBottom.GetCurrentState() == ClockState.Active)
                    {
                        AppManagement._flagExitApp = true;
                        StoryboardBottom.Stop();
                    }
                 
                    LocationRectangle rec = GetVisibleMapAre(myMaps);
                    GetListNhaTro(rec.Southeast, rec.Northwest);
                    
                    //customIndeterminateProgressBar.IsIndeterminate = false;
                }
            }
            
        }

        //private async void TextAutoComplete(object sender, TextChangedEventArgs e)
        //{
        //    string input = Textchanged.Text;
        //    if (input == "")
        //    {
        //        listAutoComplete.Visibility = Visibility.Collapsed;
        //        listContext.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            var response = await client.GetStringAsync("https://maps.googleapis.com/maps/api/place/autocomplete/json?input=" + input + "&types=geocode&language=vi&sensor=true&key=AIzaSyBw9SrK5W_b6QYaGzBLCsqcMPH0qtGqwU4");
        //            _autocomplete = JsonConvert.DeserializeObject<LocationAutoComplete.RootObject>(response);
        //            if (_autocomplete.status == "OK")
        //            {
        //                listAutoComplete.ItemsSource = _autocomplete.predictions;
        //                listAutoComplete.Visibility = Visibility.Visible;
        //                listContext.Visibility = Visibility.Collapsed;

        //            }
        //            else
        //            {
        //                _autocomplete.predictions.Clear();
        //                listAutoComplete.ItemsSource = _autocomplete.predictions;
        //                listContext.Visibility = Visibility.Visible;
        //                listAutoComplete.Visibility = Visibility.Collapsed;

        //            }
        //        }
        //    } 
        //}

        private void PressEnter(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void listContext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_tinh_tp != null && listContext.SelectedIndex != -1)
            {

                Tinh_ThanhPho _quanPhuong = listContext.SelectedItem as Tinh_ThanhPho;
                if (_quanPhuong.Id == "my_location")
                {
                    gridcontext.Visibility = Visibility.Collapsed;
                    AppManagement.GetGPS(myMaps);
                }
                else
                {
                    var listQuanPhuong = _tinh_tp.FindAll(r => r.Parent == _quanPhuong.Id);
                    listQuan_Phuong.ItemsSource = listQuanPhuong;
                    gridcontext.Visibility = Visibility.Collapsed;
                    gridQuanPhuong.Visibility = Visibility.Visible;
                }

            }
            listContext.SelectedIndex = -1;
        }

        private void listAutoComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void listQuan_Phuong_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listQuan_Phuong.SelectedIndex != -1)
            {
                gridQuanPhuong.Visibility = Visibility.Collapsed;
                Tinh_ThanhPho _quanPhuong = listQuan_Phuong.SelectedItem as Tinh_ThanhPho;
                GeoCoordinate _geopoint = new GeoCoordinate( double.Parse(_quanPhuong.Latitude,CultureInfo.InvariantCulture),double.Parse(_quanPhuong.Longitude,CultureInfo.InvariantCulture));
                myMaps.SetView(_geopoint, _quanPhuong.Zoom);
            }
        }

        private void ButtonDanhSach_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (btnDanhSach.Tag != "Map")
            {
                txtDanhsach.Text = "Danh sách";
                btnDanhSach.Tag = "Map";
                Grid2.Visibility = Visibility.Collapsed;
                AppManagement._flagExitApp = true;
            }
            else
            {
                txtDanhsach.Text = "Bản đồ";
                btnDanhSach.Tag = "List";
                Grid2.Visibility = Visibility.Visible;
                listNhaTroInMap.ItemsSource = AppManagement.list;
                AppManagement._flagHouseSave = false;
                AppManagement._flagExitApp = false;
            }
        }

        private void listNhaTroInCircle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        
        {
            if (listNhaTroInCircle.SelectedIndex != -1)
            {
                AppManagement.listPivot = new ObservableCollection<NhaTro>();
                AppManagement.listPivot.Add(listNhaTroInCircle.SelectedItem as NhaTro);
                AppManagement.flaglist = false;
                NavigationService.Navigate(new Uri("/View/Page_DetailNhaTro.xaml", UriKind.Relative));
                listNhaTroInCircle.SelectedIndex = -1;
            }
            
        }

        //void nhatro_circle_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        //{
        //    var response=e.Result;
        //    var data = JsonConvert.DeserializeObject<IDictionary<string, object>>(response)["data"];
        //    AppManagement.ListInfoExtend.ListInfoExtends = JsonConvert.DeserializeObject<List<InfoExtends>>(data.ToString());
        //    //for (int i = 0; i < AppManagement.ListNhaTroInCircle.List_nt_in_circle.Count; i++)
        //    //{
        //    //    var iteminfo = AppManagement.ListInfoExtend.ListInfoExtends.FirstOrDefault(r => r.id == AppManagement.ListNhaTroInCircle.List_nt_in_circle.ElementAt(i).id);
        //    //    AppManagement.ListNhaTroInCircle.List_nt_in_circle.ElementAt(i).address = iteminfo.address;
        //    //    AppManagement.ListNhaTroInCircle.List_nt_in_circle.ElementAt(i).description = iteminfo.description;
        //    //    var li = AppManagement.ListNhaTroInCircle.List_nt_in_circle.ElementAt(i).image.Split(',');
        //    //    AppManagement.ListNhaTroInCircle.List_nt_in_circle.ElementAt(i).ListImageShow = new List<NhaTro.ImageShow>();
        //    //    for (int j = 0; j < li.Length; j++)
        //    //    {
        //    //        if (li[j] != "")
        //    //        {
        //    //            AppManagement.ListNhaTroInCircle.List_nt_in_circle.ElementAt(i).ListImageShow.Add(new NhaTro.ImageShow() { ImageNT = li[j] });
        //    //        }
        //    //    }
        //    //}
        //    AppManagement.currentindex = listNhaTroInCircle.SelectedIndex;
        //    AppManagement.listPivot = AppManagement.ListNhaTroInCircle.List_nt_in_circle;
        //    AppManagement.flaglist = false;
        //    NavigationService.Navigate(new Uri("/View/Page_DetailNhaTro.xaml", UriKind.Relative));
        //    //listNhaTroInCircle.SelectedIndex = -1;
        //}

        private void listNhaTroInMap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("anhtuan");
            if (listNhaTroInMap.SelectedIndex != -1)
            {
                AppManagement.listPivot = new ObservableCollection<NhaTro>();
                
                AppManagement.listPivot.Add(listNhaTroInMap.SelectedItem as NhaTro);
                AppManagement.flaglist = true;
                NavigationService.Navigate(new Uri("/View/Page_DetailNhaTro.xaml", UriKind.Relative));
                listNhaTroInMap.SelectedIndex = -1;
            }
        }

        private void nhatro_map_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            
            var response = e.Result;
            var data = JsonConvert.DeserializeObject<IDictionary<string, object>>(response)["data"];
            AppManagement.ListInfoExtend.ListInfoExtends = JsonConvert.DeserializeObject<List<InfoExtends>>(data.ToString());
           
            //AppManagement.currentindex = listNhaTroInMap.SelectedIndex;
            AppManagement.listPivot = AppManagement.list;
            AppManagement.flaglist = true;
            NavigationService.Navigate(new Uri("/View/Page_DetailNhaTro.xaml", UriKind.Relative));
           
        }

        private LocationRectangle GetVisibleMapAre(Map mMap)
        {
            GeoCoordinate mCenter = mMap.Center;
            Point pCenter = mMap.ConvertGeoCoordinateToViewportPoint(mCenter);
            GeoCoordinate topLeft = myMaps.ConvertViewportPointToGeoCoordinate(new Point(0, 0));
            GeoCoordinate bottomRight = myMaps.ConvertViewportPointToGeoCoordinate(new Point(myMaps.ActualWidth, myMaps.ActualHeight));

            if (topLeft != null && bottomRight != null)
            {
                Point pNW = new Point(pCenter.X - mMap.ActualWidth / 2, pCenter.Y - mMap.ActualHeight / 2);
                Point pSE = new Point(pCenter.X + mMap.ActualWidth / 2, pCenter.Y + mMap.ActualHeight / 2);
                if (pNW != null && pSE != null)
                {
                    GeoCoordinate gcNW = mMap.ConvertViewportPointToGeoCoordinate(pNW);
                    GeoCoordinate gcSE = mMap.ConvertViewportPointToGeoCoordinate(pSE);
                    return new LocationRectangle(gcNW, gcSE);
                }
            }

            return null; 
        }
        private  void GetListNhaTro(GeoCoordinate se, GeoCoordinate nw)
        {
            string str = nw.Latitude.ToString() + se.Latitude.ToString() + AppManagement.APIKEY;
            var hash = AppManagement.GetMd5Hash(str);
            string url = AppManagement._URI +
                         "selng=" +   se.Longitude.ToString(CultureInfo.InvariantCulture) +
                         "&selat=" + se.Latitude.ToString(CultureInfo.InvariantCulture) +
                         "&type=3" +
                         "&zoom=12" +
                         "&nwlat=" + nw.Latitude.ToString(CultureInfo.InvariantCulture) +
                         "&nwlng=" +  nw.Longitude.ToString(CultureInfo.InvariantCulture) +
                         "&hash=" + hash +
                         "&task=getRoomsByLocation" +
                         "&link=1";
            var getListNhaTroClient = new WebClient();
            getListNhaTroClient.DownloadStringCompleted += getListNhaTroClient_DownloadStringCompleted;
            getListNhaTroClient.DownloadStringAsync(new Uri(url));
        }

        void getListNhaTroClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            var respone = e.Result;
            var x = JsonConvert.DeserializeObject<IDictionary<string, object>>(respone)["data"];
            AppManagement.list.Clear();
            if (x != null)
            {
                AppManagement.list = JsonConvert.DeserializeObject<ObservableCollection<NhaTro>>(x.ToString());
                Controller.CheckStateNhaTro.CheckState();
                Remove_Add_NhaTro(AppManagement.list);
            }
            if (AppManagement._flagHouseSave == true)
                listNhaTroInMap.ItemsSource = AppManagement._nhadaluu;
            else
                listNhaTroInMap.ItemsSource = AppManagement.list;
            txtSoPhong.Text = "Tìm được " + AppManagement.list.Count.ToString() + " phòng";
        }
        
        private void Remove_Add_NhaTro(ICollection<NhaTro> newlist)
        {
            for (int i = 0; i < AppManagement._list_marker.Count; i++)
            {
                int dem = 0;
                for (int j = 0; j < newlist.Count; j++)
                {
                    if (AppManagement._list_marker[i].TagId == newlist.ElementAt(j).id)
                    {
                        break;
                    }
                    else
                    {
                        dem += 1;
                    }
                }
                if(dem==newlist.Count)
                {
                    var marker = AppManagement._list_marker.ElementAt(i);
                    AppManagement._list_marker.Remove(marker);
                    AppManagement._layer.Remove(AppManagement._layer.FirstOrDefault(r => r.Content == marker));
                    i -= 1;
                }
            }

            for (int i = 0; i < newlist.Count; i++)
            {
                int dem = 0;
                for (int j = 0; j < AppManagement._list_marker.Count; j++)
                {
                    if (AppManagement._list_marker[j].TagId == newlist.ElementAt(i).id)
                    {
                        break;
                    }
                    else
                    {
                        dem += 1;
                    }
                }
                if (dem == AppManagement._list_marker.Count)
                {
                    USCustomPushpin _tooltip = new USCustomPushpin();
                    _tooltip.TagId = newlist.ElementAt(i).id;
                    _tooltip.Price = newlist.ElementAt(i).price;
                    _tooltip.PathStar = newlist.ElementAt(i).flagColor.ToString();
                    _tooltip.ColorMarker = newlist.ElementAt(i).flagColor.ToString();
                    _tooltip.FillMarker();
                    var _grid = _tooltip.Content as Grid;
                    var _stackpanel = _grid.Children.FirstOrDefault(r => r.GetType() == new StackPanel().GetType()) as StackPanel;
                    _stackpanel.Tag = _tooltip.TagId;
                    _stackpanel.Tap += Marker_Tapped;
                    AppManagement._list_marker.Add(_tooltip);
                    MapOverlay overlay = new MapOverlay();
                    overlay.Content = _tooltip;
                    GeoCoordinate coo = new GeoCoordinate();
                    coo.Latitude = double.Parse(AppManagement.list.ElementAt(i).latitude, CultureInfo.InvariantCulture);
                    coo.Longitude = double.Parse(AppManagement.list.ElementAt(i).longitude, CultureInfo.InvariantCulture);
                    overlay.GeoCoordinate = coo;

                    AppManagement._layer.Add(overlay);
                }
            }
        }

        private void Marker_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            myMaps.MapElements.Clear();
            AppManagement._marker_pre.Marker_Selected = false;
            AppManagement._flagclickMarker = true;
            var _stack_marker =sender as StackPanel;
            var marker = AppManagement._list_marker.FirstOrDefault(r => r.TagId == _stack_marker.Tag) as USCustomPushpin;
            var layer = AppManagement._layer.FirstOrDefault(r => r.Content == marker);
            marker.Marker_Selected = true;
            AppManagement._marker_pre = marker;
            DrawCirclesInMap circle = new DrawCirclesInMap();
            circle.DrawCircles(myMaps, layer.GeoCoordinate);
            AppManagement.ListNhaTroInCircle.List_nt_in_circle.Clear();
            for (int i = 0; i < AppManagement.list.Count; i++)
            {
                GeoCoordinate point = new GeoCoordinate(double.Parse(AppManagement.list.ElementAt(i).latitude, CultureInfo.InvariantCulture), double.Parse(AppManagement.list.ElementAt(i).longitude, CultureInfo.InvariantCulture));
                if (circle.distance2M(circle.GetData_Geo.I, point) <= circle.GetData_Geo.R)
                {
                    AppManagement.ListNhaTroInCircle.List_nt_in_circle.Add(new NhaTro()
                    {
                        id = AppManagement.list.ElementAt(i).id,
                        price = AppManagement.list.ElementAt(i).price,
                        area = AppManagement.list.ElementAt(i).area,
                        title = AppManagement.list.ElementAt(i).title,
                        updated_timestamp = AppManagement.list.ElementAt(i).updated_timestamp,
                        slug = AppManagement.list.ElementAt(i).slug,
                        image = AppManagement.list.ElementAt(i).image,
                        phone = AppManagement.list.ElementAt(i).phone,
                        site=AppManagement.list.ElementAt(i).site,
                        flagColor=AppManagement.list.ElementAt(i).flagColor
                    });
                }
            }
            AppManagement._flagStoryBoard = true;
            AppManagement._flagExitApp = false;
            StoryboardBottom.Begin();
        }

        private void BtnDangTin_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Hiện tại chức năng đăng tin đang được hoàn thiện, bạn có thể đăng tin tại website nhatrotot.com.",
    "Thông báo", MessageBoxButton.OKCancel);
            if(result==MessageBoxResult.OK)
            {
                 WebBrowserTask webBrowserTask = new WebBrowserTask();
                 webBrowserTask.Uri = new Uri("http://www.nhatrotot.com"); 
                 webBrowserTask.Show(); 
            }
        }

        private void BtnPhanHoi_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            webBrowserTask.Uri = new Uri("https://docs.google.com/forms/d/1cDAvuxIPbS2Xzp5v2xRtxUw_5vP5vz0QKH_WLEC6v74/viewform");
            webBrowserTask.Show(); 
        }

        private void map_loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "471b98ab-d776-40ec-b99c-f6bce6025130";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "iplKm4LOqP75GooxDXOU7w";
        }

        private void ButtonShowNhaDaLuu_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(Grid2.Visibility==Visibility.Collapsed)
            {
                Grid2.Visibility = Visibility.Visible;
                StoryboardBottom.Stop();
                listNhaTroInMap.ItemsSource = AppManagement._nhadaluu;
                AppManagement._flagHouseSave = true;
                AppManagement._flagExitApp = false;
            }
           else
            {
                AppManagement._flagHouseSave = false;
                AppManagement._flagExitApp = true;
                Grid2.Visibility = Visibility.Collapsed;
            }
            
        }


        public static bool checkNetworkConnection()
        {
            var ni = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;

            bool IsConnected = false;
            if ((ni == NetworkInterfaceType.Wireless80211) || (ni == NetworkInterfaceType.MobileBroadbandCdma) || (ni == NetworkInterfaceType.MobileBroadbandGsm))
                IsConnected = true;
            else if (ni == NetworkInterfaceType.None)
                IsConnected = false;
            return IsConnected;
        }

        private void gridchildcontext_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (gridcontext.Visibility == Visibility.Visible)
                gridcontext.Visibility = Visibility.Collapsed;
        }

        private void saveNhaTroTrangChu_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string s = "";
            //AppManagement._flagsaveNTTrangChu = true;
            //Image star = sender as Image;
            //NhaTro item = AppManagement.ListNhaTroInCircle.List_nt_in_circle.FirstOrDefault(r => r.id == star.Tag.ToString());
            //NhaTro itemInlist = AppManagement.list.FirstOrDefault(r => r.id == star.Tag.ToString());;
            //item.flagColor = 3;

            //    itemInlist.flagColor = 3;
            //    USCustomPushpin _tooltip = AppManagement._list_marker.FirstOrDefault(r => r.TagId == star.Tag.ToString()); ;
            //    _tooltip.PathStar = item.flagColor.ToString();
            //    _tooltip.FillMarker();

            //if (AppManagement._nhadaluu == null)
            //{
            //    AppManagement._nhadaluu = new ObservableCollection<NhaTro>();
            //}
            //AppManagement._nhadaluu.Add(item);
            //AppManagement._sndl.SoNhaLuu += 1;
            //if (AppManagement._nhadaxem.FirstOrDefault(r => r.id == item.id) != null)
            //{
            //    AppManagement._nhadaxem.Remove(AppManagement._nhadaxem.FirstOrDefault(r => r.id == item.id));
            //}
            //Controller_Read_Write_NhaTro.WriteNhaLuu();
        }

        private void iconstar_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string s = "";
        }

      

        //private async void Textchanged_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    string input = Textchanged.Text;
        //    if (input == "")
        //    {
        //        listAutoComplete.Visibility = Visibility.Collapsed;
        //        listContext.Visibility = Visibility.Visible;
        //    }
        //    else
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            var response = await client.GetStringAsync("https://maps.googleapis.com/maps/api/place/autocomplete/json?input=" + input + "&types=geocode&language=vi&sensor=true&key=AIzaSyBw9SrK5W_b6QYaGzBLCsqcMPH0qtGqwU4");
        //            _autocomplete = JsonConvert.DeserializeObject<LocationAutoComplete.RootObject>(response);
        //            if (_autocomplete.status == "OK")
        //            {
        //                listAutoComplete.ItemsSource = _autocomplete.predictions;
        //                listAutoComplete.Visibility = Visibility.Visible;
        //                listContext.Visibility = Visibility.Collapsed;

        //            }
        //            else
        //            {
        //                _autocomplete.predictions.Clear();
        //                listAutoComplete.ItemsSource = _autocomplete.predictions;
        //                listContext.Visibility = Visibility.Visible;
        //                listAutoComplete.Visibility = Visibility.Collapsed;

        //            }
        //        }
        //    } 
        //}


    }
}