using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels.DynamicLayout.ValueEditors
{
    public class DateTimeValueEditor : SingleValueEditor<DateTime>
    {
        public DateTimeValueEditor()
        {

        }

        public DateTimeValueEditor(DateTime value)
            : base(value)
        {
        }
        

        public static DateTimeValueEditor TryParse(string dateTime, DateTimeValueEditor defaultValue)
        {
            if (string.IsNullOrEmpty(dateTime))
                return defaultValue;
            
            var dateTimeComponents = dateTime.Split('/');
            var year = int.Parse(dateTimeComponents[2]);
            var month = int.Parse(dateTimeComponents[1]);
            var day = int.Parse(dateTimeComponents[0]);

            return new DateTimeValueEditor(new DateTime(year, month, day));

        }



        protected override string FormatValue(DateTime date)
        {
            return FormatDate(date);
        }


        public static string FormatDate(DateTime date)
        {
            return $"{PadLeftWithZero(date.Day)}/{PadLeftWithZero(date.Month)}/{date.Year}";
        }
        
        private static string PadLeftWithZero(int x)
        {
            return x.ToString().PadLeft(2, '0');
        }


        public override IConditionValueEditor Clone()
        {
            return new DateTimeValueEditor() { Value = this.Value };
        }

        protected override DateTime Incrementer(DateTime value)
        {
            return value.AddDays(1);
        }

        protected override DateTime Decrementer(DateTime value)
        {
            return value.AddDays(-1);
        }


    }
}
