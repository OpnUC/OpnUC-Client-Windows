using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Threading;

namespace OpnUC_Client_Windows
{
    public class WebAPI
    {

        private string baseUrl = Properties.Settings.Default.ApiUrl;
        private bool isLogin = false;
        private string token = null;
        private DispatcherTimer timer = null;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public WebAPI()
        {

            this.timer = new DispatcherTimer(DispatcherPriority.Normal);
            // 30分ごとに実行
            this.timer.Interval = new TimeSpan(0, 30, 0);
            this.timer.Tick += DispatcherTimer_Tick;
            this.timer.Start();

            this.login(Properties.Settings.Default.username, Properties.Settings.Default.password);

        }

        /// <summary>
        /// Token Refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {

            if (this.isLogin)
            {
                this.onAuthRefresh();
                logger.Debug("JWT Refresh");
            }

        }

        /// <summary>
        /// ログイン
        /// </summary>
        /// <param name="username">ユーザ名</param>
        /// <param name="password">パスワード</param>
        /// <returns></returns>
        public bool login(string username, string password)
        {

            if (string.IsNullOrEmpty(username))
            {
                new ArgumentNullException("username", "ユーザ名が設定されていません");
            }

            if (string.IsNullOrEmpty(password))
            {
                new ArgumentNullException("password", "パスワードが設定されていません");
            }

            string endpoint = this.baseUrl + "/auth/login";

            WebClient wc = new WebClient();

            var ps = new System.Collections.Specialized.NameValueCollection();
            // 送信するデータ（フィールド名と値の組み合わせ）を追加
            ps.Add("username", username);
            ps.Add("password", password);

            var response = wc.UploadValues(endpoint, ps);
            string resText = System.Text.Encoding.UTF8.GetString(response);

            if (wc.ResponseHeaders.AllKeys.Any(c => c == "Authorization"))
            {
                this.token = wc.ResponseHeaders.Get("Authorization");
            }

            this.isLogin = true;

            return this.isLogin;

        }

        /// <summary>
        /// Tokenのリフレッシュ
        /// </summary>
        /// <returns></returns>
        public bool onAuthRefresh()
        {

            return this._callApi("/auth/refresh");

        }

        /// <summary>
        /// 発信処理
        /// </summary>
        /// <param name="telNumber">電話番号</param>
        /// <returns></returns>
        public bool onCall(string telNumber)
        {

            object param = new
            {
                number = telNumber
            };

            return this._callApi("/click2call/originate", param);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private bool _callApi(string endpoint, object param = null)
        {

            if (!this.isLogin)
            {
                this.login(Properties.Settings.Default.username, Properties.Settings.Default.password);
            }

            WebClient wc = new WebClient();

            wc.Headers["Content-Type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer :" + this.token;

            string stringParam = "";

            // パラメタがある場合、JSONに変換
            if (param != null)
            {
                stringParam = JsonConvert.SerializeObject(param);
            }

            try
            {
                string response = wc.UploadString(this.baseUrl + endpoint, "POST", stringParam);

                if (wc.ResponseHeaders.AllKeys.Any(c => c == "Authorization"))
                {
                    this.token = wc.ResponseHeaders.Get("Authorization");
                }
            }
            catch (WebException e)
            {
                StreamReader streamReader = new StreamReader(e.Response.GetResponseStream(), true);

                try
                {
                    var target = streamReader.ReadToEnd();
                }
                finally
                {
                    streamReader.Close();
                }
            }

            return true;
        }

    }
}
