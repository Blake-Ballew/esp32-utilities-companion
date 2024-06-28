using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;

namespace esp32_utilities_companion
{
    /// <summary>
    /// Interaction logic for SerialPage.xaml
    /// </summary>
    public partial class SerialPage : UserControl
    {
        private SerialPort _port;
        public string[] SerialPorts { get; private set; }
        public string ConnectButtonText { get; private set; }

        public SerialPage()
        {
            InitializeComponent();
            _port = new SerialPort();
            _port.DataReceived += SerialPort_DataReceived;
            SerialPorts = Array.Empty<string>();
            ConnectButtonText = "Connect";
            GetSerialPorts();
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Thread.Sleep(5);
            while (_port.BytesToRead > 0)
            {
                var data = _port.ReadLine();
                if (data != null && UpdateDebugBox.IsChecked == true)
                {
                    DebugBox.Items.Add(data);
                }
            }
        }

        private void SerialDevices_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SerialDevices.Height = ConnectionPanel.Height - (ConnectionButtons.Height + 5);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetSerialPorts();
        }

        private void GetSerialPorts()
        {
            SerialPorts = SerialPort.GetPortNames();
            SerialDevices.Items.Clear();

            foreach (string serialPort in SerialPorts)
            {
                SerialDevices.Items.Add(serialPort);
            }

            if (_port.IsOpen && SerialDevices.Items.Contains(_port.PortName))
            {
                SerialDevices.SelectedIndex = SerialDevices.Items.IndexOf(_port.PortName);
            }
        }

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            var SelectedPort = SerialDevices.SelectedItem as string;

            if (SelectedPort != null)
            {
                _port.PortName = SelectedPort;
                _port.Open();

                if (_port.IsOpen)
                {
                    Connect.Click -= Connect_Click;
                    Connect.Click += Disconnect_Click;
                    Connect.Content = "Disconnect";
                }
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (_port.IsOpen)
            {
                _port.Close();
            }

            Connect.Click -= Disconnect_Click;
            Connect.Click += Connect_Click;
            Connect.Content = "Connect";
        }
    }
}
