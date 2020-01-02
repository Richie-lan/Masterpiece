using System.Web;
using System.Web.Optimization;

namespace Masterpiece.Web
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/Scripts/vue").Include(
                      "~/Scripts/vue.js"));

            //element-ui css，js
            bundles.Add(new StyleBundle("~/Content/elementui").Include(
                        "~/Content/elementui.css"));

            bundles.Add(new ScriptBundle("~/Scripts/elementui").Include(
                      "~/Scripts/elementui.js"));
        }
    }
}
