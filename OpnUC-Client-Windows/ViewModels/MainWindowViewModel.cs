using System.Linq;
using OpnUC_Client_Windows.Models;
using Reactive.Bindings;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;
using System;

namespace OpnUC_Client_Windows.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly Models.AppContext Model = Models.AppContext.Instance;

        public ReactiveCollection<AddressBook> AddressBook { get; private set; }

        public ReactiveProperty<bool> isLogin { get; private set; }
        public ReactiveProperty<Models.UserData> userData { get; private set; }

        public ReactiveProperty<string> searchKeyword { get; private set; }

        public IDialogCoordinator dialogCoordinator { get; set; }

        public ReactiveCommand LoginCommand { get; private set; }
        //public ReactiveCommand LogoutCommand { get; private set; }
        //public ReactiveCommand CloseCommand { get; private set; }
        //public ReactiveCommand ExitCommand { get; private set; }

        public ReactiveCommand SearchCommand { get; private set; }

        public ReactiveCommand onCallCommand { get; private set; }

        public MainWindowViewModel()
        {

            this.AddressBook = new ReactiveCollection<Models.AddressBook>();

            this.userData = new ReactiveProperty<Models.UserData>();
            this.isLogin = new ReactiveProperty<bool>(this.Model.WebAPI.isLogin);

            this.searchKeyword = new ReactiveProperty<string>();

            this.LoginCommand = new ReactiveCommand();
            this.LoginCommand.Subscribe(_ => this.login());

            //this.LogoutCommand = new ReactiveCommand();
            //this.LogoutCommand.Subscribe();

            //this.CloseCommand = new ReactiveCommand();
            //this.CloseCommand.Subscribe();

            //this.ExitCommand = new ReactiveCommand();
            //this.ExitCommand.Subscribe();

            this.SearchCommand = new ReactiveCommand();
            this.SearchCommand.Subscribe(_ =>
                this.fetchAddressBook()
            );

            this.onCallCommand = new ReactiveCommand();
            this.onCallCommand.Subscribe(_ =>
                Library.SharedFnc.OnCall(_.ToString())
            );

            if (!this.isLogin.Value)
            {
                // ログインしていない場合はログインする
                this.login();
            }

            this.InitDataLoad();

            this.Model.WebSocket.UpdatePresenceEvent += WebSocket_MyProgressEvent;
        }

        /// <summary>
        /// 初期データ取得
        /// </summary>
        private async void InitDataLoad()
        {

            // ユーザ情報取得
            this.userData.Value = await this.Model.WebAPI.getUserInfo();

            this.fetchAddressBook();

        }

        /// <summary>
        /// WebSocketでプレゼンス情報が更新された場合
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>
        /// TEL1に限定している
        /// </remarks>
        private void WebSocket_MyProgressEvent(Library.WebSocket.UpdatePresenceEventArgs e)
        {

            var items = this.AddressBook.Where(c => c.tel1 == e.ext);

            //
            if (items != null)
            {
                foreach (var item in items)
                {
                    item.tel1_status = e.status;
                }
            }

        }

        /// <summary>
        /// ログインダイヤログを表示する
        /// </summary>
        /// <remarks>
        /// パスワードの処理はSecureStringを使った方が良い
        /// パスワードの復号化が出来ないと、Exceptionが発生し落ちる
        /// </remarks>
        private async void login()
        {

            if (string.IsNullOrEmpty(Properties.Settings.Default.username) &&
             string.IsNullOrEmpty(Properties.Settings.Default.password))
            {
                // Login
                LoginDialogData result = await dialogCoordinator.ShowLoginAsync(
                    this,
                    "ログイン",
                    "ユーザ名とパスワードを入力してください。",
                    new LoginDialogSettings
                    {
                        InitialUsername = Properties.Settings.Default.username ?? "",
                        InitialPassword = Library.StringEncryption.DecryptString(Properties.Settings.Default.password) ?? "",
                        EnablePasswordPreview = true,
                        RememberCheckBoxVisibility = Visibility.Visible,
                        RememberCheckBoxText = "ログイン情報を保存し、ログインを維持する"
                    });

                if (result != null)
                {
                    // ログイン情報を保存するか
                    if (result.ShouldRemember)
                    {
                        Properties.Settings.Default.username = result.Username;
                        Properties.Settings.Default.password = Library.StringEncryption.EncryptString(result.Password);
                        Properties.Settings.Default.Save();
                    }
                }
                else
                {
                    // ログイン情報入力無し
                    return;
                }

            }

            // ログイン
            var loginResult = await this.Model.WebAPI.Login(
                Properties.Settings.Default.username,
                Library.StringEncryption.DecryptString(Properties.Settings.Default.password)
                );

            if (!loginResult)
            {
                await dialogCoordinator.ShowMessageAsync(this, "エラー", "ログインできませんでした。");
                return;
            }

            // ログイン情報更新
            this.isLogin.Value = true;

        }

        /// <summary>
        /// アドレス帳情報取得
        /// </summary>
        private async void fetchAddressBook()
        {

            // アドレス帳情報取得
            var x = await this.Model.WebAPI.getAddressBook(this.searchKeyword.Value);

            // 更新
            this.AddressBook.ClearOnScheduler();
            this.AddressBook.AddRangeOnScheduler(x.ToArray());

        }

    }
}
