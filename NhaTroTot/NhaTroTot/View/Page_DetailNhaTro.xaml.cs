using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NhaTroTot.Public;
using NhaTroTot.Models;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Tasks;
using NhaTroTot.Controller;
using NhaTroTot.Usercontrols;
using System.Net.Http;
using Newtonsoft.Json;
using NhaTroTot.Model;

namespace NhaTroTot.View
{
    public partial class Page_DetailNhaTro : PhoneApplicationPage
    {
        NhaTro item;
        NhaTro itemInlist;
        NhaTro Infoex;
        USCustomPushpin _tooltip;
        public Page_DetailNhaTro()
        {
            InitializeComponent();
            item = new NhaTro();
            _tooltip = new USCustomPushpin();
            Infoex = new NhaTro();
            itemInlist = new NhaTro();
            
            
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (AppManagement._flagsaveNTTrangChu == true)
            //    NavigationService.GoBack();
            if(AppManagement._flagHouseSave==false)
            {
                string _url = "";
                _url = AppManagement._URI + "task=getRoomsByIds&scope=title,description,id,address&link=1&rids=" + AppManagement.listPivot.ElementAt(0).id + "&num=1";
                HttpClient client = new HttpClient();
                var response = await client.GetStringAsync(new Uri(_url));
                var data = JsonConvert.DeserializeObject<IDictionary<string, object>>(response)["data"];
                AppManagement.ListInfoExtend.ListInfoExtends = JsonConvert.DeserializeObject<List<InfoExtends>>(data.ToString());
                getItem();
                pivotNT.ItemsSource = AppManagement.listPivot;
                AppManagement._flagnavigateBack = true;
                AppManagement._flagFillMarker = true;
                item = pivotNT.Items.ElementAt(0) as NhaTro;

                itemInlist = AppManagement.list.FirstOrDefault(r => r.id == item.id);
                _tooltip = AppManagement._list_marker.FirstOrDefault(r => r.TagId == item.id);

                if (item.flagColor == 1)
                {
                    item.flagColor = 2;
                    itemInlist.flagColor = 2;
                    if (AppManagement._nhadaxem == null)
                    {
                        AppManagement._nhadaxem = new List<NhaTro>();
                    }
                    AppManagement._nhadaxem.Add(item);
                    Controller_Read_Write_NhaTro.WriteNhaXem();
                }

                _tooltip.ColorMarker = item.flagColor.ToString();
                _tooltip.FillMarker();
                iconStar.Source = new BitmapImage(new Uri(item.link, UriKind.Relative));   
            }
            else
            {
                pivotNT.ItemsSource = AppManagement.listPivot;
                AppManagement._flagnavigateBack = true;
                AppManagement._flagFillMarker = true;
                item = pivotNT.Items.ElementAt(0) as NhaTro;
                iconStar.Source = new BitmapImage(new Uri(item.link, UriKind.Relative)); 
            }
            
        }
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void getItem()
        {
            var tree = AppManagement.listPivot.ElementAt(0);
            var treeInfo = AppManagement.ListInfoExtend.ListInfoExtends.FirstOrDefault(r => r.id == tree.id);
            tree.description = treeInfo.description;
            tree.address = treeInfo.address;
            tree.slug = treeInfo.slug;
            var li = tree.image.Split(',');
            tree.ListImageShow = new List<NhaTro.ImageShow>();
            for (int i = 0; i < li.Length; i++)
            {
                if (li[i] != "")
                {
                    tree.ListImageShow.Add(new NhaTro.ImageShow() { ImageNT = li[i] });
                }
            }
            
        }

        private void SettingPanel_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(NavigationService.CanGoBack==true)
            {
                NavigationService.GoBack();
            }
        }

       
        private void CallPhone_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneCallTask phoneCallTask = new PhoneCallTask();

            phoneCallTask.PhoneNumber = item.phone;
            phoneCallTask.DisplayName = "Chủ Nhà Trọ";

            phoneCallTask.Show();
        }

        private void btnShare_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShareLinkTask shareLinkTask = new ShareLinkTask();
            shareLinkTask.LinkUri = new Uri("http://wwww.nhatrotot/chi-tiet/"+item.slug, UriKind.Absolute);

            shareLinkTask.Show();
            
        }


        private void SaveNhaTro_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if(item.flagColor==3)
            {
                item.flagColor = 2;
                if(AppManagement._flagHouseSave==false)
                    itemInlist.flagColor = 2;
                AppManagement._nhadaluu.Remove(AppManagement._nhadaluu.FirstOrDefault(r=>r.id==item.id));
                AppManagement._sndl.SoNhaLuu -= 1;
                AppManagement._nhadaxem.Add(item);
            }
            else
            {
                item.flagColor = 3;
                if(AppManagement._flagHouseSave==false)
                {
                    itemInlist.flagColor = 3;
                    _tooltip.PathStar = item.flagColor.ToString();
                    _tooltip.FillMarker();
                }
                   
                if (AppManagement._nhadaluu == null)
                {
                    AppManagement._nhadaluu = new ObservableCollection<NhaTro>();
                }
                AppManagement._nhadaluu.Add(item);
                AppManagement._sndl.SoNhaLuu += 1;
                if(AppManagement._nhadaxem.FirstOrDefault(r=>r.id==item.id)!=null)
                {
                    AppManagement._nhadaxem.Remove(AppManagement._nhadaxem.FirstOrDefault(r=>r.id==item.id));
                }
                
            }
            Controller_Read_Write_NhaTro.WriteNhaLuu();
            iconStar.Source = new BitmapImage(new Uri(item.link, UriKind.Relative));
        }

        private void btnDelete_Tapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBoxResult result =  MessageBox.Show("Bạn có muốn xóa nhà này khỏi bản đồ không?", "Cảnh báo", MessageBoxButton.OKCancel);
            if(result==MessageBoxResult.OK)
            {
                AppManagement._nhadaxoa.Add(item);
                if (item.flagColor == 3)
                    AppManagement._nhadaluu.Remove(AppManagement._nhadaluu.FirstOrDefault(r => r.id == item.id));
                else if(item.flagColor==2)
                    AppManagement._nhadaxem.Remove(AppManagement._nhadaxem.FirstOrDefault(r => r.id == item.id));
                if(AppManagement._flagHouseSave==false)
                {
                    AppManagement.list.Remove(item);
                    if (AppManagement.flaglist == false)
                    {
                        AppManagement.ListNhaTroInCircle.List_nt_in_circle.Remove(AppManagement.ListNhaTroInCircle.List_nt_in_circle.FirstOrDefault(r => r.id == item.id));
                    }
                    AppManagement._list_marker.Remove(_tooltip);
                    AppManagement._layer.Remove(AppManagement._layer.FirstOrDefault(r => r.Content == _tooltip));
                }
                Controller_Read_Write_NhaTro.WriteNhaXoa();
            }
        }

        private void BackTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}