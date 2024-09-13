﻿using IceCoffee.SimpleCRUD.OptionalAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdtdServerKit.Models
{
    /// <summary>
    /// Colored Chat Model
    /// </summary>
    public class ColoredChat
    {
        /// <summary>
        /// Player Id
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// Name Color
        /// </summary>
        public string? NameColor { get; set; }

        /// <summary>
        /// Text Color
        /// </summary>
        public string? TextColor { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string? Description { get; set; }
    }
}
