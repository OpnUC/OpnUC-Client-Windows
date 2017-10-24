using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpnUC_Client_Windows.Library
{
    public class RunAs
    {

        /// <summary>
        /// 管理者権限が必要なプログラムを起動する
        /// </summary>
        /// <param name="fileName">プログラムのフルパス。</param>
        /// <param name="arguments">プログラムに渡すコマンドライン引数。</param>
        /// <param name="parentForm">親プログラムのウィンドウ。</param>
        /// <param name="waitExit">起動したプログラムが終了するまで待機する。</param>
        /// <returns>起動に成功した時はtrue。
        /// 「ユーザーアカウント制御」ダイアログでキャンセルされた時はfalse。</returns>
        public static bool RunElevated(string fileName, string arguments, bool waitExit)
        {

            //プログラムがあるか調べる
            if (!System.IO.File.Exists(fileName))
            {
                throw new System.IO.FileNotFoundException();
            }

            System.Diagnostics.ProcessStartInfo psi =
                new System.Diagnostics.ProcessStartInfo();
            //ShellExecuteを使う。デフォルトtrueなので、必要はない。
            psi.UseShellExecute = true;
            //昇格して実行するプログラムのパスを設定する
            psi.FileName = fileName;
            //動詞に「runas」をつける
            psi.Verb = "runas";
            //子プログラムに渡すコマンドライン引数を設定する
            psi.Arguments = arguments;

            try
            {
                //起動する
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
                if (waitExit)
                {
                    //終了するまで待機する
                    p.WaitForExit();
                }
            }
            catch (System.ComponentModel.Win32Exception)
            {
                //「ユーザーアカウント制御」ダイアログでキャンセルされたなどによって
                //起動できなかった時
                return false;
            }

            return true;
        }

    }
}
