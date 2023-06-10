using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailServices
{

    public interface IEmailSender
    {
        void SendEmail(Message message);    //sử dụng để gửi email đồng bộ
        Task SendEmailAsync(Message message);   //Phương thức này trả về một đối tượng Task,
                                                //cho phép việc gửi email được thực hiện một cách bất đồng bộ và không làm tắc nghẽn luồng chính của ứng dụng.
    }
}
