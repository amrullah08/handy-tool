using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RavePOCBot
{
    [Serializable]
    public class CustomCalendarForm
    {
        public DateTime? CheckInDate;
        public static IForm<CustomCalendarForm> BuildForm()
        {
            return new FormBuilder<CustomCalendarForm>().Message("Select Date ").Build();
        }
    }
}