using System;
namespace Registro.Controls
{
    public class Mails
    {
        public interface IMailAndroid
        {
            void SendEmail();
        }

        public interface IMailiOS
        {
            void SendEmail();
        }
    }
}
