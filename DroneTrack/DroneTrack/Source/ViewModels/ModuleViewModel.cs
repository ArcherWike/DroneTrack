using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;


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
