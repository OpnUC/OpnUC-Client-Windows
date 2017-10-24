using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace OpnUC_Client_Windows.Library
{
    public class WebAPI
    {

        /// <summary>
        /// ログイン状態
        /// </summary>
        public bool isLogin
        {
            get
            {
                return this._isLogin;
            }
        }

        public string token
        {
            get
            {
                return this._token;
            }
        }

        private string baseUrl = Properties.Settings.Default.ApiUrl;
        private bool _isLogin = false;
        private string _token = null;
        private DispatcherTimer timer = null;
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public WebAPI()
        {

            ServicePointManager.Expect100Continue = false; // HTTPエラー(417)対応

            this.timer = new DispatcherTimer(DispatcherPriority.Normal);
            // 30分ごとに実行
            this.timer.Interval = new TimeSpan(0, 30, 0);
            this.timer.Tick += DispatcherTimer_Tick;
            this.timer.Start();

        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~WebAPI()
        {

            if (this.timer != null && this.timer.IsEnabled)
            {
                this.timer.Stop();
            }

        }

        /// <summary>
        /// Token Refresh
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void DispatcherTimer_Tick(object sender, EventArgs e)
        {

            if (this._isLogin)
            {
                await this.onAuthRefresh();
                logger.Debug("JWT Refresh");
            }

        }

        /// <summary>
        /// ログイン
        /// </summary>
        /// <param name="username">ユーザ名</param>
        /// <param name="password">パスワード</param>
        /// <returns></returns>
        public async Task<bool> Login(string username, string password)
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

            using (WebClient wc = new WebClient())
            {
                var ps = new System.Collections.Specialized.NameValueCollection();
                // 送信するデータ（フィールド名と値の組み合わせ）を追加
                ps.Add("username", username);
                ps.Add("password", password);

                byte[] response = null;

                try
                {
                    response = await wc.UploadValuesTaskAsync(endpoint, ps);
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        System.Net.HttpWebResponse errors = (System.Net.HttpWebResponse)ex.Response;

                        if (errors.StatusCode == HttpStatusCode.BadRequest)
                        {
                            return this._isLogin;
                        }
                    }
                    throw;
                }

                string resText = System.Text.Encoding.UTF8.GetString(response);

                if (wc.ResponseHeaders.AllKeys.Any(c => c == "Authorization"))
                {
                    this._token = wc.ResponseHeaders.Get("Authorization");
                }
            }

            this._isLogin = true;

            return this._isLogin;

        }

        /// <summary>
        /// ユーザ情報取得
        /// </summary>
        /// <returns></returns>
        public async Task<Models.UserData> getUserInfo()
        {

            var jsonString = await this._callApi("/auth/user", "GET");

            var dataObject = (JObject)jsonString.GetValue("data");

            return JsonConvert.DeserializeObject<Models.UserData>(dataObject.ToString());

        }

        /// <summary>
        /// アドレス帳情報取得
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Models.AddressBook>> getAddressBook(string Keyword = "")
        {

            var param = new NameValueCollection
            {
                { "sort", "name_kana|asc" },
                { "page", "1" },
                { "per_page", "65535" },
                { "typeId", "1" },
                { "groupId", "0" },
                { "keyword", Keyword },
            };

            var jsonString = await this._callApi("/addressbook/search", "GET", param);

            var dataObject = jsonString.GetValue("data");

            return JsonConvert.DeserializeObject<IEnumerable<Models.AddressBook>>(dataObject.ToString());

        }

        /// <summary>
        /// Tokenのリフレッシュ
        /// </summary>
        /// <returns></returns>
        public async Task<object> onAuthRefresh()
        {

            return await this._callApi("/auth/refresh", "GET");

        }

        /// <summary>
        /// 発信処理
        /// </summary>
        /// <param name="telNumber">電話番号</param>
        /// <returns></returns>
        public async Task<object> onCall(string telNumber)
        {

            var param = new NameValueCollection
            {
                { "number", telNumber }
            };

            return await this._callApi("/click2call/originate", "POST", param);

        }

        /// <summary>
        /// APIを呼び出す
        /// </summary>
        /// <param name="endpoint">リクエストURL</param>
        /// <param name="method">メソッド(GET/POST)</param>
        /// <param name="param">パラメータ</param>
        /// <returns></returns>
        private async Task<JObject> _callApi(string endpoint, string method = "post", NameValueCollection param = null)
        {

            if (!this._isLogin)
            {
                await this.Login(Properties.Settings.Default.username, Properties.Settings.Default.password);
            }

            WebClient wc = new WebClient();

            wc.Headers["Content-Type"] = "application/json";
            wc.Headers["Authorization"] = "Bearer :" + this._token;

            string response = string.Empty;

            try
            {

                if (method.ToUpper() == "GET")
                {
                    // パラメタがある場合、JSONに変換
                    if (param != null)
                    {
                        foreach (string key in param)
                        {
                            wc.QueryString.Add(key, param[key]);
                        }
                    }

                    response = await wc.DownloadStringTaskAsync(this.baseUrl + endpoint);
                }
                else
                {
                    string stringParam = "";

                    // パラメタがある場合、JSONに変換
                    if (param != null)
                    {
                        stringParam = JsonConvert.SerializeObject(NvcToDictionary(param));
                    }

                    response = await wc.UploadStringTaskAsync(this.baseUrl + endpoint, method, stringParam);
                }

                if (wc.ResponseHeaders.AllKeys.Any(c => c == "Authorization"))
                {
                    this._token = wc.ResponseHeaders.Get("Authorization");
                }
            }
            catch (WebException e)
            {
                logger.Fatal("Fatal Error", e);

                throw;
            }

            JObject result = null;

            try
            {
                result = (JObject)JsonConvert.DeserializeObject(response);
            }
            catch (Exception e)
            {
                logger.Fatal(e, "Json Deserialize Error");
                throw;
            }

            return result;
        }

        /// <summary>
        /// NameValueCollectionをDictionaryに変換する
        /// </summary>
        /// <param name="nvc"></param>
        /// <returns></returns>
        private static Dictionary<string, object> NvcToDictionary(NameValueCollection nvc)
        {
            var result = new Dictionary<string, object>();
            foreach (string key in nvc.Keys)
            {
                result.Add(key, nvc[key]);
            }

            return result;
        }

    }

}
