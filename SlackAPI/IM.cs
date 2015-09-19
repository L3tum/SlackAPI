using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlackAPI
{
    public class IM
    {
        public string id { get; set; }
        public bool is_im { get; set; }
        public string user { get; set; }
        public int created { get; set; }
        public bool is_user_deleted { get; set; }

        public IM(String ID, bool is_im, String user, int created, bool is_user_deleted)
        {
            this.id = ID;
            this.is_im = is_im;
            this.user = user;
            this.created = created;
            this.is_user_deleted = is_user_deleted;
        }
    }
}