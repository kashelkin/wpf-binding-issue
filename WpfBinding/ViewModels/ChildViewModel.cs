using System;
using System.Collections.ObjectModel;
using System.Linq;
using WpfBinding.MVVM;

namespace WpfBinding.ViewModels
{
    public class ChildViewModel : BaseViewModel // Implements INotifyPropertyChanged
    {
        public ObservableCollection<string> Collection { get; private set; }

        private string selectedItem;
        public string SelectedItem
        {
            get => selectedItem;
            set
            {
                // Throws on attempt of setting null value
                selectedItem = value ?? throw new NullReferenceException($"{nameof(SelectedItem)} in {nameof(ChildViewModel)} is null!!!");
                NotifyPropertyChanged();
            }
        }

        public ChildViewModel(ObservableCollection<string> collection)
        {
            Collection = collection;
            SelectedItem = collection.FirstOrDefault();
        }
    }
}
