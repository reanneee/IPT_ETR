#pragma checksum "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "31b5637ab1e58d9fbf6a2db57a5d390a55d458dc"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Admin_Index), @"mvc.1.0.view", @"/Views/Admin/Index.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\_ViewImports.cshtml"
using Ecom;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\_ViewImports.cshtml"
using Ecom.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\_ViewImports.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"31b5637ab1e58d9fbf6a2db57a5d390a55d458dc", @"/Views/Admin/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"8f590f90d6d197def03301032e200ac8485fb047", @"/Views/_ViewImports.cshtml")]
    public class Views_Admin_Index : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 1 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
  
    ViewData["Title"] = "Admin Dashboard";
    var topProducts = ViewBag.TopSellingProducts as List<Ecom.Models.TopSellingProduct>;

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<div class=""container-fluid"">
    <div class=""row"">
        <div class=""col-12"">
            <h1 class=""h3 mb-4 text-gray-800"">Admin Dashboard</h1>
        </div>
    </div>

    <!-- Dashboard Stats Cards -->
    <div class=""row mb-4"">
        <div class=""col-xl-3 col-md-6 mb-4"">
            <div class=""card border-left-primary shadow h-100 py-2"">
                <div class=""card-body"">
                    <div class=""row no-gutters align-items-center"">
                        <div class=""col mr-2"">
                            <div class=""text-xs font-weight-bold text-primary text-uppercase mb-1"">Total Products</div>
                            <div class=""h5 mb-0 font-weight-bold text-gray-800"">");
#nullable restore
#line 21 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                                                           Write(ViewBag.TotalProducts);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</div>
                        </div>
                        <div class=""col-auto"">
                            <i class=""fas fa-box fa-2x text-gray-300""></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class=""col-xl-3 col-md-6 mb-4"">
            <div class=""card border-left-success shadow h-100 py-2"">
                <div class=""card-body"">
                    <div class=""row no-gutters align-items-center"">
                        <div class=""col mr-2"">
                            <div class=""text-xs font-weight-bold text-success text-uppercase mb-1"">Total Orders</div>
                            <div class=""h5 mb-0 font-weight-bold text-gray-800"">");
#nullable restore
#line 37 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                                                           Write(ViewBag.TotalOrders);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</div>
                        </div>
                        <div class=""col-auto"">
                            <i class=""fas fa-shopping-cart fa-2x text-gray-300""></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class=""col-xl-3 col-md-6 mb-4"">
            <div class=""card border-left-info shadow h-100 py-2"">
                <div class=""card-body"">
                    <div class=""row no-gutters align-items-center"">
                        <div class=""col mr-2"">
                            <div class=""text-xs font-weight-bold text-info text-uppercase mb-1"">Total Users</div>
                            <div class=""h5 mb-0 font-weight-bold text-gray-800"">");
#nullable restore
#line 53 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                                                           Write(ViewBag.TotalUsers);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</div>
                        </div>
                        <div class=""col-auto"">
                            <i class=""fas fa-users fa-2x text-gray-300""></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class=""col-xl-3 col-md-6 mb-4"">
            <div class=""card border-left-warning shadow h-100 py-2"">
                <div class=""card-body"">
                    <div class=""row no-gutters align-items-center"">
                        <div class=""col mr-2"">
                            <div class=""text-xs font-weight-bold text-warning text-uppercase mb-1"">Total Revenue</div>
                            <div class=""h5 mb-0 font-weight-bold text-gray-800"">₱");
#nullable restore
#line 69 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                                                            Write(ViewBag.TotalRevenue.ToString("N2"));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</div>
                        </div>
                        <div class=""col-auto"">
                            <i class=""fas fa-dollar-sign fa-2x text-gray-300""></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Top Selling Products Section -->
    <div class=""row"">
        <div class=""col-12"">
            <div class=""card shadow mb-4"">
                <div class=""card-header py-3"">
                    <h6 class=""m-0 font-weight-bold text-primary"">Top 4 Selling Products</h6>
                </div>
                <div class=""card-body"">
");
#nullable restore
#line 88 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                     if (topProducts != null && topProducts.Any())
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <div class=\"row\">\r\n");
#nullable restore
#line 91 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                             foreach (var product in topProducts)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                                <div class=""col-xl-3 col-lg-6 col-md-6 col-sm-12 mb-4"">
                                    <div class=""card h-100 shadow-sm"">
                                   
                                        <div class=""position-relative"">
