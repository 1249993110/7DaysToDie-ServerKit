﻿using IceCoffee.SimpleCRUD.OptionalAttributes;

namespace SdtdServerKit.Data.Entities
{
    public class T_ChatRecord
    {
        [PrimaryKey, IgnoreUpdate, IgnoreInsert]
        public int Id { get; set; }

        [IgnoreUpdate]
        public DateTime CreatedAt { get; set; }

        public int EntityId { get; set; }
        public string? PlayerId { get; set; }

        public required string SenderName { get; set; }

        public ChatType ChatType { get; set; }

        public required string Message { get; set; }
    }
}