using SdtdServerKit.FunctionSettings;
using SdtdServerKit.Hooks;
using SdtdServerKit.Managers;
using SdtdServerKit.Variables;

namespace SdtdServerKit.Functions
{
    /// <summary>
    /// Function base class
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public abstract class FunctionBase<TSettings> : IFunction where TSettings : ISettings
    {
        /// <summary>
        /// Settings
        /// </summary>
        public TSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    throw new InvalidOperationException($"The settings for function '{_functionName}' is null!");
                }

                return _settings;
            }
        }

        private readonly string _functionName;
        private bool _isRunning;
        private TSettings? _settings;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Function base constructor
        /// </summary>
        public FunctionBase()
        {
            _functionName = this.GetType().Name;
        }

        void IFunction.LoadSettings()
        {
            _settings = ConfigManager.Get<TSettings>();
            if (_settings != null)
            {
                OnSettingsChanged(_settings);
            }
            ConfigManager.SettingsChanged += OnSettingsChanged;
        }

        /// <summary>
        /// 功能名称
        /// </summary>
        public string Name => _functionName;

        /// <summary>
        /// 格式化命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        protected virtual string FormatCmd(string cmd, IPlayerBase player)
        {
            return StringTemplate.Render(cmd, new VariablesBase()
            {
                PlayerId = player.PlayerId,
                PlayerName = player.PlayerName,
                EntityId = player.EntityId
            });
        }

        /// <summary>
        /// 捕获玩家聊天消息时调用，返回true表示该消息由当前函数处理
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="managedPlayer"></param>
        /// <returns></returns>
        protected virtual Task<bool> OnChatCmd(string cmd, ManagedPlayer managedPlayer)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Disabled function
        /// </summary>
        protected virtual void OnDisableFunction()
        {
            ChatMessageHook.RemoveHook(OnChatCmd);
        }

        /// <summary>
        /// Enabled function
        /// </summary>
        protected virtual void OnEnableFunction()
        {
            ChatMessageHook.AddHook(OnChatCmd);
        }

        ///// <summary>
        ///// Enabled function, 无论服务器上是否有玩家, 返回值将设置到 _isRunning
        ///// </summary>
        //protected virtual bool OnEnableFunctionNonePlayer()
        //{
        //    return false;
        //}

        /// <summary>
        /// If the settings are changed, update the settings and call the protected OnSettingsChanged method
        /// </summary>
        /// <param name="settings"></param>
        private void OnSettingsChanged(ISettings settings)
        {
            if (settings is TSettings changedSettings)
            {
                lock (this)
                {
                    _settings = changedSettings;
                    if (_settings.IsEnabled)
                    {
                        PrivateEnableFunction();
                    }
                    else
                    {
                        PrivateDisableFunction();
                    }
                    OnSettingsChanged();
                }
            }
        }

        /// <summary>
        /// 配置改变时调用, 功能初始化时将自动调用一次
        /// </summary>
        protected virtual void OnSettingsChanged()
        {
        }

        /// <summary>
        /// 发送全局消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected void SendGlobalMessage(string message)
        {
            Utils.SendGlobalMessage(new GlobalMessage()
            {
                Message = message,
                SenderName = ConfigManager.GlobalSettings.GlobalServerName
            });
        }

        /// <summary>
        /// 发送私聊消息
        /// </summary>
        /// <param name="playerIdOrName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected void SendMessageToPlayer(string playerIdOrName, string message)
        {
            Utils.SendPrivateMessage(new PrivateMessage()
            {
                TargetPlayerIdOrName = playerIdOrName,
                Message = message,
                SenderName = ConfigManager.GlobalSettings.WhisperServerName
            });
        }

        /// <summary>
        /// Prevent duplicate settings
        /// </summary>
        private void PrivateDisableFunction()
        {
            // If the function is not disabled
            if (_isRunning)
            {
                _isRunning = false;
                OnDisableFunction();
            }
        }

        /// <summary>
        /// Prevent duplicate settings
        /// </summary>
        private void PrivateEnableFunction()
        {
            // If the function is not running.
            if (_isRunning == false)
            {
                //_isRunning = OnEnableFunctionNonePlayer();

                // Only there are players on the server.
                //if (ConnectionManager.Instance.Clients.Count > 0)
                {
                    _isRunning = true;
                    OnEnableFunction();
                }
            }
        }
    }
}