using System;
using System.ComponentModel;
using System.Windows;

namespace OpnUC_Client_Windows
{
    public partial class NotifyIconWrapper : Component
    {
        // https://garafu.blogspot.jp/2015/06/dev-tasktray-residentapplication.html

        public NotifyIconWrapper()
        {
            InitializeComponent();

            // コンテキストメニューのイベントを設定
            this.toolStripMenuItem_Open.Click += this.toolStripMenuItem_Open_Click;
            this.toolStripMenuItem_Exit.Click += this.toolStripMenuItem_Exit_Click;
        }

        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /// <summary>
        /// コンテキストメニュー "表示" 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Open_Click(object sender, EventArgs e)
        {

            // メイン画面を開く
            this.showMainWindow();

        }

        /// <summary>
        /// コンテキストメニュー "終了"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {

            // タスクトレイアイコン アイコンを消してから終了しないとアイコンが残る場合有り
            this.notifyIcon.Visible = false;
       
            // 現在のアプリケーションを終了
            Application.Current.Shutdown();

        }

        /// <summary>
        /// タスクトレイアイコンがダブルクリックされた場合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            // メイン画面を開く
            this.showMainWindow();

        }

        /// <summary>
        /// メイン画面を開く
        /// </summary>
        private void showMainWindow()
        {

            // メイン画面が存在するか
            if (Application.Current.MainWindow == null)
            {
                // MainWindow を生成、表示
                var wnd = new MainWindow();
                wnd.Show();
            }
            else
            {
                Application.Current.MainWindow.Show();
                Application.Current.MainWindow.Activate();
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }

        }
    }
}
