using System;

namespace EmailServices
{
    public class EmailConfiguration
    {
        public string From { get; set; }
        public string SmtpServer { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
    }
}
