using System.Windows;
using System.Windows.Controls;

namespace PaginacaoView
{
    public partial class HorariosDisplay : UserControl
    {
        #region Header
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header), typeof(string), typeof(HorariosDisplay), new PropertyMetadata(default(string)));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        #endregion

        public static readonly DependencyProperty HProperty = DependencyProperty.Register(
            nameof(H), typeof(string[]), typeof(HorariosDisplay), new PropertyMetadata(null));

        public string[] H
        {
            get => (string[])GetValue(HProperty);
            set => SetValue(HProperty, value);
        }   
        
        public HorariosDisplay()
        {
            InitializeComponent();
        }
    }
}