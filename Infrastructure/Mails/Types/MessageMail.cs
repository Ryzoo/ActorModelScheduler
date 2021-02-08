using Core.Interfaces.Mails;
using Newtonsoft.Json;

namespace Infrastructure.Mails.Types
{
    public class MessageParams : IEmailTemplateParams
    {
        public string UserName;
        public string UserEmail;
        public string Message;
    }
    
    public class MessageMail : BaseMail
    {
        public MessageMail()
        {
            Subject = "You have new message!";
            Template = "MessageMailTemplate";
        }

        protected override IEmailTemplateParams PrepareParams(string param)
        {
            return JsonConvert.DeserializeObject<MessageParams>(param);
        }
    }
}