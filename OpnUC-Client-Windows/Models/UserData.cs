using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace OpnUC_Client_Windows.Models
{
    public class UserData : BindableBase
    {

        private int _id;

        public int id
        {
            get
            {
                return this._id;

            }
            set
            {
                this.SetProperty(ref this._id, value);
            }
        }

        private string _username;

        public string username
        {
            get
            {
                return this._username;
            }
            set
            {
                this.SetProperty(ref this._username, value);
            }
        }

        private string _display_name;

        public string display_name
        {
            get
            {
                return this._display_name;
            }
            set
            {
                this.SetProperty(ref this._display_name, value);
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

        private int _avatar_type;

        public int avatar_type
        {
            get
            {
                return this._avatar_type;
            }
            set
            {
                this.SetProperty(ref this._avatar_type, value);
            }
        }

        private string _avatar_filename;

        public string avatar_filename
        {
            get
            {
                return this._avatar_filename;
            }
            set
            {
                this.SetProperty(ref this._avatar_filename, value);
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

        private AddressBook _address_book;

        public AddressBook address_book
        {
            get
            {
                return this._address_book;
            }
            set
            {
                this.SetProperty(ref this._address_book, value);
            }
        }

        private List<int> _roles;

        public List<int> roles
        {
            get
            {
                return this._roles;
            }
            set
            {
                this.SetProperty(ref this._roles, value);
            }
        }

        private List<string> _permissions;

        public List<string> permissions
        {
            get
            {
                return this._permissions;
            }
            set
            {
                this.SetProperty(ref this._permissions, value);
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

        public virtual BitmapImage avatar
        {
            get
            {
                return new BitmapImage(new Uri(this.avatar_path));
            }
        }
    }
}
