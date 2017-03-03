namespace Sitecore.Support.FXM.Speak.Controls.Navigation.Menus
{
  using Sitecore;
  using Sitecore.Data.Items;
  using Sitecore.FXM.Abstractions;
  using Sitecore.FXM.Speak.Controls.Navigation.Menus;
  using Sitecore.Names;
  using Sitecore.Resources;
  using Sitecore.Web.UI;
  using Sitecore.Web.UI.Controls.Common.HyperlinkButtons;
  using System;
  using System.Linq;
  using System.Runtime.CompilerServices;
  using System.Text;
  using System.Web.Mvc;

  public static class DashboardMenuHelper
  {
    private static string GetHeaderItemHtml(DashboardMenuItem headerItem)
    {
      StringBuilder builder = new StringBuilder();
      string str = headerItem.IsOpen ? "block" : "none";
      string str2 = headerItem.IsOpen ? " open" : string.Empty;
      string str3 = headerItem.IsOpen ? " isOpen='true" : string.Empty;
      string str4 = headerItem.IsHighlighted ? " highlighted" : string.Empty;
      string themedImageSource = Images.GetThemedImageSource(headerItem.Icon, ImageDimension.id24x24);
      builder.Append("<div>");
      builder.Append("<div class = 'header menuItem" + str2 + str4 + "' " + str3 + "'>");
      builder.Append("<a href='#'>");
      builder.Append("<img class='menuicon' src='" + themedImageSource + "'/>");
      builder.Append("<span class='toplevel'>");
      builder.Append(headerItem.Name);
      builder.Append("</span></a><img class='menuchevron'/>");
      builder.Append("</div>");
      builder.Append(string.Concat(new object[] { "<div class='toplevelcontainer itemsContainer' sc-guid='", headerItem.Id.Guid, "' style='display:", str, "'>" }));
      return builder.ToString();
    }

    private static string GetLinkItemHtml(DashboardMenuItem menuItem)
    {
      string str = menuItem.IsSelected ? "selected" : string.Empty;
      string str2 = menuItem.IsOpen ? "block" : "none";
      string str3 = menuItem.IsOpen ? " open" : string.Empty;
      string str4 = menuItem.IsOpen ? " isOpen='true" : string.Empty;
      string str5 = menuItem.IsHighlighted ? " highlighted" : string.Empty;
      StringBuilder builder = new StringBuilder();
      builder.Append("<div>");
      builder.Append(string.Concat(new object[] { "<div class = 'itemRow menuItem depth", menuItem.Depth, " ", str, str5, "'", str4, "'>" }));
      builder.Append("<div class='leftcolumn'>");
      if (menuItem.Children.Any<DashboardMenuItem>())
      {
        builder.Append("<a href='#'>");
        builder.Append("<div class='arrowcontainer " + str3 + "'>");
        builder.Append("<img/>");
        builder.Append("</div>");
        builder.Append("</a>");
      }
      else
      {
        builder.Append("&nbsp;");
      }
      builder.Append("</div>");
      builder.Append("<div class='rightcolumn'>");
      HyperlinkButton button1 = new HyperlinkButton
      {
        Text = menuItem.Name,
        Click = menuItem.Click
      };
      string str6 = button1.Render();
      builder.Append(str6);
      builder.Append("</div>");
      builder.Append("</div>");
      builder.Append(string.Concat(new object[] { "<div class='sublevelcontainer itemsContainer depth", menuItem.Depth, "' sc-guid='", menuItem.Id.Guid, "' style='display:", str2, "'>" }));
      return builder.ToString();
    }

    private static string GetRootItemHtml(DashboardMenuItem rootItem)
    {
      string str = rootItem.IsSelected ? "selected" : string.Empty;
      string themedImageSource = Images.GetThemedImageSource(rootItem.Icon, ImageDimension.id24x24);
      StringBuilder builder = new StringBuilder();
      builder.Append("<div class = 'header rootItem " + str + "'>");
      builder.Append("<a href='" + rootItem.Url + "'>");
      builder.Append("<img class='menuicon' src='" + themedImageSource + "'/>");
      builder.Append("<span class='toplevel'>");
      builder.Append(rootItem.Name);
      builder.Append("</span></a><img class='menuchevron' style='visibility:hidden'/>");
      builder.Append("</div>");
      return builder.ToString();
    }

    public static string GetUserProfileKey(string rootItemUrl)
    {
      Item item = Context.Database.GetItem(rootItemUrl);
      return string.Format(Sitecore.Names.Constants.UserProfileKeyFormat, "CrossPagesNavigationStatus", item.ID.ToShortID());
    }

    public static MvcHtmlString RenderExtendedMenu(this HtmlHelper helper, string rootItemId, bool isRootHidden, string userProfileValue)
    {
      if (string.IsNullOrEmpty(rootItemId))
      {
        return new MvcHtmlString(string.Empty);
      }
      Item item = Context.Database.GetItem(rootItemId);
      if (item == null)
      {
        return new MvcHtmlString(string.Empty);
      }
      DashboardMenuBuilder builder = new DashboardMenuBuilder(new SitecoreContextWrapper());
      return new MvcHtmlString(RenderHtml(builder.BuildMenu(item.ID, userProfileValue), isRootHidden));
    }

    private static string RenderHtml(DashboardMenuItem menuItem, bool isRootHidden)
    {
      StringBuilder builder = new StringBuilder();
      builder.Append(RenderItemHtml(menuItem, isRootHidden));
      if (menuItem.Children.Any<DashboardMenuItem>())
      {
        foreach (DashboardMenuItem item in menuItem.Children)
        {
          builder.Append(RenderHtml(item, isRootHidden));
        }
      }
      if (menuItem.Depth > 1)
      {
        builder.Append("</div></div>");
      }
      return builder.ToString();
    }

    private static string RenderItemHtml(DashboardMenuItem menuItem, bool isRootHidden)
    {
      switch (menuItem.Depth)
      {
        case 0:
          {
            StringBuilder builder = new StringBuilder();
            builder.Append("<div sc-guid='" + menuItem.Id + "'>");
            if (!isRootHidden)
            {
              builder.Append(GetRootItemHtml(menuItem));
            }
            return builder.ToString();
          }
        case 1:
          return GetHeaderItemHtml(menuItem);
      }
      return GetLinkItemHtml(menuItem);
    }
  }
}
