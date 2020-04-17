using MadPay724.AspNetCore.GateWay.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.AspNetCore.GateWay
{
    public interface IMadPayGateWay
    {

        Task<MadPayGateResult<MadPayGatePayResponse>> PayAsync(MadPayGatePayRequest madPayGatePayRequest);

        Task<MadPayGateResult<MadPayGateVerifyResponse>> VerifyAsync(MadPayGateVerifyRequest madPayGateVerifyRequest);

        Task<MadPayGateResult<MadPayGateRefundResponse>> RefundAsync(MadPayGateRefundRequest madPayGateRefundRequest);

    }
}
