using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OpnUC_Client_Windows.Models
{
    public class AddressBook : BindableBase
    {

        private int _id;

        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                this.SetProperty(ref this._id, value);
            }
        }

        private int _type;

        public int type
        {
            get
            {
                return this._type;
            }
            set
            {
                this.SetProperty(ref this._type, value);
            }
        }

        private int _owner_userid;

        public int owner_userid
        {
            get
            {
                return this._owner_userid;
            }
            set
            {
                this.SetProperty(ref this._owner_userid, value);
            }
        }

        private int _group_id;

        public int groupid
        {
            get
            {
                return this._group_id;
            }
            set
            {
                this.SetProperty(ref this._group_id, value);
            }
        }

        private string _position;

        public string position
        {
            get
            {
                return this._position;
            }
            set
            {
                this.SetProperty(ref this._position, value);
            }
        }

        private string _name_kana;

        public string name_kana
        {
            get
            {
                return this._name_kana;
            }
            set
            {
                this.SetProperty(ref this._name_kana, value);
            }
        }

        private string _name;

        public string name
        {
            get
            {
                return this._name;
            }
            set
            {
                this.SetProperty(ref this._name, value);
            }
        }

        private string _tel1;

        public string tel1
        {
            get
            {
                return this._tel1;
            }
            set
            {
                this.SetProperty(ref this._tel1, value);
            }
        }

        private string _tel2;

        public string tel2
        {
            get
            {
                return this._tel2;
            }
            set
            {
                this.SetProperty(ref this._tel2, value);
            }
        }

        private string _tel3;

        public string tel3
        {
            get
            {
                return this._tel3;
            }
            set
            {
                this.SetProperty(ref this._tel3, value);
            }
        }

        private string _email;

        public string email
        {
            get
            {
                return this._email;
            }
            set
            {
                this.SetProperty(ref this._email, value);
            }
        }

        private string _comment;

        public string comment
        {
            get
            {
                return this._comment;
            }
            set
            {
                this.SetProperty(ref this._comment, value);
            }
        }

        private DateTime _created_at;

        public DateTime created_at
        {
            get
            {
                return this._created_at;
            }
            set
            {
                this.SetProperty(ref this._created_at, value);
            }
        }

        private DateTime _updated_at;

        public DateTime updated_at
        {
            get
            {
                return this._updated_at;
            }
            set
            {
                this.SetProperty(ref this._updated_at, value);
            }
        }

        private string _group_name;

        public string group_name
        {
            get
            {
                return this._group_name;
            }
            set
            {
                this.SetProperty(ref this._group_name, value);
            }
        }

        private string _tel1_status;

        public string tel1_status
        {
            get
            {
                return this._tel1_status;
            }
            set
            {
                this.SetProperty(ref this._tel1_status, value);
            }
        }

        private string _tel2_status;

        public string tel2_status
        {
            get
            {
                return this._tel2_status;
            }
            set
            {
                this.SetProperty(ref this._tel2_status, value);
            }
        }

        private string _tel3_status;

        public string tel3_status
        {
            get
            {
                return this._tel3_status;
            }
            set
            {
                this.SetProperty(ref this._tel3_status, value);
            }
        }

        private string _avatar_path;

        public string avatar_path
        {
            get
            {
                return this._avatar_path;
            }
            set
            {
                this.SetProperty(ref this._avatar_path, value);
            }
        }

        /// <summary>
        /// アバターのBitmapImage
        /// </summary>
        /// <remarks>
        /// 毎回、オブジェクトが生成される？
        /// </remarks>
        public virtual BitmapImage avatar
        {
            get
            {
                return new BitmapImage(new Uri(this.avatar_path));
            }
        }

    }
}
