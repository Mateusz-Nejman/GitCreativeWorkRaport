﻿using GitCreativeWorkRaport.Models;
using LibGit2Sharp;
using System.Collections.ObjectModel;
using System.Windows;

namespace GitCreativeWorkRaport
{
    internal static class CommitReceiver
    {
        public static async Task Receive(ObservableCollection<RepoModel> repos, string gitLogin, DateTime startDate, DateTime endDate, ObservableCollection<RaportDataModel> output, Action recalculateHoursAction)
        {
            output.Clear();
            await Task.Run(() =>
            {
                try
                {
                    var commits = Enumerable.Empty<Commit>();
                    foreach (var item in repos)
                    {
                        var repo = new Repository(item.Path);
                        foreach (var branch in repo.Branches)
                        {
                            if (!branch.IsRemote && branch.IsTracking)
                                commits = commits.Union(branch.Commits.Where(x =>
                                    x.Author.Email == gitLogin && x.Committer.When.DateTime >= startDate &&
                                    x.Committer.When.DateTime <= endDate));
                        }

                        commits = commits.Distinct().GroupBy(commit => commit.Id).SelectMany(a => a).
                        OrderByDescending(c => c.Author.When.Date.ToString("yyyy-MM-dd")).ToList();

                        foreach (var commit in commits)
                        {
                            var id = commit.Id.ToString()[..8];

                            if (output.FirstOrDefault(r => r.Id == id, null) != null)
                            {
                                continue;
                            }

                            var raportData = new RaportDataModel(output, recalculateHoursAction)
                            {
                                Id = id,
                                RepoName = repo?.Network.Remotes.FirstOrDefault()?.Url.Split("/").Last() ?? string.Empty,
                                CommitName = FilterCommitMessage(commit),
                                Date = commit.Author.When.Date.ToString("yyyy-MM-dd")
                            };

                            Application.Current.Dispatcher.Invoke(() => output.Add(raportData));
                        }

                        repo?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Błąd");
                }
            });
        }

        private static string FilterCommitMessage(Commit commit)
        {
            return commit.Message.Contains("Merged PR") || commit.Message.Contains("Pull request") || commit.Message.Contains("Merge pull request")
                ? commit.MessageShort
                : commit.Message;
        }
    }
}
