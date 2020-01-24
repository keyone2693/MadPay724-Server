using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.Services.Site.Panel.Common.Interface
{
  public  interface IChatClient
    {
        Task SendMessage(string user, string message);
        Task SendMessage(string message);
    }
}