");
#nullable restore
#line 97 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                             if (!string.IsNullOrEmpty(product.Image))
                                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                                <img");
            BeginWriteAttribute("src", " src=\"", 4589, "\"", 4609, 1);
#nullable restore
#line 99 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
WriteAttributeValue("", 4595, product.Image, 4595, 14, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral("\r\n                                                     class=\"card-img-top\"");
            BeginWriteAttribute("alt", "\r\n                                                     alt=\"", 4685, "\"", 4765, 1);
#nullable restore
#line 101 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
WriteAttributeValue("", 4745, product.ProductName, 4745, 20, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral("\r\n                                                     style=\"height: 200px; object-fit: cover;\">\r\n");
#nullable restore
#line 103 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                            }
                                            else
                                            {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                                                <div class=""card-img-top bg-light d-flex align-items-center justify-content-center""
                                                     style=""height: 200px;"">
                                                    <i class=""fas fa-image fa-3x text-muted""></i>
                                                </div>
");
#nullable restore
#line 110 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                                            <div class=\"position-absolute top-0 end-0 m-2\">\r\n                                                <span class=\"badge bg-success\">");
#nullable restore
#line 112 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                                                          Write(product.TotalSold);

#line default
#line hidden
#nullable disable
            WriteLiteral(@" sold</span>
                                            </div>
                                        </div>
                                        <div class=""card-body d-flex flex-column"">
                                            <h6 class=""card-title font-weight-bold"">");
#nullable restore
#line 116 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                                                               Write(product.ProductName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h6>\r\n                                            <p class=\"card-text text-muted small mb-2\">");
#nullable restore
#line 117 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                                                                  Write(product.CategoryName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n                                            <p class=\"card-text text-truncate\" style=\"max-height: 3em; overflow: hidden;\">\r\n                                                ");
#nullable restore
#line 119 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                            Write(string.IsNullOrEmpty(product.Description) ? "No description available" : product.Description);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
                                            </p>
                                            <div class=""mt-auto"">
                                                <div class=""d-flex justify-content-between align-items-center mb-2"">
                                                    <span class=""h6 mb-0 text-primary"">₱");
#nullable restore
#line 123 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                                                                   Write(product.Price.ToString("N2"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\r\n                                                    <small class=\"text-muted\">Stock: ");
#nullable restore
#line 124 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                                                                Write(product.Stock);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</small>
                                                </div>
                                                <div class=""d-flex justify-content-between align-items-center"">
                                                    <small class=""text-success font-weight-bold"">
                                                        Revenue: ₱");
#nullable restore
#line 128 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                                                             Write(product.TotalRevenue.ToString("N2"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                                                    </small>\r\n                                                </div>\r\n                                                <div class=\"mt-2\">\r\n                                                    <a");
            BeginWriteAttribute("href", " href=\"", 7386, "\"", 7433, 2);
            WriteAttributeValue("", 7393, "/Admin/ProductDetails/", 7393, 22, true);
#nullable restore
#line 132 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
WriteAttributeValue("", 7415, product.ProductID, 7415, 18, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" class=""btn btn-sm btn-outline-primary"">
                                                        View Details
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
");
#nullable restore
#line 140 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        </div>\r\n");
#nullable restore
#line 142 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                    }
                    else
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <div class=\"text-center py-4\">\r\n                            <i class=\"fas fa-box-open fa-3x text-muted mb-3\"></i>\r\n                            <p class=\"text-muted\">No sales data available yet.</p>\r\n                        </div>\r\n");
#nullable restore
#line 149 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Admin\Index.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                </div>
            </div>
        </div>
    </div>
</div>

<style>
    .card {
        transition: transform 0.2s;
    }

        .card:hover {
            transform: translateY(-2px);
        }

    .border-left-primary {
        border-left: 0.25rem solid #4e73df !important;
    }

    .border-left-success {
        border-left: 0.25rem solid #1cc88a !important;
    }

    .border-left-info {
        border-left: 0.25rem solid #36b9cc !important;
    }

    .border-left-warning {
        border-left: 0.25rem solid #f6c23e !important;
    }

    .text-truncate {
        display: -webkit-box;
        -webkit-line-clamp: 2;
        -webkit-box-orient: vertical;
    }
</style>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
