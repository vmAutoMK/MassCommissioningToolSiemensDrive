using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Siemens.Engineering.Download;
using Siemens.Engineering.Download.Configurations;
using Siemens.Engineering.Connection;
using Siemens.Engineering.Online;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.Library.MasterCopies;
using Siemens.Engineering.MC.Drives;
using Siemens.Engineering.MC.Drives.Enums;
using OpennessClass;
using System.Reflection;
using Microsoft.Win32;
using System.IO;


namespace CopyDrive
{
    public partial class Form1 : Form
    {

        //Variables Openness
        Openness myOpenness = new Openness();

        //Variables Backgroundworker
        BackgroundWorker backgroundWorker = new BackgroundWorker();



        //Variables UI
        List<string> lsDevicesSinamicsG = new List<string>();
        List<string> lsPcInterfaces = new List<string>();
        bool AllSelected = true;



        //Variables used for copying the drives
        Device masterDrive;
        Device slaveDevice;
        List<DeviceSinamicsG> listDevicesToCopy = new List<DeviceSinamicsG>();
        MasterCopy mastercopy;

        //Variables for downloading
        List<DeviceExtended> listDevicesToDownload = new List<DeviceExtended>();
        int currentParameter;
        private DeviceExtended device;


        #region general functions
        public Form1()
        {
            InitializeComponent();
            InitializeAssembly();
            InitializeBackgroundWorker();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //Initialise  Assembly Resolver
        private void InitializeAssembly()
        {
            AppDomain CurrentDomain = AppDomain.CurrentDomain;
            CurrentDomain.AssemblyResolve += new ResolveEventHandler(SiemensEngineeringResolver);
        }

        // Find the Siemens.Engineering.dll
        private static Assembly SiemensEngineeringResolver(object sender, ResolveEventArgs args)
        {
            int index = args.Name.IndexOf(',');
            if (index == -1)
            {
                return null;
            }
            string name = args.Name.Substring(0, index);

            RegistryKey filePathReg = Registry.LocalMachine.OpenSubKey(
                "SOFTWARE\\Siemens\\Automation\\Openness\\16.0\\PublicAPI\\16.0.0.0");

            if (filePathReg == null)
                return null;

            object oRegKeyValue = filePathReg.GetValue(name);
            if (oRegKeyValue != null)
            {
                string filePath = oRegKeyValue.ToString();

                string path = filePath;
                string fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    return Assembly.LoadFrom(fullPath);
                }
            }

            return null;
        }

        //Initialise backgroundWorker
        private void InitializeBackgroundWorker()
        {
            backgroundWorker.DoWork += new DoWorkEventHandler(backgroundWorker_DoWork);
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(backgroundWorker_RunWorkerCompleted);
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.WorkerReportsProgress = true;
        }

        //Connect to open TIA Project.
        private void btnConnect_Click(object sender, EventArgs e)
        {
            StatusMessage("Connecting to TIA project ... ");

            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync(new List<Object> { BackgroundOperation.ConnectProject });
            }
        }

        //Open TIA without UI and Connect to Project
        private void btnOpenProjectBckGrnd_Click(object sender, EventArgs e)
        {
            StatusMessage("Opening Tia Project ... ");

            OpenFileDialog fileSearch = new OpenFileDialog();
            fileSearch.Filter = "*.ap16|*.ap16";
            fileSearch.RestoreDirectory = true;
            fileSearch.ShowDialog();

            string projectPath = fileSearch.FileName.ToString();

            if (string.IsNullOrEmpty(projectPath) == false)
            {
                if (!backgroundWorker.IsBusy)
                {
                    backgroundWorker.RunWorkerAsync(new List<Object> { BackgroundOperation.OpenProject, projectPath });
                }
            }
            else
            {
                StatusMessage("Connecting to TIA project FAILED.");
            }
        }


        //Connect Project
        private void ConnectProject()
        {
            myOpenness.ConnectToOpenProject();
        }


        //Open Project
        private void OpenProject(string projectPath)
        {
            myOpenness.StartTiaPortal(false);
            myOpenness.OpenProject(projectPath);
        }

        #endregion



