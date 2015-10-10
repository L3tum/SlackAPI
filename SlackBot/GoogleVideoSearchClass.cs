

namespace VideoGoogle
{
    public class Rootobject
    {
        public Responsedata responseData { get; set; }
        public object responseDetails { get; set; }
        public int responseStatus { get; set; }
    }

    public class Responsedata
    {
        public Result[] results { get; set; }
        public Cursor cursor { get; set; }
    }

    public class Cursor
    {
        public Page[] pages { get; set; }
        public string estimatedResultCount { get; set; }
        public int currentPageIndex { get; set; }
        public string moreResultsUrl { get; set; }
    }

    public class Page
    {
        public string start { get; set; }
        public int label { get; set; }
    }

    public class Result
    {
        public string GsearchResultClass { get; set; }
        public string title { get; set; }
        public string titleNoFormatting { get; set; }
        public string published { get; set; }
        public string content { get; set; }
        public string publisher { get; set; }
        public string tbUrl { get; set; }
        public string tbWidth { get; set; }
        public string tbHeight { get; set; }
        public string videoType { get; set; }
        public string url { get; set; }
        public string playUrl { get; set; }
        public string duration { get; set; }
    }
}
