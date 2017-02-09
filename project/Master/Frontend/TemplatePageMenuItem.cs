using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeMiner.Master.Frontend
{
    public sealed class TemplatePageMenuItem
    {
        public string Text { get; set; }
        public string Path { get; set; }
        public int Number { get; set; }

        public TemplatePageMenuItem(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new Exception("Menu item cannot have null or empty text");
            }
            Text = text;
        }
    }
}