        //Select Drive To Copy is selected/changed => Selected drive gray in TreeView
        private void cbDriveToCopy_SelectedIndexChanged(object sender, EventArgs e)
        {
            int myInt = 0;
            foreach (TreeNode node in tvDevices.Nodes)//parent nodes
            {

                foreach (TreeNode childnode in tvDevices.Nodes[myInt].Nodes)
                {
                    if (childnode.Text.Equals(cbDriveToCopy.SelectedItem.ToString()))
                    {
                        childnode.Checked = false;
                        childnode.BackColor = Color.Gray;
                    }
                    else
                    {
                        childnode.BackColor = Color.White;
                    }
                }



                myInt++;
            }
        }





        //Update lists used for UI
        private void UpdateListsForUI()
        {
            myOpenness.ListSinamicsDevices();
            myOpenness.ListPcInterfaces();
            UpdateLsDevicesSinamicsG();
            UpdateLsPcInterfaces();
        }



        //Update list with all sinamics G Devices (by drive NAME)
        private void UpdateLsDevicesSinamicsG()
        {
            lsDevicesSinamicsG.Clear();
            lsDevicesSinamicsG.Add("---");


            foreach (DeviceExtended deviceextended in myOpenness.lsDeviceExtendeds)
            {
                lsDevicesSinamicsG.Add(deviceextended.driveName);
            }


        }

        //Update list with all Interfaces (by name)
        private void UpdateLsPcInterfaces()
        {
            lsPcInterfaces.Clear();

            foreach (ConfigurationPcInterface pcInterface in myOpenness.lsPcInterfaces)
            {
                lsPcInterfaces.Add(pcInterface.Name);
            }
        }


        //Update UI elements
        private void UpdateUI()
        {

            cbDriveToCopy.DataSource = lsDevicesSinamicsG;
            cbPcInterface.DataSource = lsPcInterfaces;

            tvDevices.Nodes.Clear();


            int groupcounter = 0;
            foreach (deviceUsergroupExtended usergroupextended in myOpenness.lsUserGroupsExtended)
            {

                //devices not in specific group
                if (usergroupextended.deviceusergroup == null)
                {
                    tvDevices.Nodes.Add("UNGROUPED DEVICES");

                    foreach (Device device in usergroupextended.lsUsergroupDevices)
                    {
                        tvDevices.Nodes[groupcounter].Nodes.Add(device.DeviceItems[1].Name);
                    }
                    groupcounter++;

                }
                else if (usergroupextended.lsUsergroupDevices.Count > 0)
                {
                    //parent node from foldernames (with general)

                    tvDevices.Nodes.Add(usergroupextended.deviceusergroup.Name);

                    foreach (Device device in usergroupextended.lsUsergroupDevices)
                    {
                        //child node add all G devices to specific parent
                        tvDevices.Nodes[groupcounter].Nodes.Add(device.DeviceItems[1].Name);

                    }
                    groupcounter++;

                }
            }


            
            SelectAllTree();

            StatusMessage("Getting Sinamics G devices Done.");
        }








        //Select all devices in the tree
        // TODO: for child nodes // when select_deselect more often with different master will go wrong
        private void SelectAllTree()
        {
            foreach (TreeNode parentNode in tvDevices.Nodes)
            {

                foreach (TreeNode childNode in parentNode.Nodes)
                {
                    if (childNode.Text.Equals(cbDriveToCopy.SelectedItem.ToString()))
                    {
                        childNode.Checked = false;
                        childNode.BackColor = Color.Gray;
                        childNode.ForeColor = Color.LightGray;
                        parentNode.Checked = false;

                    }
                    else
                    {
                        childNode.Checked = true;
                        childNode.BackColor = Color.White;
                        parentNode.Checked = true;
                    }
                }
            }
        }

