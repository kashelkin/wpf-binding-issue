using System.Collections.ObjectModel;
using WpfBinding.MVVM;

namespace WpfBinding.ViewModels
{
    public class MainViewModel : BaseViewModel // Implements INotifyPropertyChanged
    {
        private ObservableCollection<string> collection;
        public ObservableCollection<string> Collection
        {
            get => collection ?? (collection = new ObservableCollection<string> { "item 1", "item 2", "item 3" });
        }

        private AsyncCommand<object> openChildWindowCommand;
        public AsyncCommand<object> OpenChildWindowCommand
        {
            get => openChildWindowCommand ?? (openChildWindowCommand = AsyncCommand.Create(async () =>
            {
                var vm = new ChildViewModel(Collection);
                await App.WindowManager.ShowModalPresentation(vm);
            }));
        }

        private RelayCommand clearCollectionCommand;
        public RelayCommand ClearCollectionCommand
        {
            get => clearCollectionCommand ?? (clearCollectionCommand = new RelayCommand(obj =>
            {
                Collection.Clear();
            }));
        }

        private RelayCommand fillCollectionCommand;
        public RelayCommand FillCollectionCommand
        {
            get => fillCollectionCommand ?? (fillCollectionCommand = new RelayCommand(obj =>
            {
                if (Collection.Count == 0)
                {
                    Collection.Add("item 1");
                    Collection.Add("item 2");
                    Collection.Add("item 3");
                }
            }));
        }
    }
}
