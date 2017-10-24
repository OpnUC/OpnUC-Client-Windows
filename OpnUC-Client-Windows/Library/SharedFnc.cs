using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace OpnUC_Client_Windows.Library
{
    public static class SharedFnc
    {
        private static readonly Models.AppContext Model = Models.AppContext.Instance;

        /// <summary>
        /// 発信処理
        /// </summary>
        /// <param name="telNumber">電話番号</param>
        public static async void OnCall(string telNumber)
        {

            // 電話番号から数字以外を削除
            telNumber = new Regex(@"[^0-9]").Replace(telNumber, "");

            if (MessageBox.Show(telNumber + "に発信します。よろしいですか？",
                "確認", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                await Model.WebAPI.onCall(telNumber);
            }

        }

    }
}
