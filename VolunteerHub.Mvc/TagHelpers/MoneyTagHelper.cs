using Microsoft.AspNetCore.Razor.TagHelpers;

namespace VolunteerHub.Mvc.TagHelpers
{
    [HtmlTargetElement("money")]
    public class MoneyTagHelper : TagHelper
    {
        public decimal Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.Attributes.SetAttribute("style", "font-weight:bold;");
            output.Content.SetContent($"{Value:0.00} UAH");
        }
    }
}
