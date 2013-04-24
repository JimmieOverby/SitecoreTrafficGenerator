using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Sitecore.Collections;
using Sitecore.Data.Items;

namespace Dms.Jump.TrafficGenerator.Business.Helper
{
    public class ItemHelper
    {
        public static StringCollection GetRandomGoalNames()
        {
            StringCollection stringCollection = new StringCollection();

            var systemSetting = Sitecore.Context.Database.GetItem("/sitecore/system/Marketing Center/Goals");

            var goals = systemSetting.GetChildren(ChildListOptions.None);

            foreach (Item goal in goals)
            {
                bool isGoal = goal.Fields["IsGoal"] != null && goal.Fields["IsGoal"].ToString() == "1";
                bool isSystem = goal.Fields["IsSystem"] != null && goal.Fields["IsSystem"].ToString() == "1";

                if (isGoal && !isSystem)
                    stringCollection.Add(goal.Name); 
            }

            return stringCollection;
        }



        public static void GetRandomRelativePaths(Item currentItem, ref StringCollection stringCollection)
        {
            if (stringCollection == null)
                stringCollection = new StringCollection();

            if (currentItem != null)
            {
                var children = currentItem.GetChildren(ChildListOptions.None);
                if (children != null)
                {
                    foreach (Item child in children)
                    {
                        if (child.Fields["__Renderings"] != null && !String.IsNullOrEmpty(child.Fields["__Renderings"].ToString()))
                        {
                            string url = Sitecore.Links.LinkManager.GetItemUrl(child);

                            var aspxExtension = ".aspx";
                            if (!url.EndsWith(aspxExtension))
                                url += aspxExtension;

                            stringCollection.Add(url);
                            GetRandomRelativePaths(child, ref stringCollection);
                        }
                    }
                }
            }
        }

    }
}
