using System.Web;
using System.Web.Optimization;

namespace CostaDiamante_HOA
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            /*SCRIPTS BUNDLES*/
            bundles.Add(new ScriptBundle("~/bundles/template").Include(
                        "~/Scripts/skel.min.js",
                        "~/Scripts/util.js",
                        "~/Scripts/main.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.mask.js",
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/dataTables.buttons.min.js",
                        "~/Scripts/moment.min.js",
                        "~/Scripts/datetime-moment.js",
                        "~/Scripts/numeral.min.js",
                        "~/Scripts/sweetalert.min.js",
                        "~/Scripts/notify.min.js",
                        "~/Scripts/custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/signalR").Include(
                        "~/Scripts/jquery.signalR-2.2.3.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-datatables-reports").Include(
                        "~/Scripts/buttons.flash.min.js",
                        "~/Scripts/buttons.html5.min.js",
                        "~/Scripts/buttons.print.min.js",
                        "~/Scripts/jszip.min.js",
                        "~/Scripts/pdfmake.min.js",
                        "~/Scripts/vfs_fonts.js"));

            bundles.Add(new ScriptBundle("~/bundles/vue-axios").Include(
                        "~/Scripts/vue.min.js",
                        "~/Scripts/axios.min.js",
                        "~/Scripts/vue-tables-2.min.js",
                        "~/Scripts/v-money.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            /*STYLE BUNDLES*/
            bundles.Add(new StyleBundle("~/Content/template").Include(
                      "~/Content/main.css",
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/buttons.dataTables.min.css"));

            bundles.Add(new StyleBundle("~/Content/icons").Include(
                      "~/Content/font-awesome.min.css",
                      "~/Content/icomoon.css"));

            bundles.Add(new StyleBundle("~/Content/jqueryui").Include(
                      "~/Content/themes/base/*.css"));
        }
    }
}
