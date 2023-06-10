using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailServices
{
    //Thông điệp email (tin nhắn)
    public class Message
    {
        public List<MailboxAddress> To { get; set; }    //Danh sách các địa chỉ Email người nhận
                                                        //Mỗi địa chỉ Email được biểu diễn bởi 1 đối tượng MailboxAdress trong thư viện MimeKit
                                                        //Các địa chỉ Email sẽ được lưu trữ trong List<>
        public string Subject { get; set; }    //Subject: tiêu đề của Mail
        public string Content { get; set; }    //Content: nội dung của mail

        public Message(IEnumerable<string> to,string subject,string content)    //IEnumerable<string> to:
                                                                                //to: đại diện cho danh sách các địa chỉ email của người nhận
                                                                                //Sử dụng IEnumerable<string>: truyền 1 danh sách các địa chỉ email kiểu 'string' vào Phương thức khởi tạo Message
        {
            To = new List<MailboxAddress>();    // Khởi tạo danh sách To (người nhận)
            To.AddRange(to.Select(x => new MailboxAddress(x))); //thêm các đối tượng MailboxAddress đã được tạo ra từ danh sách to vào danh sách To
            Subject = subject;
            Content = content;
        }
    }

    //IEnumerable<string> là một giao diện trong .NET Framework,
    //cho phép lặp qua một tập hợp các phần tử của kiểu dữ liệu string.
    //Nó đại diện cho một chuỗi các đối tượng 'string' và cung cấp các phương thức để duyệt qua từng phần tử trong tập hợp đó.
}
