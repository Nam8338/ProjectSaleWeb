using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace EmailServices
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;       //Lữu trữ cấu hình email
                                                                // khai báo _emailConfig
                                                                //readonly: giá trị trường được thiết lập duy nhất và không thể thay đổi sau đó
        public EmailSender(EmailConfiguration emailConfig)      
        {
            _emailConfig = emailConfig;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message); //Phương thức CreateEmailMessage cấu hình thông điệp email, người gửi, người nhận, tiêu đề và nội dung

            Send(emailMessage);     //kết nối đến máy chủ SMTP và gửi thông điệp mail
        }

        public async Task SendEmailAsync(Message message)       //Lập trình bất đồng bộ. Có dùng Task
        {
            var emailMessage = CreateEmailMessage(message);

            await SendAsync(emailMessage);      //await: cho biết phương thức đang được đợi, cho phép các hoạt động bất đồng bộ khác tiếp tục trong khi đợi email được gửi đi.
        }

        private MimeMessage CreateEmailMessage(Message message)     //Phương thức CreateEmailMessage để tạo ra đối tượng MimeMessage
        {
            var emailMessage = new MimeMessage();   //Khởi tạo đối tượng Mime chứa thông tin về email
            emailMessage.From.Add(new MailboxAddress(_emailConfig.From));   //thiết lập địa chỉ mail cho người gửi, được lấy từ _emailConfig.From
            emailMessage.To.AddRange(message.To);   //thêm danh sách các địa chỉ email người nhận vào Mime
            emailMessage.Subject = message.Subject; //thêm danh sách các địa chỉ email người nhận vào Mime

            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<h3 style='color:red'>{0}</h3>", message.Content) };          
            emailMessage.Body = bodyBuilder.ToMessageBody(); //khởi tạo một đối tượng BodyBuilder để xây dựng nội dung của email. Trong trường hợp này, nội dung được đặt là một chuỗi HTML với một đoạn văn bản <h3> màu đỏ, và giá trị {0} được thay thế bằng nội dung được truyền vào từ thuộc tính Content của đối tượng Message.
            return emailMessage;
        }

        private void Send (MimeMessage mailMessage)     //Khởi tạo phương thức Send
        {
            using (var client  = new SmtpClient())  //Khởi tạo đối tượng SmtpClient
            {
                client.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls |            //Cấu hình các giao thức SSL được hỗ trợ
                    SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;       //Điều này đảm bảo rằng kết nối với máy chủ SMTP sẽ sử dụng các giao thức bảo mật.
                client.CheckCertificateRevocation = false;      //vô hiệu hóa kiểm tra thu hồi chứng chỉ, điều này cho phép máy khách (client) kết nối với máy chủ SMTP mà không kiểm tra trạng thái thu hồi chứng chỉ của chứng chỉ SSL.
                try
                {
                    client.Connect(_emailConfig.SmtpServer,_emailConfig.Port, true);    //thiết lập kết nối với máy chủ SMTP, sử dụng thông tin cấu hình Server & Port
                    client.AuthenticationMechanisms.Remove("XOAUTH2");  //loại bỏ cơ chế xác thực "XOAUTH2" nếu nó được sử dụng
                    client.Authenticate(_emailConfig.Mail, _emailConfig.Password);  //xác thực với tài khoản email sử dụng các thông tin: Email & Password

                    client.Send(mailMessage);   // Gửi email 
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        private async Task SendAsync(MimeMessage mailMessage)   //Phương thức SendAsync
        {
            using (var client = new SmtpClient())
            {
                client.SslProtocols = SslProtocols.Ssl3 | SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12 | SslProtocols.Tls13;
                client.CheckCertificateRevocation = false;
                try
                {
                    await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");              // cho phép người dùng truy cập vào tài khoản email mà không cần cung cấp mật khẩu trực tiếp cho ứng dụng.
                                                                                    // Thay vì sử dụng mật khẩu, phương thức XOAUTH2 sử dụng một mã thông báo truy cập (access token) được phát hành bởi một dịch vụ xác thực bên thứ ba (như Google, Microsoft) để xác thực người dùng.
                                                                                    //Truy cập thông qua bên thứ 3: mã gửi xác thực đến mail or google
                    await client.AuthenticateAsync(_emailConfig.Mail, _emailConfig.Password);

                    await client.SendAsync(mailMessage);
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
