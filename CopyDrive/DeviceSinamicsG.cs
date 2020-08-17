using OpennessClass;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.MC.Drives;
using Siemens.Engineering.MC.Drives.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyDrive
{
    class DeviceSinamicsG
    {
        // when working with folders in TIA
        public DeviceUserGroup deviceUserGroup { get; set; }

        //Variables
        private Device device;
        private DriveObject driveObject;
        private Telegram safetyTelegram;
        private Telegram mainTelegram;
        private Telegram additionalTelegram;
        private Telegram supplementaryTelegram;
        private NetworkInterface profinetInterface;
        private IoConnector ioConnector;
        private Node node;
        public string Name { get; }

        public string PowerModule { get; }
        public int safetyAddress { get; }
        public int safetHwAddress_in { get; }
        public int safetHwAddress_out { get; }
        public IoSystem ioSystem { get; }
        public Subnet subnet { get; }
        public string ipAddress { get; }
        public string pnDeviceName { get; }
        public string pnDeviceNameAuto { get; }
        public int PnDeviceNumber { get; }
        public int hwAddress_in { get; }
        public int hwAddress_out { get; }
        public int hwAddress_supp_in { get; }
        public int hwAddress_supp_out { get; }
        public int hwAddress_add_in { get; }
        public int hwAddress_add_out { get; }
        public List<NetworkPort> networkPorts { get; set; }


        public DeviceSinamicsG(DeviceExtended deviceextended)
        {
            this.deviceUserGroup = deviceextended.usergroup;

            this.device = deviceextended.device;
            driveObject = device.DeviceItems[1].GetService<DriveObjectContainer>().DriveObjects[0];
            safetyTelegram = driveObject.Telegrams.Find(TelegramType.SafetyTelegram);
            mainTelegram = driveObject.Telegrams.Find(TelegramType.MainTelegram);
            additionalTelegram = driveObject.Telegrams.Find(TelegramType.AdditionalTelegram);
            supplementaryTelegram = driveObject.Telegrams.Find(TelegramType.SupplementaryTelegram);
            profinetInterface = getProfinetInetface();
            ioConnector = profinetInterface.IoConnectors[0];
            node = profinetInterface.Nodes[0];
            Name = device.DeviceItems[1].Name;
            PowerModule = device.DeviceItems[2].TypeIdentifier;
            safetyAddress = GetSafetyAddress();
            safetHwAddress_in = getSafetyHwAddressIn();
            safetHwAddress_out = getSafetyHwAddressOut();
            hwAddress_in = getHwAddressIn();
            hwAddress_out = getHwAddressOut();
            hwAddress_supp_in = getHwAddressSuppIn();
            hwAddress_supp_out = getHwAddressSuppOut();
            hwAddress_add_in = getHwAddressAddIn();
            hwAddress_add_out = getHwAddressAddOut();
            ioSystem = ioConnector.ConnectedToIoSystem;
            subnet = node.ConnectedSubnet;
            ipAddress = node.GetAttribute("Address").ToString();
            pnDeviceNameAuto = node.GetAttribute("PnDeviceNameAutoGeneration").ToString();
            pnDeviceName = node.GetAttribute("PnDeviceName").ToString();
            PnDeviceNumber = (int)ioConnector.GetAttribute("PnDeviceNumber");
            networkPorts = GetNetworkPorts();
        }



        //Get the safetyAddresses
        private int GetSafetyAddress()
        {
            int address = -1;

            if (safetyTelegram != null)
            {
                int.TryParse(safetyTelegram.GetAttribute("Failsafe_FDestinationAddress").ToString(), out address);
            }

            return address;
        }

        private NetworkInterface getProfinetInetface()
        {
            foreach (DeviceItem deviceItem in device.DeviceItems[1].DeviceItems)
            {
                if (deviceItem.Name.Contains("PROFINET"))
                {
                    return deviceItem.GetService<NetworkInterface>();
                }
            }
            return null;
        }

        private int getSafetyHwAddressIn()
        {
            if (safetyTelegram != null)
            {
                foreach (Address address in safetyTelegram.Addresses)
                {
                    if (address.IoType.Equals(AddressIoType.Input))
                    {
                        return address.StartAddress;
                    }
                }
            }
            return -1;
        }

        private int getSafetyHwAddressOut()
        {
            if (safetyTelegram != null)
            {
                foreach (Address address in safetyTelegram.Addresses)
                {
                    if (address.IoType.Equals(AddressIoType.Output))
                    {
                        return address.StartAddress;
                    }
                }
            }
            return -1;
        }

        private int getHwAddressIn()
        {
            if (mainTelegram != null)
            {
                foreach (Address address in mainTelegram.Addresses)
                {
                    if (address.IoType.Equals(AddressIoType.Input))
                    {
                        return address.StartAddress;
                    }
                }
            }
            return -1;
        }

        private int getHwAddressOut()
        {
            if (mainTelegram != null)
            {
                foreach (Address address in mainTelegram.Addresses)
                {
                    if (address.IoType.Equals(AddressIoType.Output))
                    {
                        return address.StartAddress;
                    }
                }
            }
            return -1;
        }

        private int getHwAddressSuppIn()
        {
            if (supplementaryTelegram != null)
            {
                foreach (Address address in supplementaryTelegram.Addresses)
                {
                    if (address.IoType.Equals(AddressIoType.Input))
                    {
                        return address.StartAddress;
                    }
                }
            }
            return -1;
        }

        private int getHwAddressSuppOut()
        {
            if (supplementaryTelegram != null)
            {
                foreach (Address address in supplementaryTelegram.Addresses)
                {
                    if (address.IoType.Equals(AddressIoType.Output))
                    {
                        return address.StartAddress;
                    }
                }
            }
            return -1;
        }

        private int getHwAddressAddIn()
        {
            if (additionalTelegram != null)
            {
                foreach (Address address in additionalTelegram.Addresses)
                {
                    if (address.IoType.Equals(AddressIoType.Input))
                    {
                        return address.StartAddress;
                    }
                }
            }
            return -1;
        }

        private int getHwAddressAddOut()
        {
            if (additionalTelegram != null)
            {
                foreach (Address address in additionalTelegram.Addresses)
                {
                    if (address.IoType.Equals(AddressIoType.Output))
                    {
                        return address.StartAddress;
                    }
                }
            }
            return -1;
        }

        private List<NetworkPort> GetNetworkPorts()
        {
            List<NetworkPort> myNetworkPorts = new List<NetworkPort>();

            foreach (DeviceItem deviceItem in device.DeviceItems[1].DeviceItems)
            {
                if (deviceItem.Name.Contains("PROFINET"))
                {
                    foreach (DeviceItem port in deviceItem.DeviceItems)
                    {
                        NetworkPort networkPort = port.GetService<NetworkPort>();
                        NetworkPortAssociation partnerPortAss = networkPort.ConnectedPorts;

                        if (partnerPortAss.Count > 0)
                        {
                            myNetworkPorts.Add(partnerPortAss[0]);
                        }
                        else
                        {
                            myNetworkPorts.Add(null);
                        }
                    }
                }
            }

            return myNetworkPorts;
        }

        public void updateNetworkPorts()
        {
            networkPorts = GetNetworkPorts();
        }
    }
}
