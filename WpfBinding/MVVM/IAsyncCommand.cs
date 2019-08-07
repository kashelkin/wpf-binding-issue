using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfBinding.MVVM
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
