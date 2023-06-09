﻿using _1.DataAccess;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2.BusinessLogic
{
    public class NotificationPollingComponent
    {
        static private readonly TimeSpan intervaloConsulta = TimeSpan.FromSeconds(10);
        static private bool _pendingNotifications;
        static private bool _poillingActive = true;
        readonly static string baseEndPoint = "https://qtechapi.azurewebsites.net/autoterra/v1";

        public static event EventHandler<PropertyChangedEventArgs> PropertyChanged;

        private static void OnNotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }

        static public bool PendingNotifications
        {
            get { return _pendingNotifications; }
            private set
            {
                if (_pendingNotifications != value)
                {
                    _pendingNotifications = value;
                    OnNotifyPropertyChanged(nameof(PendingNotifications));
                }
            }
        }

        static public async Task StartPeriodicQuery(long userId)
        {
            while (_poillingActive)
            {
                await Task.Delay(intervaloConsulta);

                try
                {
                    QConsumer qc = new QConsumer();
                    PendingNotifications = await qc.GetAsync<bool>($"{baseEndPoint}/notificaciones/pendientes/{userId}");
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static void StopPeriodicQuery()
        {
            _poillingActive = false;
        }
    }
}
