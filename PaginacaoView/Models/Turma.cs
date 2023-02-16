using System.Windows;
using System.Windows.Controls;

namespace PaginacaoView.Models
{
    public class Turma
    {
        public string Name { get; set; }
        public bool Permission { get; set; }
        public DayHour[] DayHour { get; set; }

        public StackPanel ToStackPanel()
        {
            var content = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            var nome = new TextBlock()
            {
                Text = $"{Name}",
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            
            content.Children.Add(nome);

            foreach (var horario in DayHour)
            {
                content.Children.Add(
                    new TextBlock() { Text = $"{horario}", TextAlignment = TextAlignment.Center }
                );
            }

            return content;
        }
    }
}