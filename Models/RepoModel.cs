using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GitCreativeWorkRaport.Models
{
    internal class RepoModel : PropertyChangedBase
    {
        private readonly ObservableCollection<RepoModel> _repos;
        private int _index;
        private string _path = string.Empty;
        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged();
            }
        }
        public string Path
        {
            get => _path;
            set
            {
                _path = value;
                OnPropertyChanged();
            }
        }
        public ICommand RemoveCommand { get; }

        public RepoModel(ObservableCollection<RepoModel> repos)
        {
            _repos = repos;
            RemoveCommand = new ActionCommand(() =>
            {
                _repos.Remove(this);
            });
        }
    }
}
