using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SlackAPI
{
    public class Topic
    {
        public string value { get; set; }
        public string creator { get; set; }
        public int last_set { get; set; }

        public Topic(String value, String creator, int last_set)
        {
            this.value = value;
            this.creator = creator;
            this.last_set = last_set;
        }
    }

    public class Purpose
    {
        public string value { get; set; }
        public string creator { get; set; }
        public int last_set { get; set; }

        public Purpose(String value, String creator, int last_set)
        {
            this.value = value;
            this.creator = creator;
            this.last_set = last_set;
        }
    }

    public class Channel
    {
        public string id { get; set; }
        public string name { get; set; }
        public int created { get; set; }
        public string creator { get; set; }
        public bool is_archived { get; set; }
        public bool is_member { get; set; }
        public int num_members { get; set; }
        public Topic topic { get; set; }
        public Purpose purpose { get; set; }

        public Channel(String ID, String name, int created, string creator, bool is_archived, bool is_member,
            int num_members, Topic topic, Purpose purpose)
        {
            this.id = ID;
            this.name = name;
            this.created = created;
            this.creator = creator;
            this.is_archived = is_archived;
            this.is_member = is_member;
            this.num_members = num_members;
            this.topic = topic;
            this.purpose = purpose;
        }
    }

}