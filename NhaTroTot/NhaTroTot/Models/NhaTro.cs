using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NhaTroTot.Public;

namespace NhaTroTot.Models
{
   public class NhaTro
    {
       
       private string _priceShow;
       private string _areShow;
       private string _area;
       private string _timestamp;
       private string _price;
       private string _link;
       private int _color = 1;
       public string PriceShow { get { return _priceShow; } set { _priceShow=value;} }
       public string AreaShow { get { return _areShow; } set { _areShow = value; } }      
       public int flagColor 
       { 
           get { return _color; } 
           set 
           { 
               _color = value;
               if (value == 3)
               {
                   link = "/Assets/Images/i_star-y32.png";
               }
               else
               {
                   link = "/Assets/Images/i_star-w32.png";
               }
           } 
       }
       public string id { get; set; }
       public string slug { get; set; }
       public string image { get; set; }
       public string title { get; set; }
       public string description { get; set; }
       public string location { get; set; }
       public string attribute { get; set; }
       public string slot { get; set; }
       public string link
       {
           get { return _link;}
           set { _link=value; }
       }
       public string price
       {
           get{ return _price; }
           set
           {
               _price = value;
               if (value != "0")
                   PriceShow = (double.Parse(price) / 1000000).ToString() + " triệu/tháng";
               else
                   PriceShow = "?";
           } 
       }
       public string area
       {
           get { return _area;}
           set
           {
               _area = value;
               if (value == "0" || value == "-1")
                   AreaShow = "Chưa rõ";
               else
                   AreaShow = value + " m2";
           }
       }
       public string address { get; set; }
       public string g_address { get; set; }
       public string longitude { get; set; }
       public string latitude { get; set; }
       public string author { get; set; }
       public string views { get; set; }
       public string phone { get; set; }
       public string last_updated { get; set; }
       public string revision { get; set; }
       public string city { get; set; }
       public string updated_timestamp 
       {
           get { return _timestamp; }
           set
           {
               if(AppManagement.IsNumber(value)==true)
               {
                   if (int.Parse(value) > 32)
                   {
                       _timestamp = AppManagement.CalDate(value);
                   }
                   else
                       _timestamp = value;
               }
               else
                   _timestamp = value;

              
           } 
       } 
       public string added_timestamp { get; set; }
       public string site { get; set; }

       public class ImageShow
       {
           public string ImageNT { get; set; }
       }
       public List<ImageShow> ListImageShow { get; set; }
    }
}
