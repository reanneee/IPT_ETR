#pragma checksum "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "9be322b6afdc03684a48fec56d0a2a783a89198b"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Product_Details), @"mvc.1.0.view", @"/Views/Product/Details.cshtml")]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"9be322b6afdc03684a48fec56d0a2a783a89198b", @"/Views/Product/Details.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"8f590f90d6d197def03301032e200ac8485fb047", @"/Views/_ViewImports.cshtml")]
    public class Views_Product_Details : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<Ecom.Models.Product>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Cart", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "AddToCart", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("<!-- Views/Product/Details.cshtml -->\r\n");
#nullable restore
#line 3 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
  
    ViewData["Title"] = Model.ProductName;

#line default
#line hidden
#nullable disable
            WriteLiteral(@"
<style>
    .container-narrow {
        max-width: 900px;
        margin: 0 auto;
        padding: 0 15px;
    }

    .back-button {
        display: inline-flex;
        align-items: center;
        color: #2d5a4e;
        text-decoration: none;
        font-weight: 500;
        margin-bottom: 20px;
        transition: color 0.3s ease;
    }

        .back-button:hover {
            color: #1e3d35;
            text-decoration: none;
        }

        .back-button i {
            margin-right: 8px;
        }



    ");
            WriteLiteral(@"@keyframes slideInDown {
        from {
            transform: translateY(-100%);
            opacity: 0;
        }

        to {
            transform: translateY(0);
            opacity: 1;
        }
    }

    .product-card {
        background: white;
        border-radius: 15px;
        box-shadow: 0 8px 30px rgba(0,0,0,0.08);
        overflow: hidden;
        margin: 20px 0;
    }

    .product-image {
        width: 100%;
        height: 400px;
        object-fit: cover;
        transition: transform 0.3s ease;
    }

        .product-image:hover {
            transform: scale(1.02);
        }

    .image-placeholder {
        height: 400px;
        background: #f8f9fa;
        display: flex;
        align-items: center;
        justify-content: center;
        border: 2px dashed #dee2e6;
    }

    .product-content {
        padding: 30px;
    }

    .product-title {
        font-size: 2rem;
        font-weight: 300;
        color: #2d5a4e;
        margi");
            WriteLiteral(@"n-bottom: 15px;
    }

    .product-description {
        color: #6c757d;
        margin-bottom: 20px;
        line-height: 1.6;
    }

    .price {
        font-size: 1.8rem;
        font-weight: 600;
        color: #2d5a4e;
        margin-bottom: 20px;
    }

    .product-info {
        display: flex;
        gap: 30px;
        margin-bottom: 25px;
        flex-wrap: wrap;
    }

    .info-item {
        display: flex;
        align-items: center;
        gap: 8px;
    }

    .category {
        background: #2d5a4e;
        color: white;
        padding: 6px 12px;
        border-radius: 20px;
        font-size: 0.9rem;
        font-weight: 500;
    }

    .stock {
        color: #28a745;
        font-weight: 500;
    }

        .stock.low {
            color: #ffc107;
        }

        .stock.out {
            color: #dc3545;
        }

    .stock-dot {
        width: 8px;
        height: 8px;
        border-radius: 50%;
        background: #28a745;
 ");
            WriteLiteral(@"   }

        .stock-dot.low {
            background: #ffc107;
        }

        .stock-dot.out {
            background: #dc3545;
        }

    .cart-section {
        display: flex;
        align-items: center;
        gap: 15px;
        flex-wrap: wrap;
    }

    .quantity-input {
        width: 80px;
        padding: 10px;
        border: 2px solid #dee2e6;
        border-radius: 8px;
        text-align: center;
        font-size: 1rem;
    }

        .quantity-input:focus {
            outline: none;
            border-color: #2d5a4e;
        }

    .add-to-cart-btn {
        background: #28a745;
        color: white;
        border: none;
        padding: 12px 24px;
        border-radius: 8px;
        font-weight: 600;
        transition: all 0.3s ease;
        cursor: pointer;
    }

        .add-to-cart-btn:hover {
            background: #218838;
            transform: translateY(-1px);
        }

    .out-of-stock {
        background: #dc3545;
  ");
            WriteLiteral("      color: white;\r\n        padding: 15px 20px;\r\n        border-radius: 8px;\r\n        text-align: center;\r\n        font-weight: 500;\r\n    }\r\n\r\n    ");
            WriteLiteral(@"@media (max-width: 768px) {
        .product-content {
            padding: 20px;
        }

        .product-title {
            font-size: 1.5rem;
        }

        .product-info {
            flex-direction: column;
            gap: 15px;
        }

        .cart-section {
            flex-direction: column;
            align-items: stretch;
        }

        .add-to-cart-btn {
            width: 100%;
        }

        .success-alert {
            padding: 14px 16px;
            margin-bottom: 15px;
        }

            .success-alert .alert-content {
                gap: 10px;
            }
    }
</style>

<div class=""container-narrow"">
    <a href=""javascript:history.back()"" class=""back-button"">
        <i class=""fas fa-arrow-left""></i> Back
    </a>

");
#nullable restore
#line 234 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
     if (TempData["SuccessMessage"] != null)
    {

#line default
#line hidden
#nullable disable
            WriteLiteral("        <div class=\"success-alert\" role=\"alert\">\r\n            <div class=\"alert-content\">\r\n                <div class=\"success-icon\">\r\n                    <i class=\"fas fa-check\"></i>\r\n                </div>\r\n                <span>");
#nullable restore
#line 241 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                 Write(TempData["SuccessMessage"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\r\n            </div>\r\n            <button type=\"button\" class=\"btn-close\" data-bs-dismiss=\"alert\" aria-label=\"Close\">\r\n                <i class=\"fas fa-times\"></i>\r\n            </button>\r\n        </div>\r\n");
#nullable restore
#line 247 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
    }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n    <div class=\"product-card\">\r\n        <div class=\"row g-0\">\r\n            <div class=\"col-md-6\">\r\n");
#nullable restore
#line 252 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                 if (!string.IsNullOrEmpty(Model.Image))
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <img");
            BeginWriteAttribute("src", " src=\"", 5435, "\"", 5453, 1);
#nullable restore
#line 254 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
WriteAttributeValue("", 5441, Model.Image, 5441, 12, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"product-image\"");
            BeginWriteAttribute("alt", " alt=\"", 5476, "\"", 5500, 1);
#nullable restore
#line 254 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
WriteAttributeValue("", 5482, Model.ProductName, 5482, 18, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n");
#nullable restore
#line 255 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                }
                else
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <div class=\"image-placeholder\">\r\n                        <i class=\"fas fa-image fa-4x text-muted\"></i>\r\n                    </div>\r\n");
#nullable restore
#line 261 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("            </div>\r\n\r\n            <div class=\"col-md-6\">\r\n                <div class=\"product-content\">\r\n                    <h1 class=\"product-title\">");
#nullable restore
#line 266 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                                         Write(Model.ProductName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h1>\r\n\r\n");
#nullable restore
#line 268 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                     if (!string.IsNullOrEmpty(Model.Description))
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <p class=\"product-description\">");
#nullable restore
#line 270 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                                                  Write(Model.Description);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\r\n");
#nullable restore
#line 271 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                <div class=\"price\">₱");
#nullable restore
#line 273 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                               Write(Model.Price.ToString("F2"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>\r\n\r\n                    <div class=\"product-info\">\r\n                        <div class=\"info-item\">\r\n                            <span class=\"category\">");
#nullable restore
#line 277 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                                              Write(Model.CategoryName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span>\r\n                        </div>\r\n\r\n                        <div class=\"info-item\">\r\n                            <span");
            BeginWriteAttribute("class", " class=\"", 6473, "\"", 6550, 2);
            WriteAttributeValue("", 6481, "stock-dot", 6481, 9, true);
#nullable restore
#line 281 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
WriteAttributeValue(" ", 6490, Model.Stock == 0 ? "out" : Model.Stock < 10 ? "low" : "", 6491, 59, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral("></span>\r\n                            <span");
            BeginWriteAttribute("class", " class=\"", 6594, "\"", 6667, 2);
            WriteAttributeValue("", 6602, "stock", 6602, 5, true);
#nullable restore
#line 282 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
WriteAttributeValue(" ", 6607, Model.Stock == 0 ? "out" : Model.Stock < 10 ? "low" : "", 6608, 59, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">\r\n");
#nullable restore
#line 283 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                                 if (Model.Stock > 0)
                                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                    <span>");
#nullable restore
#line 285 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                                     Write(Model.Stock);

#line default
#line hidden
#nullable disable
            WriteLiteral(" in stock</span>\r\n");
#nullable restore
#line 286 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                                }
                                else
                                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                    <span>Out of stock</span>\r\n");
#nullable restore
#line 290 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                            </span>\r\n                        </div>\r\n                    </div>\r\n\r\n");
#nullable restore
#line 295 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                     if (Model.Stock > 0)
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "9be322b6afdc03684a48fec56d0a2a783a89198b16843", async() => {
                WriteLiteral("\r\n                            <input type=\"hidden\" name=\"productId\"");
                BeginWriteAttribute("value", " value=\"", 7360, "\"", 7384, 1);
#nullable restore
#line 298 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
WriteAttributeValue("", 7368, Model.ProductID, 7368, 16, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@" />

                            <div class=""cart-section"">
                                <input type=""number""
                                       class=""quantity-input""
                                       name=""quantity""
                                       value=""1""
                                       min=""1""");
                BeginWriteAttribute("max", "\r\n                                       max=\"", 7717, "\"", 7775, 1);
#nullable restore
#line 306 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
WriteAttributeValue("", 7763, Model.Stock, 7763, 12, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral(@"
                                       title=""Quantity"" />

                                <button type=""submit"" class=""add-to-cart-btn"">
                                    <i class=""fas fa-cart-plus""></i> Add to Cart
                                </button>
                            </div>
                        ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
#nullable restore
#line 314 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                    }
                    else
                    {

#line default
#line hidden
#nullable disable
            WriteLiteral("                        <div class=\"out-of-stock\">\r\n                            <i class=\"fas fa-exclamation-triangle\"></i> Currently Out of Stock\r\n                        </div>\r\n");
#nullable restore
#line 320 "C:\Users\Gloreanne\source\repos\Ecom\Ecom\Views\Product\Details.cshtml"
                    }

#line default
#line hidden
#nullable disable
            WriteLiteral("                </div>\r\n            </div>\r\n        </div>\r\n    </div>\r\n</div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<Ecom.Models.Product> Html { get; private set; }
    }
}
#pragma warning restore 1591
