using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Threading;
using Windows.Devices.Enumeration;
using Windows.Devices.Spi;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a


namespace ProyectoPi
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {

        //
        private const byte SPI_CHIP_SELECT_LINE = 0;        /* Chip select line to use                              */
        private const byte COMAND_0 = 0X31;
        private const byte COMAND_1 = 0X32;
        private const byte COMAND_2 = 0X33;
        private const byte COMAND_3 = 0X34;
        private const byte COMAND_4 = 0X35;

        private SpiDevice SPI_PIC;
        private bool clear = false;
        public MainPage()
        {
            this.InitializeComponent();

            InitSPI();

        }

        private async void InitSPI()
        {
            try
            {
                var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
                settings.ClockFrequency = 156250;                              /* 5MHz is the rated speed of the ADXL345 accelerometer                     */
                settings.Mode = SpiMode.Mode1;                                  /* The accelerometer expects an idle-high clock polarity, we use Mode3    
                                                                                 * to set the clock polarity and phase to: CPOL = 1, CPHA = 1         
                                                                                 */

                string aqs = SpiDevice.GetDeviceSelector();                     /* Get a selector string that will return all SPI controllers on the system */
                var dis = await DeviceInformation.FindAllAsync(aqs);            /* Find the SPI bus controller devices with our selector string             */
                SPI_PIC = await SpiDevice.FromIdAsync(dis[0].Id, settings);    /* Create an SpiDevice with our bus controller and SPI settings             */
                if (SPI_PIC == null)
                {
                    this.Nombres.Text = string.Format(
                        "SPI Controller {0} is currently in use by " +
                        "another application. Please ensure that no other applications are using SPI.",
                        dis[0].Id);
                    return;
                }
            }
            catch (Exception ex)
            {
                this.Nombres.Text = "SPI Initialization failed. Exception: " + ex.Message;
                return;
            }

            /* 
             * Initialize the accelerometer:
             *
             * For this device, we create 2-byte write buffers:
             * The first byte is the register address we want to write to.
             * The second byte is the contents that we want to write to the register. 
             */
        }

        private void Nom_Click(object sender, RoutedEventArgs e)
        {
            if (clear)
            {
                this.Nombres.Text = "";
                this.Nombres1.Text = "";
                this.Nombres2.Text = "";
                clear = false;
            }
            else
            {
                this.Nombres.Text = "Fuentes Bello David\n";
                this.Nombres1.Text = "Hernándes Abad Adrian\n";
                this.Nombres2.Text = "Martínez Gonzáles Andrés Alfonso\n";
                clear = true;
            }
        }

        private void Spi_Click(object sender, RoutedEventArgs e)
        {
            byte[] WriteBuf_DataFormat = new byte[] { COMAND_0 };
            try
            {
                SPI_PIC.Write(WriteBuf_DataFormat);
            }
            /* If the write fails display the error and stop running */
            catch (Exception ex)
            {
                this.Nombres.Text = "Failed to communicate with device: " + ex.Message;
                return;
            }
        }
    }
}
