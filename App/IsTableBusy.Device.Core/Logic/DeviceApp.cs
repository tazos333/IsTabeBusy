﻿

using IsTableBusy.Device.Core.Exceptions;
using System;

namespace IsTableBusy.Device.Core.Logic
{
    public sealed class DeviceApp
    {
        private readonly IoTDevice device;
        private readonly ApiClient apiClient;
        private readonly LightManager lightManager;

        private Table Table { get; set; }

        public DeviceApp(IoTDevice device, ApiClient apiClient)
        {
            this.device = device;
            this.apiClient = apiClient;
            this.lightManager = new LightManager(device);

            this.State = AppState.NotStarted;
        }

        private AppState state;

        public AppState State
        {
            get
            {
                return state;
            }
            private set
            {
                state = value;
                lightManager.Apply(state);
            }
        }

        public void Run()
        {
            try
            {
                apiClient.RegisterDevice();
                RefreshState();
                this.device.Button.Clicked += Button_Clicked;
            }
            catch (ReadingTableException)
            {
                this.State = AppState.Error;
            }
            catch(Exception ex)
            {
                switch (ex.Message)
                {
                    case "Device registration error":
                    case "Device is not connected to device":
                        {
                            this.State = AppState.Error;
                            return;
                        }
                    default:
                        {
                            throw;
                        }
                }
                
            }
        }

        private void RefreshState()
        {
            Table = apiClient.GetTable();
            if(Table == null)
            {
                throw new Exception("Device is not connected to device");
            }
            if (Table.IsBusy)
            {
                this.State = AppState.Busy;
            }
            else
            {
                this.State = AppState.Free;
            }
        }

        private void Button_Clicked(Button sender, object args)
        {
            try
            {
                HandleClick();
            }
            catch (CachngeTableStateException)
            {
                this.State = AppState.Error;
            }
        }

        private void HandleClick()
        {
            switch (this.State)
            {
                case AppState.NotStarted:
                    {
                        break;
                    }
                case AppState.Working:
                    {
                        break;
                    }
                case AppState.Busy:
                    {
                        UpdateToFreeState();
                        break;
                    }
                case AppState.Free:
                    {
                        UpdateToBusyState();
                        break;
                    }
                case AppState.Error:
                    {
                        RefreshState();
                        break;
                    }
            }
        }

        private void UpdateToFreeState()
        {
            this.State = AppState.Working;
            this.apiClient.SetBusy(false);
            this.State = AppState.Free;

        }

        private void UpdateToBusyState()
        {
            this.State = AppState.Working;
            this.apiClient.SetBusy(true);
            this.State = AppState.Busy;
        }
    }
}

