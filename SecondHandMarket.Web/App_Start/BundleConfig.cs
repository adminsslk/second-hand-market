using System.Web;
using System.Web.Optimization;

namespace SecondHandMarket.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/modernizr").Include(
                        "~/js/modernizr-*"));

            bundles.Add(new ScriptBundle("~/js").Include(
                        "~/js/jquery-{version}.js",
                        "~/js/jquery.cookie.js",
                        "~/js/bootstrap.min.js",
                        "~/js/bootstrap3-typeahead.js",
                        "~/js/site.js",
                        "~/js/tinymce/tinymce.min.js"
                        ));

            bundles.Add(new StyleBundle("~/css").Include(
                      "~/css/bootstrap.min.css",
                      "~/css/stylish-portfolio.css",
                      "~/css/site.css",
                      "~/css/helpers.css",
                      "~/font-awesome-4.1.0/css/font-awesome.min.css",
                      "~/font-mfizz-1.2/font-mfizz.css"
                      ));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
        }
    }
}
