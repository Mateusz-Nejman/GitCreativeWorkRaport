using GitCreativeWorkRaport.Models;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace GitCreativeWorkRaport.ViewModels
{
    internal class MainViewModel : PropertyChangedBase
    {
        private readonly DateTime _dateNow = DateTime.Now;
        private DateTime _startDate;
        private DateTime _endDate;
        private string _repoPath = string.Empty;
        private string _login = string.Empty;
        private int _dayCount;
        private double _hoursCount;
        private double _percent;
        private bool _processStarted = false;
        private ObservableCollection<RepoModel> _repos = [];
        private ObservableCollection<RaportDataModel> _commits = [];

        public DateTime StartTime
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndTime
        {
            get => _endDate.AddHours(23.99);
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        public string RepoPath
        {
            get => _repoPath;
            set
            {
                _repoPath = value;
                OnPropertyChanged();
            }
        }

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public int DayCount
        {
            get => _dayCount;
            set
            {
                _dayCount = value;
                HoursCount = CalculateHoursCount();
                OnPropertyChanged();
            }
        }

        public double HoursCount
        {
            get => _hoursCount;
            set
            {
                _hoursCount = value;
                Percent = CalculatePercent();
                OnPropertyChanged();
            }
        }

        public double Percent
        {
            get => _percent;
            set
            {
                _percent = value;
                OnPropertyChanged();
            }
        }

        public bool ProcessStarted
        {
            get => _processStarted;
            set
            {
                _processStarted = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProcessStartedNeg));
            }
        }

        public bool ProcessStartedNeg => !ProcessStarted;

        public ObservableCollection<RepoModel> Repos
        {
            get => _repos;
            set
            {
                _repos = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<RaportDataModel> Commits
        {
            get => _commits;
            set
            {
                _commits = value;
                OnPropertyChanged();
            }
        }

        public ICommand GenerateCommand { get; }
        public ICommand AddRepoCommand { get; }
        public ICommand SaveCommand { get; }

        public MainViewModel()
        {
            StartTime = new(_dateNow.Year, _dateNow.Month, 1);
            EndTime = new(_dateNow.Year, _dateNow.Month, DateTime.DaysInMonth(_dateNow.Year, _dateNow.Month));
            GenerateCommand = new AsyncCommand(Generate);
            AddRepoCommand = new ActionCommand(AddRepoPath);
            SaveCommand = new ActionCommand(Save);

            ConfigModel config = ConfigModel.Load("config.json");
            Login = config.Login;
            Repos = new ObservableCollection<RepoModel>(config.RepoPaths.Select((value, index) =>
            {
                return new RepoModel(Repos)
                {
                    Index = index + 1,
                    Path = value
                };
            }));

            Commits.CollectionChanged += Commits_CollectionChanged;
        }

        private void Commits_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(var repo in Commits.Select((value, index) => new { index, value }))
            {
                repo.value.Index = repo.index + 1;
            }
        }

        private double CalculatePercent()
        {
            int hourInDay = 8;
            double hoursInWorkDays = DayCount * hourInDay;
            return Math.Round(CalculateHoursCount() / hoursInWorkDays * 100, 2);
        }

        private double CalculateHoursCount()
        {
            double hoursCount = 0;
            foreach (var raport in Commits)
            {
                hoursCount += raport.Time;
            }

            return hoursCount;
        }

        private void RecalculateHours()
        {
            HoursCount = CalculateHoursCount();
        }

        private void AddRepoPath()
        {
            OpenFolderDialog dialog = new()
            {
                Multiselect = false,
                Title = "Wybierz folder"
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                if(Directory.Exists(dialog.FolderName))
                {
                    Repos.Add(new RepoModel(Repos)
                    {
                        Index = Repos.Count + 1,
                        Path = dialog.FolderName,
                    });
                }
            }
        }

        private async Task Generate()
        {
            ConfigModel model = new()
            {
                Login = Login,
                RepoPaths = Repos.Select(r => r.Path).ToList()
            };
            model.Save("config.json");

            ProcessStarted = true;
            Mouse.OverrideCursor = Cursors.Wait;
            Commits.Clear();
            HoursCount = 0;
            Percent = 0;
            await CommitReceiver.Receive(Repos, Login, StartTime, EndTime, Commits, RecalculateHours);

            foreach (var commit in Commits.Select((value, i) => new { i, value }))
            {
                commit.value.Index = commit.i + 1;
            }

            Mouse.OverrideCursor = null;
            ProcessStarted = false;
        }

        private void Save()
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Zapisz raport",
                Filter = "Document files (*.docx)|*.docx",
                FileName = "raport." + _startDate.ToString("MM.yyyy") + ".docx"
            };

            if (saveFileDialog.ShowDialog() == true && saveFileDialog.FileName != "")
            {
                try
                {
                    DocumentFormater formater = new(saveFileDialog.FileName);
                    formater.CreateHeading(_startDate, _endDate);
                    formater.CreateTable(Commits);
                    formater.CreateEnding(HoursCount, _startDate);
                    formater.SaveAndClose();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Błąd");
                }
            }
        }
    }
}
