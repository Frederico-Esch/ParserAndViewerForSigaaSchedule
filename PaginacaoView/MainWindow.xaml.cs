using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json;
using PaginacaoView.Models;

namespace PaginacaoView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Turma[] _turmas;

        private readonly Dictionary<int, Dictionary<int, Turma>> _horarios = new()
        {
            {2, new Dictionary<int, Turma>()},
            {3, new Dictionary<int, Turma>()},
            {4, new Dictionary<int, Turma>()},
            {5, new Dictionary<int, Turma>()},
            {6, new Dictionary<int, Turma>()},
        };

        private readonly Dictionary<int, UcDayGrid> _horarioViews = new();
        private void LoadContentFromJson(string pathToFile)
        {
            string data;
            using (var stream = new FileStream(pathToFile, FileMode.Open))
            using (var reader = new StreamReader(stream))
            {
                data = reader.ReadToEnd();
            }
            _turmas = JsonConvert.DeserializeObject<Turma[]>(data)?.Where(t => t.Permission).ToArray() ?? Array.Empty<Turma>();
            
            if(ListaTurmas.Items.Count > 0) ListaTurmas.Items.Clear();
            foreach (var item in _horarios) item.Value.Clear();
            foreach (var turma in _turmas)
            {
                foreach (var dayHour in turma.DayHour) { dayHour.Hour -= 1; }
                var checkBox = new CheckBox()
                {
                    Content = turma.ToStackPanel(),
                    Tag = turma,
                    VerticalAlignment = VerticalAlignment.Center
                };
                
                checkBox.Checked += HandleCheck;
                checkBox.Unchecked += HandleUncheck;
                
                var itemList = new ListViewItem
                {
                    Content = checkBox,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                ListaTurmas.Items.Add(itemList);
            }
        }

        private bool VerifyHours(IEnumerable<DayHour> dayHours)
        {
            return !dayHours.Any(horario => _horarios[horario.Day].ContainsKey(horario.Hour));
        }
        
        public MainWindow()
        {
            InitializeComponent();
            _horarioViews[2] = Segunda;
            _horarioViews[3] = Terca;
            _horarioViews[4] = Quarta;
            _horarioViews[5] = Quinta;
            _horarioViews[6] = Sexta;
        }

        private void UpdateView()
        {
            foreach (var item in _horarios)
            {
                var dia = item.Key;
                var horarios = item.Value;

                foreach (var subItem in horarios)
                {
                    var horario = subItem.Key;
                    var turma = subItem.Value;

                    var content = new Border()
                    {
                        Child = turma.ToStackPanel(),
                        Background = Brushes.LightSkyBlue
                    };
                    _horarioViews[dia].Horarios[horario].Content = content;
                }
            }
        }

        private void PaintSignalBackground(IEnumerable<DayHour> horarios, Brush background)
        {
            foreach (var diaHorario in horarios)
            {
                var dia = diaHorario.Day;
                var horario = diaHorario.Hour;
                if (!_horarios[dia].ContainsKey(horario)) continue;
                if (_horarioViews[dia].Horarios[horario].Content is not Border border) continue;
                border.Background = background;
            }
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox { Tag: Turma turma} button) return;
            
            if (!VerifyHours(turma.DayHour))
            {
                PaintSignalBackground(turma.DayHour, Brushes.IndianRed);
                MessageBox.Show("Já ocupado", "Horário conflitante");
                PaintSignalBackground(turma.DayHour, Brushes.LightSkyBlue);
                button.Unchecked -= HandleUncheck;
                button.IsChecked = false;
                button.Unchecked += HandleUncheck;
            }
            else
            {
                foreach (var horario in turma.DayHour)
                {
                    _horarios[horario.Day][horario.Hour] = turma;
                }

                UpdateView();
            }
            
            ListaTurmas.UnselectAll();
        }
        
        private void HandleUncheck(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox { Tag: Turma turma } button) return;
            foreach (var horario in turma.DayHour)
            {
                _horarios[horario.Day].Remove(horario.Hour);
                _horarioViews[horario.Day].Horarios[horario.Hour].Content = null;
                _horarioViews[horario.Day].Horarios[horario.Hour].Background = Brushes.Transparent;
            }
            ListaTurmas.UnselectAll();
        }

        private void HandleSelectFileButton(object sender, EventArgs e)
        {
            var fileSelector = new OpenFileDialog
            {
                Filter = @"Json files (*.json) |*.json;*.Json;*.JSON;*.txt",
                Multiselect = false
            };
            if ((!(fileSelector.ShowDialog() ?? false)) || !fileSelector.CheckFileExists) return;
            LoadContentFromJson(fileSelector.FileName);
        }
    }
}