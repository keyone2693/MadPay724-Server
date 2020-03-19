using SmsIrRestfulNetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MadPay724.Services.Common
{
    public class SmsService : ISmsService
    {
        private readonly string _token;
        public SmsService()
        {
            _token = new Token().GetToken("fa8d78e8df8e169d20b378e8", "aa00");
        }
        #region Auth
        public bool SendFastVerificationCode(string mobile, string code)
        {
            try
            {
                var fastSend = new UltraFastSend()
                {
                    Mobile = Convert.ToInt64(mobile),
                    TemplateId = 21555,
                    ParameterArray = new List<UltraFastParameters>{
                        new UltraFastParameters
                        {
                            Parameter = "VerificationCode" , ParameterValue = code
                        }
                    }.ToArray()
                };
                var fastSendRes = new UltraFast().Send(_token, fastSend);
                if (fastSendRes.IsSuccessful)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        public bool SendVerificationCode(string mobile, string code)
        {
            var message = $"کاربر گرامی با تشکر از پیسوتن به مادپی 724 ، کد ثبت نام :{code}";
            var mobileNumbers = new string[] { mobile };
            var messages = new string[] { message };
            try
            {
                var messageRes = new MessageSend().Send(_token, new MessageSendObject
                {
                    MobileNumbers = mobileNumbers,
                    Messages = messages,
                    LineNumber = GetLineNumber(),
                    SendDateTime = DateTime.Now.AddSeconds(30),
                    CanContinueInCaseOfError = true
                });
                if (messageRes.IsSuccessful)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }

        }
        #endregion
        #region Common
        public string GetLineNumber()
        {
            var credit = new SmsLine().GetSmsLines(_token);
            if (credit == null)
                return "50002015997738";

            if (credit.IsSuccessful && credit.SMSLines.Any())
                return credit.SMSLines.FirstOrDefault().LineNumber.ToString();
            else
                return "50002015997738";
        }
        #endregion



    }
}
