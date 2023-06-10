using System;

namespace EmailServices
{
    //Cấu hình Email: chứa các thông tin về Email
    public class EmailConfiguration
    {
        public string From { get; set; }    //Người gửi
        public string SmtpServer { get; set; }  //địa chỉ máy chủ SMTP
        public string Mail { get; set; }    //Địa chỉ mail người nhận
        public string Password { get; set; }
        public int Port { get; set; }   //Cổng kết nối
    }
}