        //Start copying drives
        // put master drive in mastercopy
        //fill listDevicesToCopy (=target devices)
        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (cbDriveToCopy.SelectedItem.ToString().Equals("---") || cbDriveToCopy.SelectedItem.ToString().Equals(""))
            {
                //No Device is selected to copy
                StatusMessage("No Drive To Copy Selected.");

            }
            else
            {
                progressBar.Value = 5;

                btnCopy.Enabled = false;
                btnDownload.Enabled = false;
                btnCancel.Enabled = true;

                //Get drive to copy and selected drives in tree
                listDevicesToCopy.Clear();

                // search masterdrive
                //fill listDevicesToCopy (=target devices)
                foreach (DeviceExtended deviceextended in myOpenness.lsDeviceExtendeds)//collection of all devices (and their usergroup as extra)
                {


                    if (deviceextended.driveName.Equals(cbDriveToCopy.SelectedItem.ToString()))
                    {
                        //check all Gdevices if name equals 'drive to copy' => masterDrive
                        masterDrive = deviceextended.device;
                        continue;
                    }
                    else
                    {
                        //every drive that is selected and is not master drive => listDevicesToCopy (target devices)
                        //int nodeCounter = 0;
                        foreach (TreeNode parentNode in tvDevices.Nodes)//parent node
                        {
                            foreach (TreeNode childNode in parentNode.Nodes)//child node
                            {
                                if (deviceextended.driveName.Equals(childNode.Text) && childNode.Checked)
                                {
                                    listDevicesToCopy.Add(new DeviceSinamicsG(deviceextended));
                                    continue;
                                }
                            }
                           // nodeCounter++;
                        }
                    }

                }

                //Start copying devices
                mastercopy = myOpenness.DeviceToLibrary(masterDrive);

                StatusMessage("Start copy devices");

                if (!backgroundWorker.IsBusy)
                {
                    backgroundWorker.RunWorkerAsync(new List<Object> { BackgroundOperation.CopyDevices, });
                }

            }

        }

        //Copy Devices from lib to project
        private void CopyDevice(BackgroundWorker worker, DoWorkEventArgs e)
        {
            DeviceSinamicsG sinamicsG;

            for (int i = 0; i < listDevicesToCopy.Count; i++)
            {
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    sinamicsG = listDevicesToCopy[i];
                    sinamicsG.updateNetworkPorts();


                    slaveDevice = myOpenness.AddDeviceFromLibrary(mastercopy, sinamicsG.deviceUserGroup);
                    myOpenness.DeleteDevice(sinamicsG.Name, sinamicsG.deviceUserGroup); //TODO not correct, doesn't check usergroup -> to check


                    //TODO hieronder loopt het vast
                    SetDeviceName(slaveDevice, sinamicsG.Name);
                    SetNetwork(slaveDevice, sinamicsG.subnet, sinamicsG.ioSystem, sinamicsG.ipAddress, sinamicsG.pnDeviceNameAuto, sinamicsG.pnDeviceName, sinamicsG.PnDeviceNumber);
                    SetSafetyAddress(slaveDevice, sinamicsG.safetyAddress);
                    SetSafetyHWAddress(slaveDevice, sinamicsG.safetHwAddress_in, sinamicsG.safetHwAddress_out);
                    SetHWAddress(slaveDevice, sinamicsG.hwAddress_in, sinamicsG.hwAddress_out);
                    SetHWAddressSupp(slaveDevice, sinamicsG.hwAddress_supp_in, sinamicsG.hwAddress_supp_out);
                    SetHWAddressAdd(slaveDevice, sinamicsG.hwAddress_add_in, sinamicsG.hwAddress_add_out);
                    SetTopology(slaveDevice, sinamicsG.networkPorts);

                    int percentComplete = (i + 1) * 100 / listDevicesToCopy.Count;
                    if (i > 100)
                    {
                        i = 100;
                    }
                    worker.ReportProgress(percentComplete, new List<Object> { sinamicsG.Name, "Done" });
                }
            }

            CleanupLibrary(mastercopy);
            myOpenness.ListSinamicsDevices();
        }

        //Set device name and Number
        private void SetDeviceName(Device device, string name)
        {
            device.DeviceItems[1].Name = name;
            device.Name = name;
        }

        private void SetNetwork(Device device, Subnet subnet, IoSystem ioSystem, string ipAddress, string pnDeviceNameAuto, string pnDeviceName, int PnDeviceNumber)
        {
            foreach (DeviceItem deviceItem in device.DeviceItems[1].DeviceItems)
            {
                if (deviceItem.Name.Contains("PROFINET"))
                {
                    IoConnector ioConnector = deviceItem.GetService<NetworkInterface>().IoConnectors[0];
                    Node node = deviceItem.GetService<NetworkInterface>().Nodes[0];

                    node.ConnectToSubnet(subnet);
                    ioConnector.ConnectToIoSystem(ioSystem);
                    node.SetAttribute("Address", ipAddress);
                    ioConnector.SetAttribute("PnDeviceNumber", PnDeviceNumber);

                    if (pnDeviceNameAuto.Equals("False"))
                    {
                        node.SetAttribute("PnDeviceNameAutoGeneration", false);
                        node.SetAttribute("PnDeviceName", pnDeviceName);
                    }
                }
            }
        }

        //Set the safetyAddresses
        private void SetSafetyAddress(Device device, int safetyAddress)
        {
            DriveObject driveObject = device.DeviceItems[1].GetService<DriveObjectContainer>().DriveObjects[0];
            Telegram telegram = driveObject.Telegrams.Find(TelegramType.SafetyTelegram);

            if (telegram != null)
            {
                telegram.SetAttribute("Failsafe_FDestinationAddress", safetyAddress);
            }
        }

        //Set the Safety HW Addresses
        private void SetSafetyHWAddress(Device device, int safetHwAddress_in, int safetHwAddress_out)
        {
            DriveObject driveObject = device.DeviceItems[1].GetService<DriveObjectContainer>().DriveObjects[0];
            Telegram telegram = driveObject.Telegrams.Find(TelegramType.SafetyTelegram);

            if (telegram != null)
            {
                foreach (Address address in telegram.Addresses)
                {
                    if (address.IoType.Equals(AddressIoType.Input))
                    {
                        address.StartAddress = safetHwAddress_in;
                    }
                    if (address.IoType.Equals(AddressIoType.Output))
                    {
                        address.StartAddress = safetHwAddress_out;
                    }
                }
            }
        }

        //Set the HW Addresses
        private void SetHWAddress(Device device, int hwAddress_in, int hwAddress_out)
        {
            DriveObject driveObject = device.DeviceItems[1].GetService<DriveObjectContainer>().DriveObjects[0];
            Telegram telegram = driveObject.Telegrams.Find(TelegramType.MainTelegram);

            if (telegram != null)
            {
                foreach (Address address in telegram.Addresses)
                {
                    if (address.IoType.Equals(AddressIoType.Input))
                    {
                        address.StartAddress = hwAddress_in;
                    }
                    if (address.IoType.Equals(AddressIoType.Output))
                    {
                        address.StartAddress = hwAddress_out;
                    }
                }
            }
        }

        //Set the HW Addresses Supplementary Telegram
        private void SetHWAddressSupp(Device device, int hwAddress_in, int hwAddress_out)
        {
            DriveObject driveObject = device.DeviceItems[1].GetService<DriveObjectContainer>().DriveObjects[0];
            Telegram telegram = driveObject.Telegrams.Find(TelegramType.SupplementaryTelegram);

            if (telegram != null)
            {
                foreach (Address address in telegram.Addresses)
                {
                    if (address.IoType.Equals(AddressIoType.Input))
                    {
                        address.StartAddress = hwAddress_in;
                    }
                    if (address.IoType.Equals(AddressIoType.Output))
                    {
                        address.StartAddress = hwAddress_out;
                    }
                }
            }
        }

        //Set the HW Addresses Additional Telegram
        private void SetHWAddressAdd(Device device, int hwAddress_in, int hwAddress_out)
        {
            DriveObject driveObject = device.DeviceItems[1].GetService<DriveObjectContainer>().DriveObjects[0];
            Telegram telegram = driveObject.Telegrams.Find(TelegramType.AdditionalTelegram);

            if (telegram != null)
            {
                foreach (Address address in telegram.Addresses)
                {
                    if (address.IoType.Equals(AddressIoType.Input))
                    {
                        address.StartAddress = hwAddress_in;
                    }
                    if (address.IoType.Equals(AddressIoType.Output))
                    {
                        address.StartAddress = hwAddress_out;
                    }
                }
            }
        }

        //Set Topology
        private void SetTopology(Device device, List<NetworkPort> networkPorts)
        {
            int i = 0;

            foreach (DeviceItem deviceItem in device.DeviceItems[1].DeviceItems)
            {
                if (deviceItem.Name.Contains("PROFINET"))
                {
                    foreach (DeviceItem port in deviceItem.DeviceItems)
                    {
                        NetworkPort networkPort = port.GetService<NetworkPort>();
                        if (networkPorts[i] != null)
                        {
                            networkPort.ConnectToPort(networkPorts[i]);
                        }

                        i++;
                    }
                }
            }
        }

        //Clean library
        private void CleanupLibrary(MasterCopy mastercopy)
        {
            mastercopy.Delete();
        }

        //Click download button
        private void btnDownload_Click(object sender, EventArgs e)
        {
            btnCopy.Enabled = false;
            btnDownload.Enabled = false;
            btnCancel.Enabled = true;

            progressBar.Value = 5;

            listDevicesToDownload.Clear();

            foreach (DeviceExtended deviceextended in myOpenness.lsDeviceExtendeds)
            {
                foreach (TreeNode parentNode in tvDevices.Nodes)
                {
                    foreach(TreeNode childNode in parentNode.Nodes)
                    {
                        if (deviceextended.driveName.Equals(childNode.Text) && childNode.Checked)
                        {
                            listDevicesToDownload.Add(deviceextended);
                        }
                    }
                 
                }
            }

            StatusMessage("Start Download devices");

            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync(new List<Object> { BackgroundOperation.DownloadDevices, cbPcInterface.SelectedItem.ToString() });
            }
        }

        //Download All Devices
        private void DownloadDevices(BackgroundWorker worker, DoWorkEventArgs e, string selectedPcInterface)
        {
            DeviceExtended deviceextended;
            for (int i = 0; i < listDevicesToDownload.Count; i++)
            {
                int percentComplete;

                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    deviceextended = listDevicesToDownload[i];

                    if (!DownloadToDevice(deviceextended, selectedPcInterface))
                    {
                        percentComplete = (i + 1) * 100 / listDevicesToDownload.Count;
                        if (i > 100)
                        {
                            i = 100;
                        }
                        worker.ReportProgress(percentComplete, new List<Object> { deviceextended.device.DeviceItems[1].Name, "Failed" });
                        continue;
                    }

                    SafetyComm(deviceextended.device);
                    CopyRamToRom(deviceextended.device);

                    percentComplete = (i + 1) * 100 / listDevicesToDownload.Count;
                    if (i > 100)
                    {
                        i = 100;
                    }
                    worker.ReportProgress(percentComplete, new List<Object> { deviceextended.driveName, "Done" });
                }
            }
        }

        //Download To Device
        private bool DownloadToDevice(DeviceExtended deviceextended, string selectedPcInterface)
        {

            DeviceSinamicsG sinamicsG = new DeviceSinamicsG(deviceextended);

            DownloadProvider downloadProvider = deviceextended.device.DeviceItems[1].GetService<DownloadProvider>(); //TODO check
            DownloadConfigurationDelegate pre = PreDownload;
            DownloadConfigurationDelegate post = PostDownload;

            ConnectionConfiguration connConfiguration = downloadProvider.Configuration;
            ConfigurationMode configurationMode = connConfiguration.Modes.Find("PN/IE");
            ConfigurationPcInterface pcInterface = configurationMode.PcInterfaces.Find(selectedPcInterface, 1);
            ConfigurationSubnet subnet = pcInterface.Subnets.Find(sinamicsG.subnet.Name);
            IConfiguration configuration = subnet.Addresses.Find(sinamicsG.ipAddress);

            try
            {
                downloadProvider.Download(configuration, pre, post, DownloadOptions.Software);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        static void PreDownload(DownloadConfiguration configuration)
        {
            StartDriveDownloadCheckConfiguration dcc = configuration as StartDriveDownloadCheckConfiguration;
            if (dcc != null)
            {
                dcc.Checked = true;
            }
        }

        static void PostDownload(DownloadConfiguration configuration)
        {
        }

        //When Safety is activated, do the safety acceptance test
        private void SafetyComm(Device device)
        {
            DriveParameter parameter;
            OnlineDriveObject onlineDriveObject;
            OnlineProvider online;
            string deviceType = device.TypeIdentifier.ToString();

            online = device.DeviceItems[1].GetService<OnlineProvider>();
            online.GoOnline();

            onlineDriveObject = device.DeviceItems[1].GetService<OnlineDriveObjectContainer>().OnlineDriveObjects[0];
            if (onlineDriveObject != null)
            {
                //p9601 != 0 ==> safety activated
                parameter = onlineDriveObject.Parameters.Find("p9601");
                if (!parameter.Value.ToString().Equals("0"))
                {
                    //p10 => 95
                    parameter = onlineDriveObject.Parameters.Find("p10");
                    if (parameter != null)
                    {
                        parameter.Value = 95;
                    }

                    //Set Safety Pwd (p9761)
                    parameter = onlineDriveObject.Parameters.Find("p9761");
                    if (parameter != null)
                    {
                        string value = "0x" + txtSIPwd.Text;
                        parameter.Value = value;
                    }

                    //p9700 => 87
                    currentParameter = 9700;
                    parameter = onlineDriveObject.Parameters.Find("p9700");
                    if (parameter != null)
                    {
                        if (deviceType.Contains("G120"))
                        { parameter.Value = 87; }
                        else if (deviceType.Contains("G110"))
                        { parameter.Value = 208; }
                        else
                        { parameter.Value = 87; }
                    }

                    //p9701 => 172
                    parameter = onlineDriveObject.Parameters.Find("p9701");
                    if (parameter != null)
                    {
                        if (deviceType.Contains("G120"))
                        { parameter.Value = 172; }
                        else if (deviceType.Contains("G110"))
                        { parameter.Value = 220; }
                        else
                        { parameter.Value = 172; }
                    }

                    //p10 => 0
                    parameter = onlineDriveObject.Parameters.Find("p10");
                    if (parameter != null)
                    {
                        parameter.Value = 0;
                    }
                }
            }

            online.GoOffline();
        }

        //Copy Ram to Rom
        private void CopyRamToRom(Device device)
        {
            DriveParameter parameter;
            OnlineDriveObject onlineDriveObject;
            OnlineProvider online;

            online = device.DeviceItems[1].GetService<OnlineProvider>();
            online.GoOnline();

            onlineDriveObject = device.DeviceItems[1].GetService<OnlineDriveObjectContainer>().OnlineDriveObjects[0];
            if (onlineDriveObject != null)
            {

                //p971 => 1
                parameter = onlineDriveObject.Parameters.Find("p971");
                if (parameter != null)
                {
                    parameter.Value = 1;
                }
            }

            online.GoOffline();
        }

        //Update status message
        private void StatusMessage(String MessageText)
        {
            StatusMessages.Text = DateTime.Now.ToString("HH:mm:ss") + ":" + MessageText + " \n" + StatusMessages.Text;
            StatusMessages.Invalidate();
            StatusMessages.Refresh();

            TaskScheduler.FromCurrentSynchronizationContext();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<Object> workerArgs = e.Argument as List<object>;
            BackgroundOperation operation = (BackgroundOperation)workerArgs[0];

            try
            {
                switch (operation)
                {
                    case BackgroundOperation.ConnectProject:
                        ConnectProject();
                        break;

                    case BackgroundOperation.OpenProject:
                        OpenProject((string)workerArgs[1]);
                        break;

                    case BackgroundOperation.UpdateListsForUI:
                        UpdateListsForUI();
                        break;

                    case BackgroundOperation.CopyDevices:
                        CopyDevice(backgroundWorker, e);
                        break;

                    case BackgroundOperation.DownloadDevices:
                        DownloadDevices(backgroundWorker, e, (string)workerArgs[1]);
                        break;
                }

                e.Result = operation;
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("Error when calling method 'set_Value' of type 'Siemens.Engineering.MC.Drives.DriveParameter'") && currentParameter == 9700)
                {
                    //TODO Handling wrong safety Pass
                }
                e.Result = null;
            }

        }


        //BackgroundWorker Completed
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                StatusMessage(e.Error.Message);
            }
            else if (e.Cancelled)
            {
                StatusMessage("Canceled");
                progressBar.Value = 100;
            }
            else if (e.Result == null)
            {
                StatusMessage("Something went wrong");
            }
            else
            {
                BackgroundOperation operation = (BackgroundOperation)e.Result;

                switch (operation)
                {
                    case BackgroundOperation.ConnectProject:
                        if (myOpenness.tiaPortal is null || myOpenness.tiaPortal.Projects.Count <= 0)
                        {
                            StatusMessage("Connecting to TIA project FAILED, no open TIA project found.");
                        }
                        else
                        {
                            StatusMessage("Connected to TIA project " + myOpenness.tiaProject.Name);

                            StatusMessage("Getting Sinamics G devices ...");
                            if (!backgroundWorker.IsBusy)
                            {
                                backgroundWorker.RunWorkerAsync(new List<Object> { BackgroundOperation.UpdateListsForUI });
                            }

                        }
                        break;

                    case BackgroundOperation.OpenProject:
                        if (myOpenness.tiaPortal is null || myOpenness.tiaPortal.Projects.Count <= 0)
                        {
                            StatusMessage("Opening project FAILED");
                        }
                        else
                        {
                            StatusMessage("Connected to TIA project " + myOpenness.tiaProject.Name);

                            StatusMessage("Getting Sinamics G devices ...");
                            if (!backgroundWorker.IsBusy)
                            {
                                backgroundWorker.RunWorkerAsync(new List<Object> { BackgroundOperation.UpdateListsForUI });
                            }
                        }
                        break;

                    case BackgroundOperation.UpdateListsForUI:
                        UpdateUI();
                        btnCopy.Enabled = true;
                        btnDownload.Enabled = true;
                        btnCancel.Enabled = false;
                        break;

                    case BackgroundOperation.CopyDevices:
                        StatusMessage("Copy devices done");
                        btnCopy.Enabled = true;
                        btnDownload.Enabled = true;
                        btnCancel.Enabled = false;
                        break;

                    case BackgroundOperation.DownloadDevices:
                        StatusMessage("Download devices done");
                        btnCopy.Enabled = true;
                        btnDownload.Enabled = true;
                        btnCancel.Enabled = false;
                        break;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnCopy.Enabled = true;
            btnDownload.Enabled = true;
            btnCancel.Enabled = false;
            backgroundWorker.CancelAsync();
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            List<Object> workerArgs = e.UserState as List<object>;

            progressBar.Value = e.ProgressPercentage;
            StatusMessage("Device " + workerArgs[0] + " " + workerArgs[1]);
        }

        //Possible background operations
        public enum BackgroundOperation
        {
            ConnectProject,
            OpenProject,
            UpdateListsForUI,
            CopyDevices,
            DownloadDevices
        }

        private void btnSelectDeselectAll_Click(object sender, EventArgs e)
        {
            if (AllSelected)
            {
                foreach (TreeNode node in tvDevices.Nodes)
                {
                    node.Checked = false;

                    if(node.Nodes.Count >0)
                    {
                        this.CheckAllChildNodes(node, false);
                    }
                }

                AllSelected = false;
            }
            else
            {
                foreach (TreeNode node in tvDevices.Nodes)
                {
                    node.Checked = true;

                    if (node.Nodes.Count > 0)
                    {
                        this.CheckAllChildNodes(node, true);
                    }
                }


                AllSelected = true;
            }


        }

        private void node_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // The code only executes if the user caused the checked state to change.
            if (e.Action != TreeViewAction.Unknown)
            {
                if (e.Node.Nodes.Count > 0)
                {
                    /* Calls the CheckAllChildNodes method, passing in the current 
                    Checked value of the TreeNode whose checked state changed. */
                    this.CheckAllChildNodes(e.Node, e.Node.Checked);
                }
            }
        }


        private void CheckAllChildNodes(TreeNode treeNode, bool nodeChecked)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    this.CheckAllChildNodes(node, nodeChecked);
                }
            }
        }

        private void tsmCollapsAll_Click(object sender, EventArgs e)
        {
            tvDevices.CollapseAll();
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tvDevices.ExpandAll();
        }
    }
}
