using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhaTroTot.Models
{
    public class Tinh_ThanhPho
    {
        public String Id { get; set; }
        public String Parent { get; set; }
        public String Name { get; set; }
        public String Latitude { get; set; }
        public String Longitude { get; set; }
        public double Zoom { get; set; }
    }
}
