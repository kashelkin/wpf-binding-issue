using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

        private NotifyTaskCompletion<string> awaitedText;
        public NotifyTaskCompletion<string> AwaitedText
        {
            get => awaitedText;
            set
            {
                awaitedText = value;
                NotifyPropertyChanged();
            }
        }

        private AsyncCommand<object> openChildWindowCommand;
        public AsyncCommand<object> OpenChildWindowCommand
        {
            get => openChildWindowCommand ?? (openChildWindowCommand = AsyncCommand.Create(async () =>
            {
                AwaitedText = new NotifyTaskCompletion<string>(GetAwaitedText("There is no blocking!"));

                var vm = new ChildViewModel(Collection);
                await App.WindowManager.ShowModalPresentation(vm);
            }));
        }

        private async Task<string> GetAwaitedText(string text)
        {
            await Task.Delay(3000);
            return text;
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

        private RelayCommand collectGarbageCommand;
        public RelayCommand CollectGarbageCommand
        {
            get => collectGarbageCommand ?? (collectGarbageCommand = new RelayCommand(obj =>
            {
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
            }));
        }
    }
}
