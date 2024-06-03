using System.Collections.ObjectModel;
using System.Windows.Input;

namespace GitCreativeWorkRaport.Models
{
    internal class RaportDataModel : PropertyChangedBase
    {
        private readonly ObservableCollection<RaportDataModel> _collection;
        private double _time;
        private int _index;
        private string _date = string.Empty;
        private string _repoName = string.Empty;
        private string _id = string.Empty;
        private string _commitName = string.Empty;

        public double Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                OnPropertyChanged();
            }
        }

        public string Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public string RepoName
        {
            get => _repoName;
            set
            {
                _repoName = value;
                OnPropertyChanged();
            }
        }

        public string CommitName
        {
            get => _commitName;
            set
            {
                _commitName = value;
                OnPropertyChanged();
            }
        }

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public ICommand RemoveCommand { get; }

        public RaportDataModel(ObservableCollection<RaportDataModel> collection)
        {
            _collection = collection;
            RemoveCommand = new ActionCommand(() =>
            {
                _collection.Remove(_collection.First(c => c.Id == Id));
            });
        }
    }
}
