using System.Web;
using System.Web.Mvc;

namespace AspNetLab.Helpers
{
    public static class Extensions
    {
        private static MarkdownHelper _markdownHelper;

        static Extensions()
        {
            _markdownHelper = new MarkdownHelper();
        }
        public static HtmlString MarkDown(this HtmlHelper helper, string markdownFileName)
        {
            return new MvcHtmlString(_markdownHelper.GetMarkdownContent(markdownFileName));
        }
    }
}