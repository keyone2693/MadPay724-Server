using MadPay724.AspNetCore.GateWay.Data;
using MadPay724.AspNetCore.GateWay.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MadPay724.AspNetCore.GateWay
{
    public class MadPayGateWay : IMadPayGateWay, IDisposable
    {
        private readonly HttpClient _http;
        private StringContent _content;
        private HttpResponseMessage _response;

        public MadPayGateWay()
        {
            _http = new HttpClient();
        }

        #region AsyncMethods
        public async Task<MadPayGateResult<MadPayGatePayResponse>> PayAsync(MadPayGatePayRequest madPayGatePayRequest)
        {
            _http.DefaultRequestHeaders.Clear();

            _content = new StringContent(
                JsonConvert.SerializeObject(madPayGatePayRequest), UTF8Encoding.UTF8, "application/json");

            _response = await _http.PostAsync(ApiRoutes.Pay.PaySend, _content);

            if ((int)_response.StatusCode == 200)
            {
                return JsonConvert
                .DeserializeObject<MadPayGateResult<MadPayGatePayResponse>>(await _response.Content.ReadAsStringAsync());
            }
            else if((int)_response.StatusCode == 400)
            {
                var res = JsonConvert
                .DeserializeObject<MadPayGateResult<string>>(await _response.Content.ReadAsStringAsync());

                return new MadPayGateResult<MadPayGatePayResponse>
                {
                    Messages = res.Messages,
                    Status = false,
                    Result = null
                };
            }
            else
            {
                return new MadPayGateResult<MadPayGatePayResponse>
                {
                    Messages = new string[] {"خطای نامشخص"},
                    Status = false,
                    Result = null
                };
            }
        }

        public async Task<MadPayGateResult<MadPayGateVerifyResponse>> VerifyAsync(MadPayGateVerifyRequest madPayGateVerifyRequest)
        {
            _http.DefaultRequestHeaders.Clear();

            _content = new StringContent(
                JsonConvert.SerializeObject(madPayGateVerifyRequest), UTF8Encoding.UTF8, "application/json");

            _response = await _http.PostAsync(ApiRoutes.Verify.VerifySend, _content);

            if ((int)_response.StatusCode == 200)
            {
                return JsonConvert
                .DeserializeObject<MadPayGateResult<MadPayGateVerifyResponse>>(await _response.Content.ReadAsStringAsync());
            }
            else if ((int)_response.StatusCode == 400)
            {
                var res = JsonConvert
                .DeserializeObject<MadPayGateResult<string>>(await _response.Content.ReadAsStringAsync());

                return new MadPayGateResult<MadPayGateVerifyResponse>
                {
                    Messages = res.Messages,
                    Status = false,
                    Result = null
                };
            }
            else
            {
                return new MadPayGateResult<MadPayGateVerifyResponse>
                {
                    Messages = new string[] { "خطای نامشخص" },
                    Status = false,
                    Result = null
                };
            }
        }

        public async Task<MadPayGateResult<MadPayGateRefundResponse>> RefundAsync(MadPayGateRefundRequest madPayGateRefundRequest)
        {
            _http.DefaultRequestHeaders.Clear();

            _content = new StringContent(
                JsonConvert.SerializeObject(madPayGateRefundRequest), UTF8Encoding.UTF8, "application/json");

            _response = await _http.PostAsync(ApiRoutes.Refund.RefundSend, _content);

            if ((int)_response.StatusCode == 200)
            {
                return JsonConvert
                .DeserializeObject<MadPayGateResult<MadPayGateRefundResponse>>(await _response.Content.ReadAsStringAsync());
            }
            else if ((int)_response.StatusCode == 400)
            {
                var res = JsonConvert
                .DeserializeObject<MadPayGateResult<string>>(await _response.Content.ReadAsStringAsync());

                return new MadPayGateResult<MadPayGateRefundResponse>
                {
                    Messages = res.Messages,
                    Status = false,
                    Result = null
                };
            }
            else
            {
                return new MadPayGateResult<MadPayGateRefundResponse>
                {
                    Messages = new string[] { "خطای نامشخص" },
                    Status = false,
                    Result = null
                };
            }
        }

        #endregion

        #region Dispose
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _http.Dispose();
                    if(_content != null)
                        _content.Dispose();
                    if (_response != null)
                        _response.Dispose();
                    //
                    disposed = true;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MadPayGateWay()
        {
            Dispose(true);
        }
        #endregion

    }
}
