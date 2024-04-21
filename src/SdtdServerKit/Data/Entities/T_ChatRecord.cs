using IceCoffee.SimpleCRUD.OptionalAttributes;

namespace SdtdServerKit.Data.Entities
{
    public class T_ChatRecord
    {
        [PrimaryKey, IgnoreUpdate, IgnoreInsert]
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string PlayerId { get; set; }

        public string SenderName { get; set; }

        public ChatType ChatType { get; set; }

        public string Message { get; set; }
    }
}