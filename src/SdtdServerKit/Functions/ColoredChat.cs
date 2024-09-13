using SdtdServerKit.Data.IRepositories;
using SdtdServerKit.FunctionSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdtdServerKit.Functions
{
    /// <summary>
    /// Colored Chat Function
    /// </summary>
    public class ColoredChat : FunctionBase<ColoredChatSettings>
    {
        private readonly IColoredChatRepository _coloredChatRepository;

        /// <inheritdoc/>
        public ColoredChat(IColoredChatRepository coloredChatRepository)
        {
            _coloredChatRepository = coloredChatRepository;
        }

        
    }
}
