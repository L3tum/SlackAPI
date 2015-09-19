using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace SlackAPI
{
    public class User
    {
        public String ID;
        public String Name;
        public String Username;
        public bool deleted;
        public String HexColor;
        public bool is_Admin;
        public bool is_Owner;
        public bool is_primaryOwner;
        public bool is_restricted;
        public bool is_ultraRestricted;
        public bool has_2fa;
        public bool has_files;

        public User(String ID, String Name, String Username, bool deleted, String HexColor, bool is_Admin, bool is_Owner,
            bool is_primaryOwner, bool is_restricted, bool is_ultraRestricted, bool has_2fa, bool has_files)
        {
            this.ID = ID;
            this.Name = Name;
            this.Username = Username;
            this.deleted = deleted;
            this.HexColor = HexColor;
            this.is_Admin = is_Admin;
            this.is_Owner = is_Owner;
            this.is_primaryOwner = is_primaryOwner;
            this.is_restricted = is_restricted;
            this.is_ultraRestricted = is_ultraRestricted;
            this.has_2fa = has_2fa;
            this.has_files = has_files;
        }
    }
}