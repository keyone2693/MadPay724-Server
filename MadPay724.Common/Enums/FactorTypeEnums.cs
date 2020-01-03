using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MadPay724.Common.Enums
{
    public enum FactorTypeEnums
    {
        [Display(Name = "فاکتور")]
        Factor = 1,
        [Display(Name = "ایزی پی")]
        EasyPay = 2,
        [Display(Name = "حمایتی")]
        Support = 3
    }
}
