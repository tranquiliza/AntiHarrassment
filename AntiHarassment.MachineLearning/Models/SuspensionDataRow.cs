using AntiHarassment.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntiHarassment.MachineLearning.Models
{
    public class SuspensionDataRow
    {
        public bool IsFlaggedAsTag { get; set; }
        public string Text { get; set; }

        public SuspensionDataRow(Suspension suspension, Tag tag)
        {
            if (suspension.Tags.Any(x => x.TagId == tag.TagId))
            {
                IsFlaggedAsTag = true;
            }

            // TODO this might add too much inaccuracy.
            var stringBuilder = new StringBuilder();
            foreach (var message in suspension.ChatMessages)
            {
                stringBuilder.AppendLine(message.Message);
            }

            Text = stringBuilder.ToString();
        }
    }
}
