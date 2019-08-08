using System;
using System.Collections.Generic;
using System.Text;

namespace MadPay724.Data.Dtos.Site.Panel.Document
{
    public class DocumentForReturnDto
    {
        public string Id { get; set; }
        public short Approve { get; set; }
        public bool IsTrue { get; set; }
        public string Name { get; set; }
        public string NationalCode { get; set; }
        public string FatherNameRegisterCode { get; set; }
        public DateTime BirthDay { get; set; }
        public string Address { get; set; }
        public string PicUrl { get; set; }

    }
}
