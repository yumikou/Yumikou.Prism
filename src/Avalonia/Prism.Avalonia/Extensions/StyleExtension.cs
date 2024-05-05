using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Extensions
{
    internal static class StyleExtension
    {
        public static Style SimpleClone(this Style source)
        {
            if (source is null) { return null; }

            Style resultStyle = new Style();
            resultStyle.Selector = source.Selector;

            foreach (var ani in source.Animations)
            {
                resultStyle.Animations.Add(ani);
            }
            foreach (var chi in source.Children)
            {
                resultStyle.Children.Add(chi);
            }
            foreach (var res in source.Resources)
            {
                resultStyle.Resources.Add(res);
            }
            foreach (var set in source.Setters)
            {
                resultStyle.Setters.Add(set);
            }

            return resultStyle;
        }
    }
}
