
using System.Web.Optimization;

namespace HalloDoc_Project.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // CSS bundling and minification
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                      "~/Content/site.css"));

            // JS bundling and minification
            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/Scripts/jquery.js",
                      "~/Scripts/bootstrap.js"));

            // Enable minification  
            BundleTable.EnableOptimizations = true;
        }
    }
}