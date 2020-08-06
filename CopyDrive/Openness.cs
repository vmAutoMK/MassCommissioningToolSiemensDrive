using System;
using System.Collections.Generic;
using System.IO;
using Siemens.Engineering;
using Siemens.Engineering.Connection;
using Siemens.Engineering.Download;
using Siemens.Engineering.HW;
using Siemens.Engineering.Library.MasterCopies;

namespace OpennessClass
{
    class Openness
    {
        #region variables
        public TiaPortal tiaPortal;
        public Project tiaProject;
        
        /// <summary>
        /// contains all sinamic G devices and the group they are in (group can be null)
        /// </summary>
        public List<DeviceExtended> lsDeviceExtendeds = new List<DeviceExtended>();
        /// <summary>
        /// contains all groups and the sinamic G devices they contain
        /// </summary>
        public List<deviceUsergroupExtended> lsUserGroupsExtended = new List<deviceUsergroupExtended>();



        public List<ConfigurationPcInterface> lsPcInterfaces= new List<ConfigurationPcInterface>();
        #endregion

        #region general Openness
        public Openness()
        {

        }

        //Start Tia Portal
        public void StartTiaPortal(bool UI)
        {
            if (UI)
            {
                tiaPortal = new TiaPortal(TiaPortalMode.WithUserInterface);
            }
            else
            {
                tiaPortal = new TiaPortal(TiaPortalMode.WithoutUserInterface);
            }
        }

        //Open Project
        public void OpenProject(String path)
        {
            tiaProject = tiaPortal.Projects.Open(new FileInfo(path));
        }

        //Connect to Open Project
        public void ConnectToOpenProject()
        {
            IList<TiaPortalProcess> processes = TiaPortal.GetProcesses();
            switch (processes.Count)
            {
                case 1:
                    tiaPortal = processes[0].Attach();


                    if (tiaPortal.Projects.Count <= 0)
                    {
                        return;
                    }
                    tiaProject = tiaPortal.Projects[0];
                    break;

                case 0:
                    return;

                default:
                    return;
            }
        }

        //Add PLC
        public void AddPlcObject(String name, String mlfb, String version)
        {
            String deviceID = "Ordernumber:" + name + "/V" + version;
            tiaProject.Devices.CreateWithItem(deviceID, name, name);
        }

        //Add Drive
        public void AddDriveObject(String name, String mlfb, String version, String type)
        {
            //TODO
        }

        #endregion



        /// <summary>
        /// Get All Sinamics G devices (Filter on type contains "Sinamics_G")
        /// 
        /// creates myUserGroup for each user group and adds the G devices in this class and the name of the group
        /// </summary>
        public void ListSinamicsDevices()
        {


            lsDeviceExtendeds.Clear();

            //devices not in a specific group
            deviceUsergroupExtended noUserGroup = new deviceUsergroupExtended(null);
            foreach (Device device in tiaProject.Devices)
            {
                if (lsUserGroupsExtended.Count == 0)
                {
                    lsUserGroupsExtended.Add(noUserGroup);
                }

                if (device.TypeIdentifier != null && device.TypeIdentifier.Contains("G1"))// (device.Name.Contains("SINAMICS G"))
                {
                    lsDeviceExtendeds.Add(new DeviceExtended(device, null));
                    noUserGroup.lsUsergroupDevices.Add(device);
                }
            }
            noUserGroup.sortDrivesAlfabetic();

            //de folders
            foreach (DeviceUserGroup usergroup in tiaProject.DeviceGroups)
            {
                deviceUsergroupExtended tempUsergroup = new deviceUsergroupExtended(usergroup);
                lsUserGroupsExtended.Add(tempUsergroup);
                foreach (Device device in usergroup.Devices)
                {
                    if (device.TypeIdentifier != null && device.TypeIdentifier.Contains("G1"))// (device.Name.Contains("SINAMICS G"))
                    {
                        lsDeviceExtendeds.Add(new DeviceExtended(device, usergroup));
                        tempUsergroup.lsUsergroupDevices.Add(device);
                    }
                }

                tempUsergroup.sortDrivesAlfabetic();
            }


        }

        public void ListPcInterfaces()// TODO check
        {
            if(tiaProject.Devices[0] != null)
            {
                foreach(Device device in tiaProject.Devices)
                {
                    foreach(DeviceItem deviceItem in device.DeviceItems)
                    {
                        try
                        {
                            DownloadProvider downloadProvider = deviceItem.GetService<DownloadProvider>();
                            if(downloadProvider != null)
                            {
                                ConnectionConfiguration connConfiguration = downloadProvider.Configuration;
                                ConfigurationMode configurationMode = connConfiguration.Modes.Find("PN/IE");

                                foreach (ConfigurationPcInterface pcInterface in configurationMode.PcInterfaces)
                                {
                                    lsPcInterfaces.Add(pcInterface);
                                }

                                return;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }
            }
        }



        /// <summary>
        /// copy device to Library (project lib -> master copies)
        /// </summary>
        /// <param name="deviceToCopy"></param>
        /// <returns></returns>
        public MasterCopy DeviceToLibrary(Device deviceToCopy)
        {
            MasterCopySystemFolder masterCopyFolder = tiaProject.ProjectLibrary.MasterCopyFolder;
            MasterCopy masterCopy = masterCopyFolder.MasterCopies.Create(deviceToCopy); // creates master copy of the device to copy

            return masterCopy;
        }

        
        /// <summary>
        ///         /// Create Device from library in general
        /// </summary>
        /// <param name="mastercopy"></param>
        /// <returns></returns>
        public Device AddDeviceFromLibrary(MasterCopy mastercopy, DeviceUserGroup usergroup) 
        {
            if (usergroup==null)
            {
                Device device = tiaProject.Devices.CreateFrom(mastercopy);
                return device;
            }
            else
            {
                Device device = usergroup.Devices.CreateFrom(mastercopy);
                return device;
            }
        }

        /// <summary>
        /// Delete device on device name
        /// </summary>
        /// <param name="deviceName"></param>
        public void DeleteDevice(string deviceName, DeviceUserGroup usergroup)
        {
            if (usergroup == null)
            {
                foreach (Device device in tiaProject.Devices)
                {
                    if (device.DeviceItems[1].Name.Equals(deviceName))
                    {
                        device.Delete();
                        return;
                    }
                }
            }
            else
            {
                foreach (Device device in usergroup.Devices)
                {
                    if (device.DeviceItems[1].Name.Equals(deviceName))
                    {
                        device.Delete();
                        return;
                    }
                }
            }


        }
    }

    
    /// <summary>
    /// contains the device and the group it is in (group can be null)
    /// </summary>
    class DeviceExtended
    {
        //props
        public Device device { get; set; }
        public DeviceUserGroup usergroup { get; set; }
        public string driveName { get; set; }

        public DeviceExtended(Device device, DeviceUserGroup usergroup)
        {
            this.device = device;
            this.usergroup = usergroup;
            driveName = device.DeviceItems[1].Name;
        }
    }

    class deviceUsergroupExtended
    {
        public DeviceUserGroup deviceusergroup { get; set; }
        public List<Device> lsUsergroupDevices = new List<Device>();

        public deviceUsergroupExtended(DeviceUserGroup deviceusergroup)
        {
            this.deviceusergroup = deviceusergroup;
        }

        public void sortDrivesAlfabetic()
        {
            lsUsergroupDevices.Sort((x, y) => string.Compare(x.Name, y.Name)); 
        }

    }


}


