using System.Collections.Generic;

namespace PaginacaoView.Models
{
    public class DayHour
    {
        public int Day { get; set; }
        public int Hour { get; set; }

        private static readonly Dictionary<int, string> DayToString = new ()
        {
            {2, "Segunda"},
            {3, "Terça"},
            {4, "Quarta"},
            {5, "Quinta"},
            {6, "Sexta"},
        };

        private static readonly Dictionary<int, string> HourToString = new ()
        {
            {0, "13h15 - 15h05"},
            {1, "15h15 - 17h05"},
            {2, "17h05 - 18h55"},
            {3, "19h00 - 20h50"},
            {4, "21h00 - 22h50"},
        };
        public override string ToString()
        {
            return $"{DayToString[Day]} {HourToString[Hour]}";
        }
    }
}