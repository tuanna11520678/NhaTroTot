using Newtonsoft.Json;
using NhaTroTot.Models;
using NhaTroTot.Public;
using System;
using System.IO;
using System.Runtime;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Runtime.Serialization.Json;
using System.Collections.ObjectModel;

namespace NhaTroTot.Controller
{
     public class Controller_Read_Write_NhaTro
    {
         public static string m_ProductFileLuuNha = "Nhadaluu.json";
         public static string m_ProductFileXemNha = "Nhadaxem.json";
         public static string m_ProductFileXoaNha = "Nhadaxoa.json";
         public static async void ReadNhaLuu()
         {

             var textFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(m_ProductFileLuuNha, CreationCollisionOption.OpenIfExists);
             StorageFolder localFolder = ApplicationData.Current.LocalFolder;
             try
             {
                 // Getting JSON from file if it exists, or file not found exception if it does not
                 //StorageFile textFile = await localFolder.GetFileAsync(m_ProductFileName);

                 using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
                 {
                     // Read text stream 
                     using (DataReader textReader = new DataReader(textStream))
                     {
                         //get size
                         uint textLength = (uint)textStream.Size;
                         await textReader.LoadAsync(textLength);
                         // read it
                         string jsonContents = textReader.ReadString(textLength);
                         // deserialize back to our products!
                         //I only had to change this following line in this function
                         AppManagement._nhadaluu=new ObservableCollection<NhaTro>();
                         List<NhaTro> temp  = JsonConvert.DeserializeObject<IList<NhaTro>>(jsonContents) as List<NhaTro>;
                         if (temp != null)
                         {
                             for (int i = 0; i < temp.Count; i++)
                             {
                                 AppManagement._nhadaluu.Add(temp[i]);
                             }
                         }
                         
                         AppManagement._sndl.SoNhaLuu = AppManagement._nhadaluu.Count;
                         // and show it
                     }
                 }
             }
             catch (Exception ex)
             {
                
             }

         }
         public static async void ReadNhaXem()
         {

             var textFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(m_ProductFileXemNha, CreationCollisionOption.OpenIfExists);
             StorageFolder localFolder = ApplicationData.Current.LocalFolder;
             try
             {
                 // Getting JSON from file if it exists, or file not found exception if it does not
                 //StorageFile textFile = await localFolder.GetFileAsync(m_ProductFileName);

                 using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
                 {
                     // Read text stream 
                     using (DataReader textReader = new DataReader(textStream))
                     {
                         //get size
                         uint textLength = (uint)textStream.Size;
                         await textReader.LoadAsync(textLength);
                         // read it
                         string jsonContents = textReader.ReadString(textLength);
                         // deserialize back to our products!
                         //I only had to change this following line in this function
                         AppManagement._nhadaxem = JsonConvert.DeserializeObject<ICollection<NhaTro>>(jsonContents);
                         if (AppManagement._nhadaxem == null)
                             AppManagement._nhadaxem = new ObservableCollection<NhaTro>();
                         // and show it
                     }
                 }
             }
             catch (Exception ex)
             {

             }

         }
         public static async void ReadNhaXoa()
         {

             var textFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(m_ProductFileXoaNha, CreationCollisionOption.OpenIfExists);
             StorageFolder localFolder = ApplicationData.Current.LocalFolder;
             try
             {
                 // Getting JSON from file if it exists, or file not found exception if it does not
                 //StorageFile textFile = await localFolder.GetFileAsync(m_ProductFileName);

                 using (IRandomAccessStream textStream = await textFile.OpenReadAsync())
                 {
                     // Read text stream 
                     using (DataReader textReader = new DataReader(textStream))
                     {
                         //get size
                         uint textLength = (uint)textStream.Size;
                         await textReader.LoadAsync(textLength);
                         // read it
                         string jsonContents = textReader.ReadString(textLength);
                         // deserialize back to our products!
                         //I only had to change this following line in this function
                         AppManagement._nhadaxoa = JsonConvert.DeserializeObject<ICollection<NhaTro>>(jsonContents);
                         if (AppManagement._nhadaxoa == null)
                             AppManagement._nhadaxoa = new ObservableCollection<NhaTro>();
                         // and show it
                     }
                 }
             }
             catch (Exception ex)
             {

             }

         }
         public static async void WriteNhaLuu()
         {
             var serialize = new DataContractJsonSerializer(typeof(List<NhaTro>));
             using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(m_ProductFileLuuNha, CreationCollisionOption.ReplaceExisting))
             {
                 serialize.WriteObject(stream, AppManagement._nhadaluu);
             }
         }
         public static async void WriteNhaXem()
         {
             var serialize = new DataContractJsonSerializer(typeof(List<NhaTro>));
             using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(m_ProductFileXemNha, CreationCollisionOption.ReplaceExisting))
             {
                 serialize.WriteObject(stream, AppManagement._nhadaxem);
             }
         }
         public static async void WriteNhaXoa()
         {
             var serialize = new DataContractJsonSerializer(typeof(List<NhaTro>));
             using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(m_ProductFileXoaNha, CreationCollisionOption.ReplaceExisting))
             {
                 serialize.WriteObject(stream, AppManagement._nhadaxoa);
             }
         }
    }
}
