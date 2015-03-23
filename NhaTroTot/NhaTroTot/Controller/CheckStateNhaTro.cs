using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NhaTroTot.Public;

namespace NhaTroTot.Controller
{
    public class CheckStateNhaTro
    {
        public static void CheckState()
        {
            for (int i = 0; i < AppManagement.list.Count; i++)
            {
                if(AppManagement._nhadaluu.FirstOrDefault(r => r.id == AppManagement.list.ElementAt(i).id)!=null)
                {
                    AppManagement.list.ElementAt(i).flagColor = 3;
                }
                else if(AppManagement._nhadaxoa.FirstOrDefault(r => r.id == AppManagement.list.ElementAt(i).id)!=null)
                    {
                        AppManagement.list.Remove(AppManagement.list.ElementAt(i));
                        i -= 1; 
                    }
                else if(AppManagement._nhadaxem.FirstOrDefault(r => r.id == AppManagement.list.ElementAt(i).id)!=null)
                    {
                        AppManagement.list.ElementAt(i).flagColor = 2;
                    }
            }
        }
    }
}
