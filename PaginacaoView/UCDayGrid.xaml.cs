using System.Windows;
using System.Windows.Controls;

namespace PaginacaoView
{
    public partial class UcDayGrid : UserControl 
    {
        public ContentControl[] Horarios { get; private set; }

        public static readonly DependencyProperty DayNameProperty = DependencyProperty.Register(
            nameof(DayName), typeof(string), typeof(UcDayGrid), new PropertyMetadata(default(string)));
        public string DayName
        {
            get => (string)GetValue(DayNameProperty);
            set => SetValue(DayNameProperty, value);
        }

        public UcDayGrid()
        {
            InitializeComponent();
            Horarios = new [] { H0, H1, H2, H3, H4 };
        }
    }
}