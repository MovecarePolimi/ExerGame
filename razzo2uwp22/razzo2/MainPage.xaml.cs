using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using UnityEngine.Windows;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.System;
using System.ComponentModel;



using System.Runtime.InteropServices.WindowsRuntime;


using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using System.Collections.ObjectModel;
using System.Text;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using UnityPlayer;
using Rect = Windows.Foundation.Rect;
using Application = Windows.UI.Xaml.Application;
using Canvas = Windows.UI.Xaml.Controls.Canvas;
using Color = Windows.UI.Color;
using Debug = System.Diagnostics.Debug;
using Object = System.Object;
using Windows.ApplicationModel;

namespace razzo2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //QUESTI
        private ObservableCollection<BluetoothLEDeviceDisplay> KnownDevices = new ObservableCollection<BluetoothLEDeviceDisplay>();
        private List<DeviceInformation> UnknownDevices = new List<DeviceInformation>();
        private DeviceWatcher deviceWatcher;
        private ObservableCollection<BluetoothLEAttributeDisplay> ServiceCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();
        private ObservableCollection<BluetoothLEAttributeDisplay> CharacteristicCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();
        private BluetoothLEDevice bluetoothLeDevice = null;
        public static GattCharacteristic selectedCharacteristic;
        public static GattCharacteristic selectedCharacteristic2;
        // Only one registered characteristic at a time.
        private GattCharacteristic registeredCharacteristic;
        private GattCharacteristic registeredCharacteristic2;
        private GattPresentationFormat presentationFormat;
        private GattPresentationFormat presentationFormat2;

        static public string DEVICE_NAME = "MOVB310";
        static public int SOC_level;

        private WinRTBridge.WinRTBridge _bridge;

        private SplashScreen splash;
        private Rect splashImageRect;
        private WindowSizeChangedEventHandler onResizeHandler;
        private bool isPhone = false;
        static public string language;

        static public bool timeoutFlag = false;

        static public float p = 0;
        static public float c = 0;
        static public float ax = 0;
        static public float ay = 0;
        static public float az = 0;
        static public float gx = 0;
        static public float gy = 0;
        static public float gz = 0;
        static public double moduleofgy = 0;
        static public double moduleofacc = 0;

        static public float timestampBat = 0;
        static public float soc = 0;
        static public float volt = 0;
        static public float amps = 0;
        static public float stat = 0;

        static public bool socFlag;

        static public string BTMacAddress;
    
        public const string foldername = @"MoveCare_SmartObjects\DATA\New\exg";
        //public const string homepath = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
        static public StorageFile file_raw = null;
        static public StorageFile file_summary = null;
        static public StorageFile file_bat = null;
        static public StorageFolder sampleFolder = null;
        static int contador = 0;
        static string valore_m_s;
        static string valore_previous;

        static public System.DateTime openTime;
        //System.DateTime closeTime;

        static Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
        static StorageFile sampleFile = null;
        static StorageFile sampleFile2 = null;

        /* GENERAL TIMEOUT
         */
        static public int generalTimeOut = 600;

        static public bool nuevoFlag2 = false;
        static public int conta = 0;

        public MainPage()
        {
            
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;

            AppCallbacks appCallbacks = AppCallbacks.Instance;
            // Setup scripting bridge
            _bridge = new WinRTBridge.WinRTBridge();
            appCallbacks.SetBridge(_bridge);

            bool isWindowsHolographic = false;
            InitVariables();
            InitLanguage();
            ValidateFolder();
            ReadMassimo();
            ReadPreviousValues();

            openTime = DateTime.Now;
            //
#if UNITY_HOLOGRAPHIC
            // If application was exported as Holographic check if the device actually supports it,
            // otherwise we treat this as a normal XAML application
            isWindowsHolographic = AppCallbacks.IsMixedRealitySupported();
#endif

            if (isWindowsHolographic)
            {
                appCallbacks.InitializeViewManager(Window.Current.CoreWindow);
            }
            else
            {
                appCallbacks.RenderingStarted += () => { RemoveSplashScreen(); };

                if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1))
                    isPhone = true;

                appCallbacks.SetSwapChainPanel(GetSwapChainPanel());
                appCallbacks.SetCoreWindowEvents(Window.Current.CoreWindow);
                appCallbacks.InitializeD3DXAML();

                splash = ((App)App.Current).splashScreen;
                GetSplashBackgroundColor();
                OnResize();
                onResizeHandler = new WindowSizeChangedEventHandler((o, e) => OnResize());
                Window.Current.SizeChanged += onResizeHandler;
            }



            //QUESTI
            /*
            if (deviceWatcher == null)
            {
                StartBleDeviceWatcher();
            }
            else
            {
                StopBleDeviceWatcher();
            }
            */
            Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);

            //  Observable observable = new Observable();
            //  Observer observer = new Observer();
            // observable.SomethingHappened += observer.HandleEvent;
            // observable.DoSomething();

            DispatcherTimerSetup();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            splash = (SplashScreen)e.Parameter;
            OnResize();

        }

        static public void InitLanguage()
        {
            Windows.ApplicationModel.Resources.ResourceLoader resourceLoader;
            var language = Windows.System.UserProfile.GlobalizationPreferences.Languages;
            //var language = "en-GB";
            switch (language[0])
                        {
                            case "it-IT":
                                resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("lang_it");
                                break;
                            case "en-GB":
                                resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("lang_en");
                                break;
                            case "es-ES":
                                resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("lang_es");
                                break;
                            default:
                                resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("lang_en");
                                break;
                        }

            tutorialscript.findBall_c = resourceLoader.GetString("findBall");
            changInstr.text1 = resourceLoader.GetString("text1");
            changInstr.text2 = resourceLoader.GetString("text2");
            changInstr.text3 = resourceLoader.GetString("text3");
            changInstr.text4 = resourceLoader.GetString("text4");
            changInstr.text5 = resourceLoader.GetString("text5");
            changInstr.text6 = resourceLoader.GetString("text6");
            changInstr.text7 = resourceLoader.GetString("text7");
            changInstr.text8 = resourceLoader.GetString("text8");
            changInstr.text8 = resourceLoader.GetString("text9");
            changInstr.text10 = resourceLoader.GetString("text10");
            changInstr.text11 = resourceLoader.GetString("text11");
            changInstr.text12 = resourceLoader.GetString("text12");
            changInstr.text13 = resourceLoader.GetString("text13");
            changInstr.text14 = resourceLoader.GetString("text14");
            changInstr.text15 = resourceLoader.GetString("text15");
            changInstr.text16 = resourceLoader.GetString("text16");
            scoreupdater.text17 = resourceLoader.GetString("text17");
            destroycollect2.text18 = resourceLoader.GetString("text18");
            destroycollect2.text19 = resourceLoader.GetString("text19");
            destroycollect2.text20 = resourceLoader.GetString("text20");
            destroycollect2.text21 = resourceLoader.GetString("text21");
            destroycollect2.text22 = resourceLoader.GetString("text22");
            countdownscript.text23 = resourceLoader.GetString("text23");
            ControlColl2.text24 = resourceLoader.GetString("text24");
            gamecontrollerendur.text25 = resourceLoader.GetString("text25");
            decollo4.text26 = resourceLoader.GetString("text26");
            decollo4.text27 = resourceLoader.GetString("text27");
            decollo4.text28 = resourceLoader.GetString("text28");
            decollo4.text29 = resourceLoader.GetString("text29");
            decollo4.text30 = resourceLoader.GetString("text30");
            decollo4.text31 = resourceLoader.GetString("text31");
            changInstr.text32 = resourceLoader.GetString("text32");
            changInstr.text33 = resourceLoader.GetString("text33");
        }



        static public void InitVariables()
        {
        
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("configInit");
            string life_s = resourceLoader.GetString("life");
            string points_s = resourceLoader.GetString("points");
            string instrDone_s = resourceLoader.GetString("instrDone");
            string press_s = resourceLoader.GetString("press");
            string sogliacalibrazioneavvenuta_s = resourceLoader.GetString("sogliacalibrazioneavvenuta");
            string sogliaskip_s = resourceLoader.GetString("sogliaskip");
            string sogligonfiaggio_s = resourceLoader.GetString("sogligonfiaggio");
            string valore_massimo_s = resourceLoader.GetString("valore_massimo");
            string pressmax_s = resourceLoader.GetString("pressmax");
            string skip_s = resourceLoader.GetString("skip");
            string abilitaskip_s = resourceLoader.GetString("abilitaskip");
            string abilitadopocalibraz_s = resourceLoader.GetString("abilitadopocalibraz");
            string abilitaprimacalibraz_s = resourceLoader.GetString("abilitaprimacalibraz");
            string statogameforce_s = resourceLoader.GetString("statogameforce");
            string statogameendur_s = resourceLoader.GetString("statogameendur");
            string maxofpercentilecomputed_s = resourceLoader.GetString("maxofpercentilecomputed");
            string provenoneseguite_s = resourceLoader.GetString("provenoneseguite");
            string endurancenotdone_s = resourceLoader.GetString("endurancenotdone");
            string duratamassimaendurance_s = resourceLoader.GetString("duratamassimaendurance");
            string sogliarettification_s = resourceLoader.GetString("sogliarettification");
            string percentile1_s = resourceLoader.GetString("percentile1");
            string percentile2_s = resourceLoader.GetString("percentile2");
            string percentile3_s = resourceLoader.GetString("percentile3");
            string maxofpercentile_s = resourceLoader.GetString("maxofpercentile");
            string maxperendurance_s = resourceLoader.GetString("maxperendurance");
            string pressaveraged_s = resourceLoader.GetString("pressaveraged");
            string registrazioneok_s = resourceLoader.GetString("registrazioneok");
            string puntiprecedenti_s = resourceLoader.GetString("puntiprecedenti");
            string statodurantecalibrazione_s = resourceLoader.GetString("statodurantecalibrazione");
            string devicenotfound_s = resourceLoader.GetString("devicenotfound");
            string secondidiricerca_s = resourceLoader.GetString("secondidiricerca");
            string sparoinbonus_s = resourceLoader.GetString("sparoinbonus");
            string carica_batteria_s = resourceLoader.GetString("carica_batteria"); 



            Int32.TryParse(life_s, out int life_c);
            Int32.TryParse(points_s, out int points_c);
            bool.TryParse(instrDone_s, out bool instrDone_c);
            float.TryParse(press_s, out float press_c);
            float.TryParse(sogliacalibrazioneavvenuta_s, out float sogliacalibrazioneavvenuta_c);
            float.TryParse(sogliaskip_s, out float sogliaskip_c);
            float.TryParse(sogligonfiaggio_s, out float sogligonfiaggio_c);
            float.TryParse(valore_massimo_s, out float valore_massimo_c);
            float.TryParse(pressmax_s, out float pressmax_c);
            bool.TryParse(skip_s, out bool skip_c);
            bool.TryParse(abilitaskip_s, out bool abilitaskip_c);
            bool.TryParse(abilitadopocalibraz_s, out bool abilitadopocalibraz_c);
            bool.TryParse(abilitaprimacalibraz_s, out bool abilitaprimacalibraz_c);
            bool.TryParse(statogameforce_s, out bool statogameforce_c);
            bool.TryParse(statogameendur_s, out bool statogameendur_c);
            bool.TryParse(maxofpercentilecomputed_s, out bool maxofpercentilecomputed_c);
            bool.TryParse(provenoneseguite_s, out bool provenoneseguite_c);
            bool.TryParse(endurancenotdone_s, out bool endurancenotdone_c);
            Int32.TryParse(duratamassimaendurance_s, out int duratamassimaendurance_c);
            float.TryParse(sogliarettification_s, out float sogliarettification_c);
            float.TryParse(percentile1_s, out float percentile1_c);
            float.TryParse(percentile2_s, out float percentile2_c);
            float.TryParse(percentile3_s, out float percentile3_c);
            float.TryParse(maxofpercentile_s, out float maxofpercentile_c);
            float.TryParse(maxperendurance_s, out float maxperendurance_c);
            float.TryParse(pressaveraged_s, out float pressaveraged_c);
            bool.TryParse(registrazioneok_s, out bool registrazioneok_c);
            Int32.TryParse(puntiprecedenti_s, out int puntiprecedenti_c);
            bool.TryParse(statodurantecalibrazione_s, out bool statodurantecalibrazione_c);
            bool.TryParse(devicenotfound_s, out bool devicenotfound_c);
            Int32.TryParse(secondidiricerca_s, out int secondidiricerca_c);
            bool.TryParse(sparoinbonus_s, out bool sparoinbonus_c);
            Int32.TryParse(carica_batteria_s, out int carica_batteria_c);
  
            pointver2.life = life_c;
            pointver2.points = points_c;
            pointver2.instrDone = instrDone_c;
            pointver2.press = press_c;
            pointver2.sogliacalibrazioneavvenuta = sogliacalibrazioneavvenuta_c;
            pointver2.sogliaskip = sogliaskip_c; //sogliaskip=5000;                    
            pointver2.sogligonfiaggio = sogligonfiaggio_c; //sogligonfiaggio = 5000;             
            pointver2.valore_massimo = valore_massimo_c;
            pointver2.pressmax = pressmax_c;
            pointver2.skip = skip_c;
            pointver2.abilitaskip = abilitaskip_c;
            pointver2.abilitadopocalibraz = abilitadopocalibraz_c;
            pointver2.abilitaprimacalibraz = abilitaprimacalibraz_c;
            pointver2.statogameforce = statogameforce_c;
            pointver2.statogameendur = statogameendur_c;
            pointver2.maxofpercentilecomputed = maxofpercentilecomputed_c;
            pointver2.provenoneseguite = provenoneseguite_c;
            pointver2.endurancenotdone = endurancenotdone_c;

            pointver2.duratamassimaendurance = duratamassimaendurance_c;
            pointver2.sogliarettification = sogliarettification_c;

            pointver2.percentile1 = percentile1_c;  //attento percentile prima prova ma la vita è la 3
            pointver2.percentile2 = percentile2_c;
            pointver2.percentile3 = percentile3_c; //23000;

            pointver2.maxofpercentile = maxofpercentile_c;//23000;
            pointver2.maxperendurance = maxperendurance_c;
            pointver2.pressaveraged = pressaveraged_c;

            pointver2.registrazioneok = registrazioneok_c;
            pointver2.puntiprecedenti = puntiprecedenti_c;
            pointver2.statodurantecalibrazione = statodurantecalibrazione_c;
            pointver2.devicenotfound = devicenotfound_c;
            pointver2.secondidiricerca = secondidiricerca_c;
            pointver2.sparoinbonus = sparoinbonus_c;
            SOC_level = carica_batteria_c;

         }

        static async void WritePreviousValues()
        {
            //Windows.Globalization.DateTimeFormatting.DateTimeFormatter formatter =
            //    new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");
            StorageFile sampleFile = await localFolder.CreateFileAsync("previus_value.dat", CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(sampleFile, pointver2.points.ToString());
        }


        // Read data from a file
        static async void ReadPreviousValues()
        {
            try
            {
                sampleFile2 = await localFolder.GetFileAsync("previus_value.dat");
                valore_previous = await FileIO.ReadTextAsync(sampleFile2);
                int.TryParse(valore_previous, out int valorePrev_f);
                if (valorePrev_f > 0)
                {
                    pointver2.puntiprecedenti = valorePrev_f;
                    pointver2.valore_massimo_flag = true;
                }
                else
                {
                    //await FileIO.WriteTextAsync(sampleFile, pointver2.valore_massimo.ToString());
                    //pointver2.valore_massimo_flag = false;
                    pointver2.puntiprecedenti = 0;
                }

            }
            catch (FileNotFoundException e)
            {
                sampleFile2 = await localFolder.CreateFileAsync("previus_value.dat", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile2, "0");
            }
            catch (IOException e)
            {
                // Get information from the exception, then throw
                // the info to the parent method.
                if (e.Source != null)
                {
                    Debug.WriteLine("IOException source: {0}", e.Source);
                }
                throw;
            }
        }


        static async void WriteMassimo()
        {
            //Windows.Globalization.DateTimeFormatting.DateTimeFormatter formatter =
            //    new Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime");
            StorageFile sampleFile = await localFolder.CreateFileAsync("config.dat", CreationCollisionOption.ReplaceExisting);

            await FileIO.WriteTextAsync(sampleFile, pointver2.valore_massimo.ToString());
        }


        // Read data from a file
        static async void ReadMassimo()
        {
            try
            {
                sampleFile = await localFolder.GetFileAsync("config.dat");
                valore_m_s = await FileIO.ReadTextAsync(sampleFile);
                float.TryParse(valore_m_s, out float valore_f);
                if (valore_f != 0)
                {
                    pointver2.valore_massimo = valore_f;
                    pointver2.valore_massimo_flag = true;
                }
                else
                {
                    //await FileIO.WriteTextAsync(sampleFile, pointver2.valore_massimo.ToString());
                    pointver2.valore_massimo_flag = false;
                }

            }
            catch (FileNotFoundException e)
            {
                sampleFile = await localFolder.CreateFileAsync("config.dat", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, "0");    
            }
            catch (IOException e)
            {
                // Get information from the exception, then throw
                // the info to the parent method.
                if (e.Source != null)
                {
                    Debug.WriteLine("IOException source: {0}", e.Source);
                }
                throw;
            }
        }

        private void OnResize()
        {
            if (splash != null)
            {
                splashImageRect = splash.ImageLocation;
                PositionImage();
            }
        }

        private void PositionImage()
        {
            var inverseScaleX = 1.0f;
            var inverseScaleY = 1.0f;
            if (isPhone)
            {
                inverseScaleX = inverseScaleX / DXSwapChainPanel.CompositionScaleX;
                inverseScaleY = inverseScaleY / DXSwapChainPanel.CompositionScaleY;
            }

            ExtendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X * inverseScaleX);
            ExtendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y * inverseScaleY);
            ExtendedSplashImage.Height = splashImageRect.Height * inverseScaleY;
            ExtendedSplashImage.Width = splashImageRect.Width * inverseScaleX;
        }

        private async void GetSplashBackgroundColor()
        {
            /*Eliminar esto de aquí!!!*/
          //  await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();

           
            try
            {
                StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///AppxManifest.xml"));
                string manifest = await FileIO.ReadTextAsync(file);
                int idx = manifest.IndexOf("SplashScreen");
                manifest = manifest.Substring(idx);
                idx = manifest.IndexOf("BackgroundColor");
                if (idx < 0)  // background is optional
                    return;
                manifest = manifest.Substring(idx);
                idx = manifest.IndexOf("\"");
                manifest = manifest.Substring(idx + 1);
                idx = manifest.IndexOf("\"");
                manifest = manifest.Substring(0, idx);
                int value = 0;
                bool transparent = false;
                if (manifest.Equals("transparent"))
                    transparent = true;
                else if (manifest[0] == '#') // color value starts with #
                    value = Convert.ToInt32(manifest.Substring(1), 16) & 0x00FFFFFF;
                else
                    return; // at this point the value is 'red', 'blue' or similar, Unity does not set such, so it's up to user to fix here as well
                byte r = (byte)(value >> 16);
                byte g = (byte)((value & 0x0000FF00) >> 8);
                byte b = (byte)(value & 0x000000FF);

                await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.High, delegate ()
                {
                    byte a = (byte)(transparent ? 0x00 : 0xFF);
                    ExtendedSplashGrid.Background = new SolidColorBrush(Color.FromArgb(a, r, g, b));
                });
            }
            catch (Exception)
            { }
        }

        public SwapChainPanel GetSwapChainPanel()
        {
            return DXSwapChainPanel;
        }

        public void RemoveSplashScreen()
        {
            DXSwapChainPanel.Children.Remove(ExtendedSplashGrid);
            if (onResizeHandler != null)
            {
                Window.Current.SizeChanged -= onResizeHandler;
                onResizeHandler = null;
            }
        }

        #region BLUET


        async void App_Suspending(Object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {

            /*
                   try
                   {
                       // BT_Code: Must write the CCCD in order for server to send notifications.
                       // We receive them in the ValueChanged event handler.
                       // Note that this sample configures either Indicate or Notify, but not both.
                       var result6 = await
                               selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                   GattClientCharacteristicConfigurationDescriptorValue.None);
                       if (result6 == GattCommunicationStatus.Success)
                       {
                           subscribedForNotifications = false;
                           //RemoveValueChangedHandler();
                           Debug.Write("Successfully un-registered for notifications");
                           //registeredCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                       }
                       else
                       {
                           Debug.Write($"Error un-registering for notifications: {result6}");
                       }
                   }
                   catch (UnauthorizedAccessException ex)
                   {
                       // This usually happens when a device reports that it support notify, but it actually doesn't.
                       Debug.Write("support notify, but it actually doesn't");
                   }

              */
        }



        /// <summary>
        /// Starts a device watcher that looks for all nearby Bluetooth devices (paired or unpaired). 
        /// Attaches event handlers to populate the device collection.
        /// </summary>
        private void StartBleDeviceWatcher()
        {
            // Additional properties we would like about the device.
            // Property strings are documented here https://msdn.microsoft.com/en-us/library/windows/desktop/ff521659(v=vs.85).aspx
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable" };

            // BT_Code: Example showing paired and non-paired in a single query.
            string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";

            deviceWatcher =
                    DeviceInformation.CreateWatcher(
                        aqsAllBluetoothLEDevices,
                        requestedProperties,
                        DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Start over with an empty collection.
            KnownDevices.Clear();

            // Start the watcher.
            deviceWatcher.Start();
        }

        /// <summary>
        /// Stops watching for all nearby Bluetooth devices.
        /// </summary>
        private void StopBleDeviceWatcher()
        {
            if (deviceWatcher != null)
            {
                // Unregister the event handlers.
                deviceWatcher.Added -= DeviceWatcher_Added;
                deviceWatcher.Updated -= DeviceWatcher_Updated;
                deviceWatcher.Removed -= DeviceWatcher_Removed;
                deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
                deviceWatcher.Stopped -= DeviceWatcher_Stopped;

                // Stop the watcher.
                deviceWatcher.Stop();
                deviceWatcher = null;
            }
        }

        private BluetoothLEDeviceDisplay FindBluetoothLEDeviceDisplay(string id)
        {
            foreach (BluetoothLEDeviceDisplay bleDeviceDisplay in KnownDevices)
            {
                if (bleDeviceDisplay.Id == id)
                {
                    return bleDeviceDisplay;
                }
            }
            return null;
        }

        private DeviceInformation FindUnknownDevices(string id)
        {
            foreach (DeviceInformation bleDeviceInfo in UnknownDevices)
            {
                if (bleDeviceInfo.Id == id)
                {
                    return bleDeviceInfo;
                }
            }
            return null;
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (this)
                {
                    Debug.WriteLine(String.Format("Added {0}{1}", deviceInfo.Id, deviceInfo.Name));

                    // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                    if (sender == deviceWatcher)
                    {
                        // Make sure device isn't already present in the list.
                        if (FindBluetoothLEDeviceDisplay(deviceInfo.Id) == null)
                        {
                            if (deviceInfo.Name != string.Empty)
                            {
                                // If device has a friendly name display it immediately.
                                KnownDevices.Add(new BluetoothLEDeviceDisplay(deviceInfo));
                            }
                            else
                            {
                                // Add it to a list in case the name gets updated later. 
                                UnknownDevices.Add(deviceInfo);
                            }
                        }

                    }
                }
            });
            //if (deviceInfo.Id == "BluetoothLE#BluetoothLE00:19:86:00:2c:b4-c0:83:41:31:21:48" && pointver2.abilitaprimacalibraz == true)//&& deviceInfo.Name == "MOV0310")//pallina verde
            //if (deviceInfo.Id == "BluetoothLE#BluetoothLE9c:b6:d0:e3:55:5e-c0:83:19:31:2c:48" && pointver2.abilitaprimacalibraz == true)//&& deviceInfo.Name == "MOV0310")//pallina viola
            //if (deviceInfo.Id == "BluetoothLE#BluetoothLE9c:b6:d0:e3:55:5e-c0:83:1f:30:51:47" && pointver2.abilitaprimacalibraz == true)//&& deviceInfo.Name == "MOV0310")//palla arancio
            if (deviceInfo.Name == "MOVB310" && pointver2.abilitaprimacalibraz == true)//id generico dell'oggetto palla MOV0310
                                                                                       //if (deviceInfo.Name == "MOVB310" && pointver2.abilitaprimacalibraz == true)//id generico dell'oggetto palla MOVB0310

            {

                ServiceCollection.Clear();
                CharacteristicCollection.Clear();

                StopBleDeviceWatcher();
                Debug.Write("\n OOOK");

                string[] intermedia;
                intermedia = deviceInfo.Id.Split('-');
                BTMacAddress = intermedia[1];
                bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
                if (bluetoothLeDevice != null)
                {


                    // Note: BluetoothLEDevice.GattServices property will return an empty list for unpaired devices. For all uses we recommend using the GetGattServicesAsync method.
                    // BT_Code: GetGattServicesAsync returns a list of all the supported services of the device (even if it's not paired to the system).
                    // If the services supported by the device are expected to change during BT usage, subscribe to the GattServicesChanged event.
                    GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);

                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        var services = result.Services;
                        Debug.Write(String.Format("Found {0} services", services.Count));
                        foreach (var service in services)
                        {
                            ServiceCollection.Add(new BluetoothLEAttributeDisplay(service));
                        }//occhio che magari è occupato il servizio da una chiusura non corretta

                        //parte da spostare
                    }
                    else
                    {
                        Debug.Write("Device unreachable");
                    }


                    if (ServiceCollection.Count > 0)
                    {
                        //show service
                        for (int i = 0; i < ServiceCollection.Count; i++)
                        {
                            Debug.Write("\n service " + i.ToString());
                            Debug.Write(ServiceCollection[i].Name);

                        }
                    }
                    //FINE SERVIZI
                    if (ServiceCollection.Count > 1)
                    {
                        var attributeInfoDisp = (BluetoothLEAttributeDisplay)ServiceCollection[2]; //SELEX servizio     [2]pallina  [9]cell

                        // CharacteristicCollection.Clear();


                        IReadOnlyList<GattCharacteristic> characteristics = null;
                        try
                        {
                            // Ensure we have access to the device.
                            var accessStatus = await attributeInfoDisp.service.RequestAccessAsync();
                            if (accessStatus == DeviceAccessStatus.Allowed)
                            {
                                // BT_Code: Get all the child characteristics of a service. Use the cache mode to specify uncached characterstics only 
                                // and the new Async functions to get the characteristics of unpaired devices as well. 
                                var result1 = await attributeInfoDisp.service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                                if (result1.Status == GattCommunicationStatus.Success)
                                {
                                    characteristics = result1.Characteristics;
                                }
                                else
                                {
                                    Debug.Write("Error accessing service.");

                                    // On error, act as if there are no characteristics.
                                    characteristics = new List<GattCharacteristic>();
                                }
                            }
                            else
                            {
                                // Not granted access
                                Debug.Write("Error accessing service.");

                                // On error, act as if there are no characteristics.
                                characteristics = new List<GattCharacteristic>();

                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Write("Restricted service. Can't read characteristics: ");
                            // On error, act as if there are no characteristics.
                            characteristics = new List<GattCharacteristic>();
                        }

                        foreach (GattCharacteristic c in characteristics)
                        {
                            CharacteristicCollection.Add(new BluetoothLEAttributeDisplay(c));
                        }


                        //show charact
                        if (CharacteristicCollection.Count > 0)

                        {
                            for (int j = 0; j < CharacteristicCollection.Count; j++)
                            {
                                Debug.Write("\n characteristic " + j.ToString());
                                Debug.Write(CharacteristicCollection[j].characteristic.Uuid);

                            }
                        }
                        //FINE CHARACTERISTIC
                        if (CharacteristicCollection.Count > 2)
                        {
                            // var attributeInfoDisp2 = (BluetoothLEAttributeDisplay)CharacteristicCollection[3];//SELECT CHARACT  [3] pallina   [0]cell
                            //var attributeInfoDisp2 = CharacteristicCollection[1];//SELECT CHARACT 00e00000-0001-11e1-ac36-0002a5d5c51

                            int giustacar = 0;
                            int battery = 0;

                            for (int j = 0; j < CharacteristicCollection.Count; j++)
                            {
                                if (CharacteristicCollection[j].characteristic.Uuid.ToString() == "00e00000-0001-11e1-ac36-0002a5d5c51b")
                                    giustacar = j;
                                if (CharacteristicCollection[j].characteristic.Uuid.ToString() == "00020000-0001-11e1-ac36-0002a5d5c51b") battery = j;

                            }


                            var attributeInfoDisp2 = CharacteristicCollection[giustacar];
                            var attributeBattery = CharacteristicCollection[battery];
                            selectedCharacteristic = attributeInfoDisp2.characteristic;
                            //selectedCharacteristic = attributeBattery.characteristic;

                            // Get all the child descriptors of a characteristics. Use the cache mode to specify uncached descriptors only 
                            // and the new Async functions to get the descriptors of unpaired devices as well. 
                            var result3 = await selectedCharacteristic.GetDescriptorsAsync(BluetoothCacheMode.Uncached);
                            if (result3.Status != GattCommunicationStatus.Success)
                            {
                                Debug.Write("Descriptor read failure: " + result3.Status.ToString());
                            }

                            // BT_Code: There's no need to access presentation format unless there's at least one. 
                            presentationFormat = null;
                            if (selectedCharacteristic.PresentationFormats.Count > 0)
                            {

                                if (selectedCharacteristic.PresentationFormats.Count.Equals(1))
                                {
                                    // Get the presentation format since there's only one way of presenting it
                                    presentationFormat = selectedCharacteristic.PresentationFormats[0];
                                }
                                else
                                {
                                    // It's difficult to figure out how to split up a characteristic and encode its different parts properly.
                                    // In this case, we'll just encode the whole thing to a string to make it easy to print out.
                                }
                            }



                            if (selectedCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                            { Debug.Write("\n IS READ \n"); }

                            //FINE PROPERTIES


                            GattReadResult result4 = await selectedCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
                            if (result4.Status == GattCommunicationStatus.Success)
                            {
                                Debug.Write(result4.Value.ToString());
                                //string formattedResult = FormatValueByPresentation(result4.Value, presentationFormat);
                                //Debug.Write($"Read result: {formattedResult}");
                            }
                            else
                            {
                                //   Debug.Write("\n epic fail \n");
                            }

                            //INIZIO REGISTRAZIONE 



                            if (!subscribedForNotifications)
                            {
                                Debug.Write("\n inizio registrazione \n");
                                // initialize status
                                GattCommunicationStatus status5 = GattCommunicationStatus.Unreachable;
                                var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
                                if (selectedCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                                {
                                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
                                }

                                else if (selectedCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                                {
                                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
                                }

                                try
                                {
                                    // BT_Code: Must write the CCCD in order for server to send indications.
                                    // We receive them in the ValueChanged event handler.
                                    status5 = await selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);

                                    if (status5 == GattCommunicationStatus.Success)
                                    {
                                        // AddValueChangedHandler();
                                        Debug.Write("\n Successfully subscribed for value changes \n");
                                        registeredCharacteristic = selectedCharacteristic;
                                        registeredCharacteristic.ValueChanged += Characteristic_ValueChanged;
                                        subscribedForNotifications = true;

                                        Debug.WriteLine("\n prova stream \n");
                                        await Task.Delay(2000);//2000e'ok
                                        if (pippo == true)
                                        {
                                            Debug.WriteLine("\n qui \n");
                                        }
                                        else
                                        {
                                            Debug.WriteLine("\n no stream \n");

                                            registeredCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                                            //PARTE SENZA TRY PERCHE' non dovrebbero esseci problemi
                                            var result6 = await
                           selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                               GattClientCharacteristicConfigurationDescriptorValue.None);
                                            if (result6 == GattCommunicationStatus.Success)
                                            {
                                                subscribedForNotifications = false;
                                                //RemoveValueChangedHandler();
                                                Debug.Write("Successfully un-registered for notifications");
                                                //registeredCharacteristic.ValueChanged -= Characteristic_ValueChanged;


                                            }


                                            StartBleDeviceWatcher();
                                        }

                                    }
                                    else
                                    {
                                        Debug.Write($"Error registering for value changes: {status5}");
                                        StartBleDeviceWatcher();
                                    }
                                }
                                catch (UnauthorizedAccessException ex)
                                {
                                    // This usually happens when a device reports that it support indicate, but it actually doesn't.
                                    Debug.Write("support indicate, but it actually doesn't");
                                }
                            }
                            else
                            {
                                try
                                {
                                    // BT_Code: Must write the CCCD in order for server to send notifications.
                                    // We receive them in the ValueChanged event handler.
                                    // Note that this sample configures either Indicate or Notify, but not both.
                                    var result6 = await
                                            selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                                GattClientCharacteristicConfigurationDescriptorValue.None);
                                    if (result6 == GattCommunicationStatus.Success)
                                    {
                                        subscribedForNotifications = false;
                                        //RemoveValueChangedHandler();
                                        Debug.Write("Successfully un-registered for notifications");
                                        StartBleDeviceWatcher();
                                    }
                                    else
                                    {
                                        Debug.Write($"Error un-registering for notifications: {result6}");
                                    }
                                }
                                catch (UnauthorizedAccessException ex)
                                {
                                    // This usually happens when a device reports that it support notify, but it actually doesn't.
                                    Debug.Write("support notify, but it actually doesn't");
                                }
                            }


                            //*****************************************************************************************
                            //*****************************************************************************************

                            selectedCharacteristic2 = attributeBattery.characteristic;

                            // Get all the child descriptors of a characteristics. Use the cache mode to specify uncached descriptors only 
                            // and the new Async functions to get the descriptors of unpaired devices as well. 
                            var result7 = await selectedCharacteristic2.GetDescriptorsAsync(BluetoothCacheMode.Uncached);
                            if (result7.Status != GattCommunicationStatus.Success)
                            {
                                Debug.Write("Descriptor read failure: " + result7.Status.ToString());
                            }

                            // BT_Code: There's no need to access presentation format unless there's at least one. 
                            presentationFormat = null;
                            if (selectedCharacteristic2.PresentationFormats.Count > 0)
                            {

                                if (selectedCharacteristic2.PresentationFormats.Count.Equals(1))
                                {
                                    // Get the presentation format since there's only one way of presenting it
                                    presentationFormat2 = selectedCharacteristic2.PresentationFormats[0];
                                }
                                else
                                {
                                    // It's difficult to figure out how to split up a characteristic and encode its different parts properly.
                                    // In this case, we'll just encode the whole thing to a string to make it easy to print out.
                                }
                            }



                            if (selectedCharacteristic2.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                            { Debug.Write("\n IS READ \n"); }

                            //FINE PROPERTIES


                            GattReadResult result8 = await selectedCharacteristic2.ReadValueAsync(BluetoothCacheMode.Uncached);
                            if (result8.Status == GattCommunicationStatus.Success)
                            {
                                Debug.Write(result8.Value.ToString());
                                //string formattedResult = FormatValueByPresentation(result4.Value, presentationFormat);
                                //Debug.Write($"Read result: {formattedResult}");
                            }
                            else
                            {
                                //   Debug.Write("\n epic fail \n");
                            }

                            //INIZIO REGISTRAZIONE 



                            if (!subscribedForNotifications2)
                            {
                                Debug.Write("\n inizio registrazione \n");
                                // initialize status
                                GattCommunicationStatus status9 = GattCommunicationStatus.Unreachable;
                                var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
                                if (selectedCharacteristic2.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                                {
                                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
                                }

                                else if (selectedCharacteristic2.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                                {
                                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
                                }

                                try
                                {
                                    // BT_Code: Must write the CCCD in order for server to send indications.
                                    // We receive them in the ValueChanged event handler.
                                    status9 = await selectedCharacteristic2.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);

                                    if (status9 == GattCommunicationStatus.Success)
                                    {
                                        // AddValueChangedHandler();
                                        Debug.Write("\n Successfully subscribed for value changes \n");
                                        registeredCharacteristic2 = selectedCharacteristic2;
                                        registeredCharacteristic2.ValueChanged += Characteristic_ValueChanged;
                                        subscribedForNotifications2 = true;

                                        Debug.WriteLine("\n prova stream \n");
                                        await Task.Delay(2000);//2000e'ok
                                        if (pippo == true)
                                        {
                                            Debug.WriteLine("\n qui \n");
                                        }
                                        else
                                        {
                                            Debug.WriteLine("\n no stream \n");

                                            registeredCharacteristic2.ValueChanged -= Characteristic_ValueChanged;
                                            //PARTE SENZA TRY PERCHE' non dovrebbero esseci problemi
                                            var result10 = await
                           selectedCharacteristic2.WriteClientCharacteristicConfigurationDescriptorAsync(
                               GattClientCharacteristicConfigurationDescriptorValue.None);
                                            if (result10 == GattCommunicationStatus.Success)
                                            {
                                                subscribedForNotifications2 = false;
                                                //RemoveValueChangedHandler();
                                                Debug.Write("Successfully un-registered for notifications");
                                                //registeredCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                                            }
                                            StartBleDeviceWatcher();
                                        }

                                    }
                                    else
                                    {
                                        Debug.Write($"Error registering for value changes: {status9}");
                                        StartBleDeviceWatcher();
                                    }
                                }
                                catch (UnauthorizedAccessException ex)
                                {
                                    // This usually happens when a device reports that it support indicate, but it actually doesn't.
                                    Debug.Write("support indicate, but it actually doesn't");
                                }
                            }
                            else
                            {
                                try
                                {
                                    // BT_Code: Must write the CCCD in order for server to send notifications.
                                    // We receive them in the ValueChanged event handler.
                                    // Note that this sample configures either Indicate or Notify, but not both.
                                    var result10 = await
                                            selectedCharacteristic2.WriteClientCharacteristicConfigurationDescriptorAsync(
                                                GattClientCharacteristicConfigurationDescriptorValue.None);
                                    if (result10 == GattCommunicationStatus.Success)
                                    {
                                        subscribedForNotifications2 = false;
                                        //RemoveValueChangedHandler();
                                        Debug.Write("Successfully un-registered for notifications");
                                        StartBleDeviceWatcher();
                                    }
                                    else
                                    {
                                        Debug.Write($"Error un-registering for notifications: {result10}");
                                    }
                                }
                                catch (UnauthorizedAccessException ex)
                                {
                                    // This usually happens when a device reports that it support notify, but it actually doesn't.
                                    Debug.Write("support notify, but it actually doesn't");
                                }
                            }


                        }
                        else { //StartBleDeviceWatcher(); 
                        }
                    }
                    else { StartBleDeviceWatcher();
                    }
                }//



            }
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (this)
                {
                    Debug.WriteLine(String.Format("Updated {0}{1}", deviceInfoUpdate.Id, ""));

                    // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                    if (sender == deviceWatcher)
                    {
                        BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                        if (bleDeviceDisplay != null)
                        {
                            // Device is already being displayed - update UX.
                            bleDeviceDisplay.Update(deviceInfoUpdate);
                            return;
                        }

                        DeviceInformation deviceInfo = FindUnknownDevices(deviceInfoUpdate.Id);
                        if (deviceInfo != null)
                        {
                            deviceInfo.Update(deviceInfoUpdate);
                            // If device has been updated with a friendly name it's no longer unknown.
                            if (deviceInfo.Name != String.Empty)
                            {
                                KnownDevices.Add(new BluetoothLEDeviceDisplay(deviceInfo));
                                UnknownDevices.Remove(deviceInfo);
                            }
                        }
                    }
                }
            });

        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (this)
                {
                    Debug.WriteLine(String.Format("Removed {0}{1}", deviceInfoUpdate.Id, ""));

                    // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                    if (sender == deviceWatcher)
                    {
                        // Find the corresponding DeviceInformation in the collection and remove it.
                        BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                        if (bleDeviceDisplay != null)
                        {
                            KnownDevices.Remove(bleDeviceDisplay);
                        }

                        DeviceInformation deviceInfo = FindUnknownDevices(deviceInfoUpdate.Id);
                        if (deviceInfo != null)
                        {
                            UnknownDevices.Remove(deviceInfo);
                        }
                    }
                }
            });
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object e)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {
                    Debug.Write(" devices found. Enumeration completed.");
                    StartBleDeviceWatcher();//aggiunto all'ultimo
                                            // NotifyType.StatusMessage) ;

                }
            });
        }

        private async void DeviceWatcher_Stopped(DeviceWatcher sender, object e)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == deviceWatcher)
                {

                    Debug.Write(" No longer watching for devices.");
                    //rootPage.NotifyUser($"No longer watching for devices.",
                    //        sender.Status == DeviceWatcherStatus.Aborted ? NotifyType.ErrorMessage : NotifyType.StatusMessage);
                }
            });
        }

        private void RemoveValueChangedHandler()
        {
           
            if (subscribedForNotifications)
            {
                registeredCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                registeredCharacteristic = null;
                subscribedForNotifications = false;
            }
        }

        private string FormatValueByPresentation(IBuffer buffer, GattPresentationFormat format)
        {
            // BT_Code: For the purpose of this sample, this function converts only UInt32 and
            // UTF-8 buffers to readable text. It can be extended to support other formats if your app needs them.
            byte[] data;
            CryptographicBuffer.CopyToByteArray(buffer, out data);
            if (format != null)
            {
                if (format.FormatType == GattPresentationFormatTypes.UInt32 && data.Length >= 4)
                {
                    return BitConverter.ToInt32(data, 0).ToString();
                }
                else if (format.FormatType == GattPresentationFormatTypes.Utf8)
                {
                    try
                    {
                        return Encoding.UTF8.GetString(data);
                    }
                    catch (ArgumentException)
                    {
                        return "(error: Invalid UTF-8 string)";
                    }
                }
                else
                {
                    // Add support for other format types as needed.
                    return "Unsupported format: " + CryptographicBuffer.EncodeToHexString(buffer);
                }
            }
            else if (data != null)
            {
                // We don't know what format to use. Let's try some well-known profiles, or default back to UTF-8.
                if (selectedCharacteristic.Uuid.Equals(GattCharacteristicUuids.HeartRateMeasurement))
                {
                    try
                    {
                        return "Heart Rate: ";
                    }
                    catch (ArgumentException)
                    {
                        return "Heart Rate: (unable to parse)";
                    }
                }
                else if (selectedCharacteristic.Uuid.Equals(GattCharacteristicUuids.BatteryLevel))
                {
                    try
                    {
                        // battery level is encoded as a percentage value in the first byte according to
                        // https://www.bluetooth.com/specifications/gatt/viewer?attributeXmlFile=org.bluetooth.characteristic.battery_level.xml
                        return "Battery Level: " + data[0].ToString() + "%";
                    }
                    catch (ArgumentException)
                    {
                        return "Battery Level: (unable to parse)";
                    }
                }
                // This is our custom calc service Result UUID. Format it like an Int
                else if (selectedCharacteristic.Uuid.Equals(Constants.ResultCharacteristicUuid))
                {
                    return BitConverter.ToInt32(data, 0).ToString();
                }
                // No guarantees on if a characteristic is registered for notifications.
                else if (registeredCharacteristic != null)
                {
                    // This is our custom calc service Result UUID. Format it like an Int
                    if (registeredCharacteristic.Uuid.Equals(Constants.ResultCharacteristicUuid))
                    {
                        return BitConverter.ToInt32(data, 0).ToString();
                    }
                }
                else
                {
                    try
                    {
                        return "Unknown format: " + Encoding.UTF8.GetString(data);
                    }
                    catch (ArgumentException)
                    {
                        return "Unknown format";
                    }
                }
            }
            else
            {
                return "Empty data received";
            }
            return "Unknown format";
        }
        private bool subscribedForNotifications = false;
        private bool subscribedForNotifications2 = false;

        private async void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            //IReadOnlyList<User> users = await User.FindAllAsync();

            //var current = users.Where(p => p.AuthenticationStatus == UserAuthenticationStatus.LocallyAuthenticated &&
            //                            p.Type == UserType.LocalUser).FirstOrDefault();

            //// user may have username
            //var data = await current.GetPropertyAsync(KnownUserProperties.AccountName);

            if (pippo == false) { pippo = true; }

            if (sender.Uuid.ToString() == "00020000-0001-11e1-ac36-0002a5d5c51b")
            {
                DataReader dataReader = DataReader.FromBuffer(args.CharacteristicValue);
                byte[] buffer = new byte[(int)args.CharacteristicValue.Length];
                dataReader.ReadBytes(buffer);
                timestampBat = (float)ReadInt16(buffer, 0);
                Debug.Write("\n Battery timestamp:" + c.ToString() + "\n");
                soc = (float)ReadInt16(buffer, 2) / 10.0f;
                Debug.Write("\n SOC:" + soc.ToString() + "\n");
                volt = (float)ReadInt16(buffer, 4);
                Debug.Write("\n Volt:" + volt.ToString() + "\n");
                amps = (float)ReadInt16(buffer, 6);
                Debug.Write("\n Amp:" + amps.ToString() + "\n");
                stat = (float)ReadInt16(buffer, 8);
                Debug.Write("\n Status:" + stat.ToString() + "\n");
                
                //First time reader flag
                socFlag = true;


                lista2.Add(new misura2()
                {
                    timestampBat = timestampBat,
                    soc = soc,
                    volt = volt,
                    amps = amps,
                    stat = stat,
                    secondi = DateTime.Now.Second,
                    millisecondi = DateTime.Now.Millisecond,
                });

            }

            if (sender.Uuid.ToString() == "00e00000-0001-11e1-ac36-0002a5d5c51b")
            {
                DataReader dataReader = DataReader.FromBuffer(args.CharacteristicValue);
                byte[] buffer = new byte[(int)args.CharacteristicValue.Length];
                dataReader.ReadBytes(buffer);
                p = (float)ReadInt16(buffer, 14);
                Debug.Write("\n press:" + p.ToString() + "\n");
                c = (float)ReadInt16(buffer, 0);
                Debug.Write("\n time:" + c.ToString() + "\n");
                ax = (float)ReadInt16(buffer, 2) / 100.0f;
                Debug.Write("\n Ax:" + ax.ToString() + "\n");
                ay = (float)ReadInt16(buffer, 4) / 100.0f;
                Debug.Write("\n Ay:" + ay.ToString() + "\n");
                az = (float)ReadInt16(buffer, 6) / 100.0f;
                Debug.Write("\n Az:" + az.ToString() + "\n");
                gx = (float)ReadInt16(buffer, 8) / 100.0f;
                Debug.Write("\n Gx:" + gx.ToString() + "\n");
                gy = (float)ReadInt16(buffer, 10) / 100.0f;
                Debug.Write("\n Gy:" + gy.ToString() + "\n");
                gz = (float)ReadInt16(buffer, 12) / 100.0f;
                Debug.Write("\n Gz:" + gz.ToString() + "\n");
                moduleofgy = Math.Sqrt(gx * gx + gy * gy + gz * gz);
                moduleofacc = Math.Sqrt(ax * ax + ay * ay + az * az);

                lista.Add(new misura()
                {
                    timestamp = c,
                    pressione = p,
                    pressionevera = pdiprima,
                    //pressionevera = pointver2.pressmax,
                    secondi = DateTime.Now.Second,
                    millisecondi = DateTime.Now.Millisecond,
                    statogioco = pointver2.life,
                    statodicalibrazione = pointver2.statodurantecalibrazione,
                    statogiocopresa = pointver2.statogameforce,
                    statogiocofatica = pointver2.statogameendur,
                    accx = ax,
                    accy = ay,
                    accz = ay,
                    girx = gx,
                    giry = gy,
                    girz = gz,
                    statotimestamp = timestampbool

                });
                pointver2.press = p;
            }

                //Debug.WriteLine(moduleofgy.ToString());
                //Debug.WriteLine(moduleofacc.ToString());
                if (moduleofacc < 9 && moduleofgy > 40) { pointver2.sparoinbonus = true; }


                //versione easy
                //if (p < pointver2.sogliarettification) { p = p + 65535f; }
                //versione complessa
                //anche questa dai
                if (pdiprima > pointver2.sogliarettification && p < -pointver2.sogliarettification) { correzionevalori = true; }
                if (pdiprima < -pointver2.sogliarettification && p > pointver2.sogliarettification) { correzionevalori = false; }
                pdiprima = p;
                if (correzionevalori == true) { p = p + 65535; }
                //if (correzionevalori == true) { p = p + 65535f; }


                if (pointver2.statodurantecalibrazione == true && p < pointver2.sogliacalibrazioneavvenuta)
                { pointver2.statodurantecalibrazione = false; }



                float cdiprima;
                //gestire il primo avvio a lista vuota in modo da evitare lisa.count  pippo7
                if (pippo7 == false) { cdiprima = c; pippo7 = true; }
                else { cdiprima = lista.Last().timestamp; }

                // if ((c - cdiprima) > 4) { timestampbool = true; } else { timestampbool = false; }//fail in timestamp
                if ((c - cdiprima) > 10) { timestampbool = true; } else { timestampbool = false; }//fail in timestamp


                if (pointver2.statogameforce == true)
                {
                    if (pointver2.life == 3)
                    {
                        valoristretta1.Add(p);
                        pointver2.percentile1 = Percentile(valoristretta1.ToArray(), 0.90f);
                        if (timestampbool && failstamplife3 == false) { failstamplife3 = true; }
                    }
                    if (pointver2.life == 2)
                    {
                        valoristretta2.Add(p);
                        pointver2.percentile2 = Percentile(valoristretta2.ToArray(), 0.90f);
                        if (timestampbool && failstamplife2 == false) { failstamplife2 = true; }
                    }
                    if (pointver2.life == 1)
                    {
                        valoristretta3.Add(p);
                        pointver2.percentile3 = Percentile(valoristretta3.ToArray(), 0.90f);
                        if (timestampbool && failstamplife1 == false) { failstamplife1 = true; }
                    }

                }
                if (pointver2.life == 0) //endurance
                {
                    if (timestampbool && failstamplife0 == false) { failstamplife0 = true; }
                    if (pippo3 == false) { pippo3 = true; tempomassimoendurance = DateTime.Now; }
                    if (pippo3 == true && (DateTime.Now - tempomassimoendurance).Seconds > pointver2.duratamassimaendurance)
                    {
                        pointver2.endurancenotdone = true;
                    }

                    if (p > pointver2.maxofpercentile * 0.75 && pointver2.statogameendur == false) { pointver2.statogameendur = true; tempoiniziale = DateTime.Now; }
                    if (p < pointver2.maxofpercentile * 0.75 && pointver2.statogameendur == true)
                    {
                        if (pippo2 == false)
                        {
                            probabiletempofinale = DateTime.Now;
                            pippo2 = true;
                        }
                        // tempotrascorso = DateTime.Now - tempoiniziale; //pointver2.statogameendur = false;
                    }
                    if (p > pointver2.maxofpercentile * 0.75 && pippo2 == true && pointver2.statogameendur == true) { pippo2 = false; }
                    if ((DateTime.Now - probabiletempofinale).Seconds > 5 && pippo2 == true && pointver2.statogameendur == true) { tempotrascorso = probabiletempofinale - tempoiniziale; pointver2.statogameendur = false; }


                }
                //(DateTime.Now - tempofinale).Seconds < 3
               

                Debug.Write("\n SONO QUI NELLE NOTIFICHE \n");
                //Debug.Write("\n SONO QUI NELLE NOTIFICHE \n" +p.ToString());


                if (pointver2.life < 0)
                {
                    try
                    {
                    //// BT_Code: Must write the CCCD in order for server to send notifications.
                    //// We receive them in the ValueChanged event handler.
                    //// Note that this sample configures either Indicate or Notify, but not both.
                    //var result6 = await
                    //        selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    //            GattClientCharacteristicConfigurationDescriptorValue.None);
                    //if (result6 == GattCommunicationStatus.Success)
                    //{
                    // subscribedForNotifications = false;
                    //    //RemoveValueChangedHandler();
                    //    Debug.Write("Successfully un-registered for notifications");
                        if (subscribedForNotifications)
                        {
                            registeredCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                            registeredCharacteristic = null;
                            subscribedForNotifications = false;
                        }
                        if (subscribedForNotifications2)
                        {
                            registeredCharacteristic2.ValueChanged -= Characteristic_ValueChanged;
                            registeredCharacteristic2 = null;
                            subscribedForNotifications2 = false;
                        }
                    //}
                    //else
                    //{
                    //    Debug.Write($"Error un-registering for notifications: {result6}");
                    //}
                    //var result16 = await
                    //        selectedCharacteristic2.WriteClientCharacteristicConfigurationDescriptorAsync(
                    //            GattClientCharacteristicConfigurationDescriptorValue.None);
                    //if (result16 == GattCommunicationStatus.Success)
                    //{
                    //subscribedForNotifications2 = false;
                    //    //RemoveValueChangedHandler();
                    //    Debug.Write("Successfully un-registered for notifications");



                    //}
                    //else
                    //{
                    //    Debug.Write($"Error un-registering for notifications: {result16}");
                    //}
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        // This usually happens when a device reports that it support notify, but it actually doesn't.
                        Debug.Write("support notify, but it actually doesn't");
                    }



                    if (pippo5 == false)
                    {
                        Observable observable = new Observable();
                        Observer observer = new Observer();
                        observable.SomethingHappened += observer.HandleEvent;
                        observable.DoSomething();

                        pippo5 = true;
                        contador++;
                        if (pointver2.valore_massimo_flag == false)
                        {
                            StorageFile sampleFile2 = await localFolder.CreateFileAsync("config.dat", CreationCollisionOption.ReplaceExisting);
                            pointver2.valore_massimo = pointver2.pressmax;
                            await FileIO.WriteTextAsync(sampleFile2, pointver2.valore_massimo.ToString());
                        }
                    }

                
                }
        }

        public static unsafe int ReadInt16(byte[] source, int index)
        {
            int num = index;
            index += 2;
            fixed (byte* numPtr = &source[0])
                return (int)*(short*)(numPtr + num);
        }


        public float Percentile(float[] sequence, float excelPercentile)
        {
            Array.Sort(sequence);
            int N = sequence.Length;
            float n = (N - 1) * excelPercentile + 1;
            //  float n = (N + 1) * excelPercentile;
            if (n == 1d) return sequence[0];
            else if (n == N) return sequence[N - 1];
            else
            {
                int k = (int)n;
                float d = n - k;
                return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
            }
        }

        public class misura
        {
            public float timestamp { get; set; }
            public float pressione { get; set; }
            public float pressionevera { get; set; } //da eliminare
            public int millisecondi { get; set; }
            public int secondi { get; set; }
            public int statogioco { get; set; } //vite
            public bool statodicalibrazione { get; set; }
            public bool statogiocopresa { get; set; }
            public bool statogiocofatica { get; set; }
            public bool statotimestamp { get; set; }

            public float accx { get; set; }
            public float accy { get; set; }
            public float accz { get; set; }
            public float girx { get; set; }
            public float giry { get; set; }
            public float girz { get; set; }
        }

        public class misura2
        {
            public float timestampBat { get; set; }
            public float soc { get; set; }
            public float volt { get; set; }
            public float amps { get; set; }
            public float stat { get; set; }
            public int millisecondi { get; set; }
            public int secondi { get; set; }
        }

        static public List<misura> lista = new List<misura>();
        static public List<misura2> lista2 = new List<misura2>();
        static public List<float> valoristretta1 = new List<float>();
        static public List<float> valoristretta2 = new List<float>();
        static public List<float> valoristretta3 = new List<float>();
        static public List<float> valorifatica = new List<float>();
        static public DateTime tempoiniziale;
        static public TimeSpan tempotrascorso;
        static public DateTime probabiletempofinale;
        static public DateTime tempomassimoendurance;
        static public DateTime nomefile;

        static public bool pippo = false;
        static public bool pippo2 = false;
        static public bool pippo3 = false;
        static public bool pippo4 = false;
        static public bool pippo5 = false;
        static public bool pippo6 = false;
        static public bool pippo7 = false;

        static public bool timestampbool = false;
        static public bool failstamplife3 = false;
        static public bool failstamplife2 = false;
        static public bool failstamplife1 = false;
        static public bool failstamplife0 = false;

        static public float pdiprima = 0;
        static public bool correzionevalori = false;

        //internal async void SaveFile()
        //{
        //    var nomefile = DateTime.Now;
        //    string filename1 = nomefile.Day.ToString() + "-" + nomefile.Month.ToString() + "-" + nomefile.Year.ToString() + "-" + nomefile.Hour.ToString() + "-" + nomefile.Minute.ToString() + "-" + nomefile.Second.ToString() + ".txt";
        //    string filename2 = "summary_" + nomefile.Day.ToString() + "-" + nomefile.Month.ToString() + "-" + nomefile.Year.ToString() + "-" + nomefile.Hour.ToString() + "-" + nomefile.Minute.ToString() + "-" + nomefile.Second.ToString() + ".txt";
        //    string filename3 = "bat_" + nomefile.Day.ToString() + "-" + nomefile.Month.ToString() + "-" + nomefile.Year.ToString() + "-" + nomefile.Hour.ToString() + "-" + nomefile.Minute.ToString() + "-" + nomefile.Second.ToString() + ".txt";
        //    string path1 = foldername + "\\" + filename1;
        //    string path2 = foldername + "\\" + filename2;
        //    string path3 = foldername + "\\" + filename3;

        //    file_raw = (StorageFile)await sampleFolder.TryGetItemAsync(path1);
        //    //StorageFile file_raw = null;

        //    //file_bat = (StorageFile)await moveCareFolder.TryGetItemAsync(path3);

          
        //    if (file_raw == null)
        //    {
        //        // If file doesn't exist, indicate users to use scenario 1
        //        file_raw = await sampleFolder.CreateFileAsync(path1, CreationCollisionOption.ReplaceExisting);
        //    }

        //    //********* Store variables in file 1 *************
        //    var stream = await file_raw.OpenAsync(FileAccessMode.ReadWrite);
           
        //    using (var outputStream = stream.GetOutputStreamAt(0))
        //    {
        //        using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
        //        {
        //            dataWriter.WriteString("timestamp" + "\t" +
        //                "press" + "\t" +
        //                "pressvera" + "\t" +
        //                "secondi" + "\t" +
        //                "millis" + "\t" +
        //                "statogioco" + "\t" +
        //                "statocalibraz" + "\t" +
        //                "statopress" + "\t" +
        //                "statofatica" + "\t" +
        //                "accx" + "\t" +
        //                "accy" + "\t" +
        //                "accz" + "\t" +
        //                "girx" + "\t" +
        //                "giry" + "\t" +
        //                "girz" + "\t" +
        //                "loststemp");

        //            foreach (var x in lista)
        //            {
        //                dataWriter.WriteString(Environment.NewLine + x.timestamp.ToString() + "\t" +
        //                    x.pressione.ToString() + "\t" +
        //                    x.pressionevera.ToString() + "\t" +
        //                    x.secondi.ToString() + "\t" +
        //                    x.millisecondi.ToString() + "\t" +
        //                    x.statogioco.ToString() + "\t" +
        //                    x.statodicalibrazione.ToString() + "\t" +
        //                    x.statogiocopresa.ToString() + "\t" +
        //                    x.statogiocofatica.ToString() + "\t" +
        //                    x.accx.ToString() + "\t" +
        //                    x.accy.ToString() + "\t" +
        //                    x.accz.ToString() + "\t" +
        //                    x.girx.ToString() + "\t" +
        //                    x.timestamp.ToString() + "\t" +
        //                    x.giry.ToString() + "\t" +
        //                    x.girz.ToString() + "\t" +
        //                    x.statotimestamp.ToString());
        //            }
        //            await dataWriter.StoreAsync();
        //            await outputStream.FlushAsync();
        //        }
        //    }
        //    //********* Store variables in file 2 *************
        //    // First, read summary file
        //    // Second store the info

        //    //file_summary = (StorageFile)await moveCareFolder.TryGetItemAsync(path2);
        //    //List<string> testo = new List<string>();

        //    //using (var inputStream = await file_summary.OpenReadAsync())
        //    //using (var classicStream = inputStream.AsStreamForRead())
        //    //using (var streamReader = new StreamReader(classicStream))
        //    //{
        //    //    while (streamReader.Peek() >= 0)
        //    //    {
        //    //        testo.Add(streamReader.ReadLine());
        //    //    }
        //    //}
        //    //if (testo.Count > 0)
        //    //{
        //    //    pointver2.puntiprecedenti = Int32.Parse(testo.Last());
        //    //    Debug.WriteLine(pointver2.puntiprecedenti.ToString());
        //    //}
        //    //var stream2 = await file_summary.OpenAsync(FileAccessMode.ReadWrite);
        //    //using (var outputStream2 = stream2.GetOutputStreamAt(0))
        //    //{
        //    //    using (var dataWriter2 = new Windows.Storage.Streams.DataWriter(outputStream2))
        //    //    {
        //    //        dataWriter2.WriteString(nomefile.Day.ToString() + "/" + nomefile.Month.ToString() + "/" + nomefile.Year.ToString() + " " + nomefile.Hour.ToString() + ":" + nomefile.Minute.ToString() + ":" + nomefile.Second.ToString() +
        //    //            Environment.NewLine + "percentile1" +
        //    //            Environment.NewLine + pointver2.percentile1.ToString() +
        //    //            Environment.NewLine + "stamplost" +
        //    //            Environment.NewLine + failstamplife3 +
        //    //            Environment.NewLine + "percentile2" +
        //    //            Environment.NewLine + pointver2.percentile2.ToString() +
        //    //            Environment.NewLine + "stamplost" +
        //    //            Environment.NewLine + failstamplife2 +
        //    //            Environment.NewLine + "percentile3" +
        //    //            Environment.NewLine + pointver2.percentile3.ToString() +
        //    //            Environment.NewLine + "stamplost" +
        //    //            Environment.NewLine + failstamplife1 +
        //    //            Environment.NewLine + "inizio-endurance" +
        //    //            Environment.NewLine + tempoiniziale.Second.ToString() + ":" + tempoiniziale.Millisecond.ToString() +
        //    //            Environment.NewLine + "stamplost" +
        //    //            Environment.NewLine + failstamplife0 +
        //    //            Environment.NewLine + "tempo-endurance" +
        //    //            Environment.NewLine + tempotrascorso.ToString() +
        //    //            Environment.NewLine + "percentileMax" +
        //    //            Environment.NewLine + pointver2.maxofpercentile.ToString() +
        //    //            Environment.NewLine + "point" +
        //    //            Environment.NewLine + pointver2.points.ToString() +
        //    //            Environment.NewLine );

        //    //        await dataWriter2.StoreAsync();
        //    //        await outputStream2.FlushAsync();
        //    //    }
        //    //}

        //    ////********* Store variables in file 3 *************


        //    stream.Dispose();
        //    //stream2.Dispose();


        //    //await Task.Run(() =>
        //    //{
        //    //    Debug.Write(System.IO.Directory.GetCurrentDirectory().ToString());


        //    //    //var path1 = System.IO.Directory.GetCurrentDirectory()  + @"/acquisizioni";
        //    //    //var path1 = storageFolder.Path;
        //    //   // Debug.WriteLine(path1.ToString());

        //    //    nomefile = DateTime.Now;
        //    //    var path2 = foldername + "\\" + nomefile.Day.ToString() + "-" + nomefile.Month.ToString() + "-" + nomefile.Year.ToString() + "-" + nomefile.Hour.ToString() + "-" + nomefile.Minute.ToString() + "-" + nomefile.Second.ToString() + ".txt";
        //    //    var path3 = foldername + "\\" + "summary_" + nomefile.Day.ToString() + "-" + nomefile.Month.ToString() + "-" + nomefile.Year.ToString() + "-" + nomefile.Hour.ToString() + "-" + nomefile.Minute.ToString() + "-" + nomefile.Second.ToString() + ".txt";
        //    //    var path4 = foldername + "\\" + "bat_" + nomefile.Day.ToString() + "-" + nomefile.Month.ToString() + "-" + nomefile.Year.ToString() + "-" + nomefile.Hour.ToString() + "-" + nomefile.Minute.ToString() + "-" + nomefile.Second.ToString() + ".txt";


        //    //    FileStream sb = new FileStream(path2, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        //    //    StreamWriter sw = new StreamWriter(sb);
        //    //    FileStream sb2 = new FileStream(path3, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        //    //    StreamWriter sw2 = new StreamWriter(sb2);
        //    //    FileStream sb3 = new FileStream(path4, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
        //    //    StreamWriter sw3 = new StreamWriter(sb3);



        //    //    //FILE
        //    //    //   await Task.Run(() =>
        //    //    //  {//or await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,() => { 
        //    //    sw.WriteLine("timestamp" + "\t" +
        //    //        "press" + "\t" +
        //    //        "pressvera" + "\t" + //da eliminare
        //    //        "secondi" + "\t" +
        //    //        "millis" + "\t" +
        //    //        "statogioco" + "\t" +
        //    //        "statocalibraz" + "\t" +
        //    //        "statopress" + "\t" +
        //    //        "statofatica" + "\t" +
        //    //      "accx" + "\t" +
        //    //      "accy" + "\t" +
        //    //      "accz" + "\t" +
        //    //      "girx" + "\t" +
        //    //      "giry" + "\t" +
        //    //      "girz" + "\t" +
        //    //      "loststemp"
        //    //        );
        //    //    foreach (var x in lista)
        //    //        sw.WriteLine(
        //    //                x.timestamp.ToString() + "\t" +
        //    //                x.pressione.ToString() + "\t" +
        //    //                x.pressionevera.ToString() + "\t" + //da eliminare
        //    //                x.secondi.ToString() + "\t" +
        //    //                x.millisecondi.ToString() + "\t" +
        //    //                x.statogioco.ToString() + "\t" +
        //    //                x.statodicalibrazione.ToString() + "\t" +
        //    //                x.statogiocopresa.ToString() + "\t" +
        //    //                x.statogiocofatica.ToString() + "\t" +
        //    //                x.accx.ToString() + "\t" +
        //    //                x.accy.ToString() + "\t" +
        //    //                x.accz.ToString() + "\t" +
        //    //                x.girx.ToString() + "\t" +
        //    //                x.giry.ToString() + "\t" +
        //    //                x.girz.ToString() + "\t" +
        //    //                x.statotimestamp
        //    //                );
        //    //    // });
        //    //    StreamReader sr2 = new StreamReader(sb2);

        //    //    string line;
        //    //    List<string> testo = new List<string>();
        //    //    while ((line = sr2.ReadLine()) != null)
        //    //    {
        //    //        testo.Add(line);
        //    //    }
        //    //    if (testo.Count > 0)
        //    //    {
        //    //        pointver2.puntiprecedenti = Int32.Parse(testo.Last());
        //    //        Debug.WriteLine(pointver2.puntiprecedenti.ToString());
        //    //    }
        //    //    sw.WriteLine("data"); sw2.WriteLine(nomefile.Day.ToString() + "/" + nomefile.Month.ToString() + "/" + nomefile.Year.ToString() + " " + nomefile.Hour.ToString() + ":" + nomefile.Minute.ToString() + ":" + nomefile.Second.ToString());
        //    //    sw2.WriteLine("percentile1"); sw2.WriteLine(pointver2.percentile1.ToString());
        //    //    sw2.WriteLine("stamplost"); sw2.WriteLine(failstamplife3);
        //    //    sw2.WriteLine("percentile2"); sw2.WriteLine(pointver2.percentile2.ToString());
        //    //    sw2.WriteLine("stamplost"); sw2.WriteLine(failstamplife2);
        //    //    sw2.WriteLine("percentile3"); sw2.WriteLine(pointver2.percentile3.ToString());
        //    //    sw2.WriteLine("stamplost"); sw2.WriteLine(failstamplife1);
        //    //    sw2.WriteLine("inizio-endurance"); sw2.WriteLine(tempoiniziale.Second.ToString() + ":" + tempoiniziale.Millisecond.ToString());
        //    //    sw2.WriteLine("stamplost"); sw2.WriteLine(failstamplife0);
        //    //    sw2.WriteLine("tempo-endurance"); sw2.WriteLine(tempotrascorso.ToString());
        //    //    sw2.WriteLine("percentileMax"); sw2.WriteLine(pointver2.maxofpercentile.ToString());
        //    //    sw2.WriteLine("point"); sw2.WriteLine(pointver2.points.ToString());

        //    //    sw.Dispose();
        //    //    sw2.Dispose();
        //    //    sb.Dispose();
        //    //    sb2.Dispose();
        //    //    sr2.Dispose();


        //    //});


        //}

        //async void logWrite()
        //{

        //}
        //async void createLogFile()
        //{
        //    StorageFolder moveCareFolder = await KnownFolders.GetFolderForUserAsync(null /* current user */, KnownFolderId.DocumentsLibrary);
        //    sampleFolder = (StorageFolder)await moveCareFolder.TryGetItemAsync(foldername);

        //    var path = moveCareFolder.Path;

        //    if (sampleFolder == null)
        //    {
        //        //If the folder don't exist, create it 
        //        //Debug.Write("Folder don't exist");
        //        StorageFolder created = await moveCareFolder.CreateFolderAsync(foldername, CreationCollisionOption.FailIfExists);
        //        sampleFolder = (StorageFolder)await moveCareFolder.TryGetItemAsync(foldername);
        //    }
        //}

        async void ValidateFolder()
        {
            StorageFolder moveCareFolder = await KnownFolders.GetFolderForUserAsync(null /* current user */, KnownFolderId.DocumentsLibrary); 
            sampleFolder = (StorageFolder)await moveCareFolder.TryGetItemAsync(foldername);
            
            var path = moveCareFolder.Path;

            if (sampleFolder == null)
            {
                //If the folder don't exist, create it 
                //Debug.Write("Folder don't exist");
                StorageFolder created = await moveCareFolder.CreateFolderAsync(foldername, CreationCollisionOption.FailIfExists);
                sampleFolder = (StorageFolder)await moveCareFolder.TryGetItemAsync(foldername);
            }
        }

        class Observer
        {

            public async void HandleEvent(object sender, EventArgs args)
            {

                               
                var nomefile = DateTime.Now;
                string filename1 = nomefile.Day.ToString() + "-" + nomefile.Month.ToString() + "-" + nomefile.Year.ToString() + "-" + nomefile.Hour.ToString() + "-" + nomefile.Minute.ToString() + "-" + nomefile.Second.ToString() + ".txt";
                string filename2 = "summary_" + nomefile.Day.ToString() + "-" + nomefile.Month.ToString() + "-" + nomefile.Year.ToString() + "-" + nomefile.Hour.ToString() + "-" + nomefile.Minute.ToString() + "-" + nomefile.Second.ToString() + ".txt";
                string filename3 = "bat_" + nomefile.Day.ToString() + "-" + nomefile.Month.ToString() + "-" + nomefile.Year.ToString() + "-" + nomefile.Hour.ToString() + "-" + nomefile.Minute.ToString() + "-" + nomefile.Second.ToString() + ".txt";

                string path1 = filename1;
                string path2 = filename2;
                string path3 = filename3;

                file_raw = (StorageFile)await sampleFolder.TryGetItemAsync(path1);

                if (file_raw == null)
                {
                    // If file doesn't exist, create it
                    file_raw = await sampleFolder.CreateFileAsync(path1, CreationCollisionOption.ReplaceExisting);
                }

                //********* save variables in file 1 *************
                var stream = await file_raw.OpenAsync(FileAccessMode.ReadWrite);

                using (var outputStream = stream.GetOutputStreamAt(0))
                {
                    using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                    {
                        dataWriter.WriteString("timestamp" + "\t" +
                            "press" + "\t" +
                            "pressvera" + "\t" +
                            "secondi" + "\t" +
                            "millis" + "\t" +
                            "statogioco" + "\t" +
                            "statocalibraz" + "\t" +
                            "statopress" + "\t" +
                            "statofatica" + "\t" +
                            "accx" + "\t" +
                            "accy" + "\t" +
                            "accz" + "\t" +
                            "girx" + "\t" +
                            "giry" + "\t" +
                            "girz" + "\t" +
                            "loststemp");

                        foreach (var x in lista)
                        {
                            dataWriter.WriteString(Environment.NewLine + x.timestamp.ToString() + "\t" +
                                x.pressione.ToString() + "\t" +
                                x.pressionevera.ToString() + "\t" +
                                x.secondi.ToString() + "\t" +
                                x.millisecondi.ToString() + "\t" +
                                x.statogioco.ToString() + "\t" +
                                x.statodicalibrazione.ToString() + "\t" +
                                x.statogiocopresa.ToString() + "\t" +
                                x.statogiocofatica.ToString() + "\t" +
                                x.accx.ToString() + "\t" +
                                x.accy.ToString() + "\t" +
                                x.accz.ToString() + "\t" +
                                x.girx.ToString() + "\t" +
                                x.giry.ToString() + "\t" +
                                x.girz.ToString() + "\t" +
                                x.statotimestamp.ToString());
                        }
                        await dataWriter.StoreAsync();
                        await outputStream.FlushAsync();
                    }
                }
                stream.Dispose();

                //*********save variables in file 2 *************

               file_summary = (StorageFile)await sampleFolder.TryGetItemAsync(path2);
                List<string> testo = new List<string>();

                if (file_summary == null)
                {
                    // If file doesn't exist, create it
                    file_summary = await sampleFolder.CreateFileAsync(path2, CreationCollisionOption.ReplaceExisting);
                }

                using (var inputStream = await file_summary.OpenReadAsync())
                using (var classicStream = inputStream.AsStreamForRead())
                using (var streamReader = new StreamReader(classicStream))
                {
                    while (streamReader.Peek() >= 0)
                    {
                        testo.Add(streamReader.ReadLine());
                    }
                }
                if (testo.Count > 0)
                {
                    pointver2.puntiprecedenti = Int32.Parse(testo.Last());
                    Debug.WriteLine(pointver2.puntiprecedenti.ToString());
                }
                var stream2 = await file_summary.OpenAsync(FileAccessMode.ReadWrite);
                using (var outputStream2 = stream2.GetOutputStreamAt(0))
                {
                    using (var dataWriter2 = new Windows.Storage.Streams.DataWriter(outputStream2))
                    {
                        dataWriter2.WriteString(nomefile.Day.ToString() + "/" + nomefile.Month.ToString() + "/" + nomefile.Year.ToString() + " " + nomefile.Hour.ToString() + ":" + nomefile.Minute.ToString() + ":" + nomefile.Second.ToString() +
                            Environment.NewLine + "percentile1" +
                            Environment.NewLine + pointver2.percentile1.ToString() +
                            Environment.NewLine + "stamplost" +
                            Environment.NewLine + failstamplife3 +
                            Environment.NewLine + "percentile2" +
                            Environment.NewLine + pointver2.percentile2.ToString() +
                            Environment.NewLine + "stamplost" +
                            Environment.NewLine + failstamplife2 +
                            Environment.NewLine + "percentile3" +
                            Environment.NewLine + pointver2.percentile3.ToString() +
                            Environment.NewLine + "stamplost" +
                            Environment.NewLine + failstamplife1 +
                            Environment.NewLine + "inizio-endurance" +
                            Environment.NewLine + tempoiniziale.Second.ToString() + ":" + tempoiniziale.Millisecond.ToString() +
                            Environment.NewLine + "stamplost" +
                            Environment.NewLine + failstamplife0 +
                            Environment.NewLine + "tempo-endurance" +
                            Environment.NewLine + tempotrascorso.ToString() +
                            Environment.NewLine + "percentileMax" +
                            Environment.NewLine + pointver2.maxofpercentile.ToString() +
                            Environment.NewLine + "point" +
                            Environment.NewLine + pointver2.points.ToString() +
                            Environment.NewLine + "Bluetooth Address" +
                            Environment.NewLine + BTMacAddress +
                            Environment.NewLine);

                        await dataWriter2.StoreAsync();
                        await outputStream2.FlushAsync();
                    }
                }
                stream2.Dispose();

                //*********save variables in file 3 *************

                file_bat = (StorageFile)await sampleFolder.TryGetItemAsync(path3);
                
                if (file_bat == null)
                {
                    // If file doesn't exist, create it
                    file_bat = await sampleFolder.CreateFileAsync(path3, CreationCollisionOption.ReplaceExisting);
                }
                var stream3 = await file_bat.OpenAsync(FileAccessMode.ReadWrite);

                using (var outputStream = stream3.GetOutputStreamAt(0))
                {
                    using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                    {
                        dataWriter.WriteString("timestamp" + "\t" +
                            "SOC" + "\t" +
                            "Volt" + "\t" +
                            "Amps" + "\t" +
                            "Status" + "\t" +
                            "Seconds" + "\t" +
                            "Milliseconds");

                        foreach (var bat in lista2)
                        {
                            dataWriter.WriteString(Environment.NewLine + bat.timestampBat.ToString() + "\t" +
                                bat.soc.ToString() + "\t" +
                                bat.volt.ToString() + "\t" +
                                bat.amps.ToString() + "\t" +
                                bat.stat.ToString() + "\t" +
                                bat.secondi.ToString() + "\t" +
                                bat.millisecondi.ToString());
                        }
                        await dataWriter.StoreAsync();
                        await outputStream.FlushAsync();
                    }
                }
                stream3.Dispose();
                
                SaveLogFile(false);
                //Save the previous value
                WritePreviousValues();

                //Save max value - the first time.
                if (pointver2.valore_massimo_flag == true)
                {
                    WriteMassimo();
                }
                
            }
        }

        class Observable
        {
            public event EventHandler SomethingHappened;

            public void DoSomething()
            {
                EventHandler handler = SomethingHappened;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        /*@
         * 
         * 
         * 
         */
        static public async void SaveLogFile(bool evento)
        {
            var toDay = DateTime.Now;
            string filename = "exg.log";
            string path = filename;
            ulong lastPosition = 0;
            Windows.Storage.Streams.IRandomAccessStream stream;

            file_raw = (StorageFile)await sampleFolder.TryGetItemAsync(path);

            if (file_raw == null)
            {
                // If file doesn't exist, create it
                file_raw = await sampleFolder.CreateFileAsync(path, CreationCollisionOption.ReplaceExisting);
                stream = await file_raw.OpenAsync(FileAccessMode.ReadWrite);
            }
            else
            {
                //Encuentro la ultima posicion
                stream = await file_raw.OpenAsync(FileAccessMode.ReadWrite);
                ulong size = stream.Size;//stream.GetOutputStreamAt(lastPosition)
                using (var inputStream = stream.GetInputStreamAt(0))
                {
                    using (var dataReader = new Windows.Storage.Streams.DataReader(inputStream))
                    {
                        uint numBytesLoaded = await dataReader.LoadAsync((uint)size);
                        string text = dataReader.ReadString(numBytesLoaded);
                    }
                }
                lastPosition = size;
            }
            if (evento)
            {
                if (!pointver2.batFlag)
                {
                    using (var outputStream = stream.GetOutputStreamAt(lastPosition))
                    {
                        using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                        {
                            dataWriter.WriteString(
                                toDay.Day.ToString() + "/" +
                                toDay.Month.ToString() + "/" +
                                toDay.Year.ToString() + "-" +
                                toDay.Hour.ToString() + ":" +
                                toDay.Minute.ToString() + ":" +
                                toDay.Second.ToString() + "\t - " +
                                "Error: Ball out of charge" +
                                Environment.NewLine);
                            await dataWriter.StoreAsync();
                            await outputStream.FlushAsync();
                        }
                    }
                    stream.Dispose();
                }
                else
                {
                    if (changInstr.nuevoFlag && !nuevoFlag2)
                    {
                        nuevoFlag2 = true;
                        using (var outputStream = stream.GetOutputStreamAt(lastPosition))//stream.GetOutputStreamAt(0))
                        {
                            using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                            {
                                dataWriter.WriteString(
                                    toDay.Day.ToString() + "/" +
                                    toDay.Month.ToString() + "/" +
                                    toDay.Year.ToString() + "-" +
                                    toDay.Hour.ToString() + ":" +
                                    toDay.Minute.ToString() + ":" +
                                    toDay.Second.ToString() + "\t - " +
                                    "Error: Timeout" +
                                    Environment.NewLine);
                                await dataWriter.StoreAsync();
                                await outputStream.FlushAsync();
                            }
                        }
                        stream.Dispose();
                    }
                    if (changInstr.timeoutflag)
                    {
                        using (var outputStream = stream.GetOutputStreamAt(lastPosition))//stream.GetOutputStreamAt(0))
                        {
                            using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                            {
                                dataWriter.WriteString(
                                    toDay.Day.ToString() + "/" +
                                    toDay.Month.ToString() + "/" +
                                    toDay.Year.ToString() + "-" +
                                    toDay.Hour.ToString() + ":" +
                                    toDay.Minute.ToString() + ":" +
                                    toDay.Second.ToString() + "\t - " +
                                    "Error: Ball not found" +
                                    Environment.NewLine);
                                await dataWriter.StoreAsync();
                                await outputStream.FlushAsync();
                            }
                        }
                        stream.Dispose();
                    }

                }
            }
            else
            {
                using (var outputStream = stream.GetOutputStreamAt(lastPosition))
                {
                    using (var dataWriter = new Windows.Storage.Streams.DataWriter(outputStream))
                    {
                        dataWriter.WriteString(
                            openTime.Day.ToString() + "/" +
                            openTime.Month.ToString() + "/" +
                            openTime.Year.ToString() + "-" +
                            openTime.Hour.ToString() + ":" +
                            openTime.Minute.ToString() + ":" +
                            openTime.Second.ToString() + "\t - " +
                            "OK: Open game" +
                            Environment.NewLine +
                            toDay.Day.ToString() + "/" +
                            toDay.Month.ToString() + "/" +
                            toDay.Year.ToString() + "-" +
                            toDay.Hour.ToString() + ":" +
                            toDay.Minute.ToString() + ":" +
                            toDay.Second.ToString() + "\t - " +
                            "OK: Close game" +
                            Environment.NewLine);
                        await dataWriter.StoreAsync();
                        await outputStream.FlushAsync();
                    }
                }
                stream.Dispose();
            }
        }


        DispatcherTimer dispatcherTimer;
        DateTimeOffset startTime;
        DateTimeOffset lastTime;
        DateTimeOffset stopTime;
        int timesTicked = 1;
        int timesToTick = 10;

        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            //IsEnabled defaults to false

            startTime = DateTimeOffset.Now;
            lastTime = startTime;

            dispatcherTimer.Start();


        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            DateTimeOffset time = DateTimeOffset.Now;
            TimeSpan span = time - lastTime;
            lastTime = time;
            //Time since last tick should be very very close to Interval

            timesTicked++;
            Debug.WriteLine(timesTicked.ToString() + "\n");
            if (pointver2.abilitaprimacalibraz)
            {
                if (pippo6 == false)
                {

                    if (deviceWatcher == null)
                    {
                        StartBleDeviceWatcher();
                    }
                    else
                    {
                        StopBleDeviceWatcher();
                    }
                    pippo6 = true;
                }

            }
            //SOC validation
            if (socFlag ==true && soc <= SOC_level)
            {
                //Se carga mensaje inicial de que la bateria es baja y que se debe dejar cargando.
                pointver2.batFlag = false;
                pointver2.devicenotfound = true;
                pointver2.life = -1;
               
                SaveLogFile(true);
                StopBleDeviceWatcher();
                StopBleDeviceWatcher();
            }

            if (timesTicked == pointver2.secondidiricerca && pointver2.abilitadopocalibraz == false)
            {
                pointver2.devicenotfound = true;               
                StopBleDeviceWatcher();
                StopBleDeviceWatcher();

            }
            if (timesTicked > pointver2.secondidiricerca + 2 && changInstr.timeoutflag == false) //Why we are using "+ 2" seconds?
            {
                StopBleDeviceWatcher();
                changInstr.timeoutflag = true;
                SaveLogFile(true);
                
                

                //stopTime = time;
                //Debug.WriteLine("Calling dispatcherTimer.Stop()\n");
                //dispatcherTimer.Stop();
                ////IsEnabled should now be false after calling stop
                //Debug.WriteLine("dispatcherTimer.IsEnabled = " + dispatcherTimer.IsEnabled + "\n");
                //span = stopTime - startTime;
                //Debug.WriteLine("Total Time Start-Stop: " + span.ToString() + "\n");
            }
            if (timesTicked >= generalTimeOut)
            {
                StopBleDeviceWatcher();
                changInstr.timeoutflag = false;
                pointver2.devicenotfound = false;
                changInstr.nuevoFlag = true;
                SaveLogFile(true);
            }
        }



        #endregion

    }




}