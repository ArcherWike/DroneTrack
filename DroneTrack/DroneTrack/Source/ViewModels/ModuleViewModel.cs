using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using DroneTrack.Source.Data;
using DroneTrack.Source.Messages;
using DroneTrack.Source.Models;



namespace DroneTrack.Source.ViewModels
{
    public partial class ModuleViewModel : ObservableObject
    {
        public void Activate()
        {
            RegisterForMessages();
        }

        protected virtual void RegisterForMessages(){ }

        public void CleanUp()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
        }
    }
}
