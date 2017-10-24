using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.Prism.Mvvm;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpnUC_Client_Windows.ViewModels
{
    public class OptionsViewModel
    {

        private readonly Models.AppContext Model = Models.AppContext.Instance;
        public IDialogCoordinator dialogCoordinator { get; set; }

        public ReactiveProperty<string> username { get; private set; }
        public ReactiveProperty<string> password { get; private set; }

        public ReactiveProperty<string> ApiUrl { get; private set; }
        public ReactiveProperty<string> WebSocketUrl { get; private set; }

        public ReactiveCommand SaveCommand { get; private set; }

        public OptionsViewModel()
        {

            this.username = new ReactiveProperty<string>(Properties.Settings.Default.username);
            this.password = new ReactiveProperty<string>(Library.StringEncryption.DecryptString(Properties.Settings.Default.password));

            this.ApiUrl = new ReactiveProperty<string>(Properties.Settings.Default.ApiUrl);
            this.WebSocketUrl = new ReactiveProperty<string>(Properties.Settings.Default.WebSocketUrl);

            this.SaveCommand = new ReactiveCommand();

            this.SaveCommand.Subscribe(_ =>
                this.Save()
            );

        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save()
        {

            Properties.Settings.Default.username = this.username.Value;
            Properties.Settings.Default.password = Library.StringEncryption.EncryptString(this.password.Value);

            Properties.Settings.Default.WebSocketUrl = this.ApiUrl.Value;
            Properties.Settings.Default.WebSocketUrl = this.WebSocketUrl.Value;

            Properties.Settings.Default.Save();

            dialogCoordinator?.ShowMessageAsync(this, "情報", "保存が完了しました");

        }

    }


}
