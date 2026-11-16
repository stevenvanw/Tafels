using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tafels
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Info : Window
    {
        public Info()
        {
            InitializeComponent();
            tboxInfo.Text = new StreamReader("Info.txt").ReadToEnd();
            // Dit zorgt ervoor dat het venster tijdens het runnen wél transparant wordt,
            // zonder dat de XAML designer hiervoor crasht of onzichtbaar wordt!
            this.AllowsTransparency = true;
            this.Background = System.Windows.Media.Brushes.Transparent;
        }

        private void TerugButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
