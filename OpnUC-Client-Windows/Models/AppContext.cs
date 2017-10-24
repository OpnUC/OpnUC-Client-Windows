using Microsoft.Practices.Prism.Mvvm;
using OpnUC_Client_Windows.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace OpnUC_Client_Windows.Models
{
    public class AppContext : BindableBase
    {

        /// <summary>
        /// 唯一のModelのインスタンス
        /// </summary>
        public static readonly AppContext Instance = new AppContext();

        /// <summary>
        /// Model間で連携するためのパイプ
        /// </summary>
        private readonly Subject<object> interaction = new Subject<object>();

        public WebAPI WebAPI { get; private set; }
        public WebSocket WebSocket { get; private set; }

        /// <summary>
        /// 閲覧と追加を管理する
        /// </summary>
        //public PeopleMaster Master { get; private set; }

        /// <summary>
        /// 単一項目の編集担当
        /// </summary>
        //public PersonDetail Detail { get; private set; }

        public AppContext()
        {
            this.WebAPI = new WebAPI();
            this.WebSocket = new WebSocket();

            //this.Master = new PeopleMaster(this.interaction);
            //this.Detail = new PersonDetail(this.interaction);
        }

    }
}
