using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

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
