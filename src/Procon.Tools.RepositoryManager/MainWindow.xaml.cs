using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Forms;
using System.IO;

namespace Procon.Tools.RepositoryManager {
    using Procon.Tools.RepositoryManager.Models;
    using Procon.Core.Utils;
    using Procon.Core.Interfaces.Repositories;
    using Procon.Core.Interfaces.Repositories.Objects;
    using Procon.Net.Utils;
    using Ionic.Zip;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public Repository Repository { get; set; }

        public ObservableCollection<LogEntry> LogEntries { get; protected set; }

        public MainWindow() {
            InitializeComponent();
            
            // Fuck all this bullshit, I have no idea what I'm doing.
            this.RefreshBinding();

            this.Repository = new Repository();
            this.Repository.RepositoryLoaded += new Core.Interfaces.Repositories.Objects.Repository.RepositoryEventHandler(Repository_RepositoryLoaded);
            this.Repository.AuthenticationSuccess += new Core.Interfaces.Repositories.Objects.Repository.RepositoryEventHandler(Repository_AuthenticationSuccess);
            this.Repository.AuthenticationFailed += new Core.Interfaces.Repositories.Objects.Repository.RepositoryEventHandler(Repository_AuthenticationFailed);
            this.Repository.RebuildCacheSuccess += new Core.Interfaces.Repositories.Objects.Repository.RepositoryEventHandler(Repository_RebuildCacheSuccess);
            this.Repository.RebuildCacheFailed += new Core.Interfaces.Repositories.Objects.Repository.RepositoryEventHandler(Repository_RebuildCacheFailed);
            this.Repository.PublishSuccess += new Core.Interfaces.Repositories.Objects.Repository.RepositoryEventHandler(Repository_PublishSuccess);
            this.Repository.PublishFailed += new Core.Interfaces.Repositories.Objects.Repository.RepositoryEventHandler(Repository_PublishFailed);

            this.LogEntries = new ObservableCollection<LogEntry>();
            CollectionViewSource.GetDefaultView(this.LogEntries).CollectionChanged += new NotifyCollectionChangedEventHandler(LogEntries_CollectionChanged);

            this.LoadDefaults();

            this.QueryRepository();
        }

        private void Repository_PublishFailed(Repository repository) {
            if (this.Dispatcher.CheckAccess() == false) {
                this.Dispatcher.BeginInvoke(new Action<Repository>(Repository_PublishFailed), repository);
                return;
            }

            this.LogEntries.Add(new LogEntry("Error while publishing package"));
        }

        private void Repository_PublishSuccess(Repository repository) {
            if (this.Dispatcher.CheckAccess() == false) {
                this.Dispatcher.BeginInvoke(new Action<Repository>(Repository_PublishSuccess), repository);
                return;
            }

            this.LogEntries.Add(new LogEntry("Package published!"));

            this.QueryRepository();
        }

        private void Repository_RebuildCacheFailed(Repository repository) {
            if (this.Dispatcher.CheckAccess() == false) {
                this.Dispatcher.BeginInvoke(new Action<Repository>(Repository_RebuildCacheFailed), repository);
                return;
            }

            this.LogEntries.Add(new LogEntry("Cache Rebuild failed"));
        }

        private void Repository_RebuildCacheSuccess(Repository repository) {
            if (this.Dispatcher.CheckAccess() == false) {
                this.Dispatcher.BeginInvoke(new Action<Repository>(Repository_RebuildCacheSuccess), repository);
                return;
            }

            this.LogEntries.Add(new LogEntry("Cache Rebuilt"));
        }

        private void LogEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.NewItems != null && this.ListViewLog.Items.Count > 0) {
                this.ListViewLog.ScrollIntoView(this.ListViewLog.Items[this.ListViewLog.Items.Count - 1]);
            }
        }

        private void LoadDefaults() {
            this.txtRepositoryUrl.Text = Defines.MYRCON_COM_REPO_PROCON2;
        }

        private void RefreshBinding() {
            this.DataContext = null;
            this.DataContext = this;
        }

        private void RebuildCache() {
            this.LogEntries.Add(new LogEntry("Starting cache rebuild test.."));
            this.Repository.BeginRebuildCache();
        }

        private void TestAuthentication() {
            this.LogEntries.Add(new LogEntry("Starting authentication test.."));
            this.Repository.BeginAuthenticationTest();
        }

        private void QueryRepository() {
            this.LogEntries.Add(new LogEntry("Starting repository query.."));
            this.Repository.BeginLoading();
        }

        private void Repository_AuthenticationFailed(Repository repository) {
            if (this.Dispatcher.CheckAccess() == false) {
                this.Dispatcher.BeginInvoke(new Action<Repository>(Repository_AuthenticationFailed), repository);
                return;
            }

            this.LogEntries.Add(new LogEntry("Authentication Failed"));
        }

        private void Repository_AuthenticationSuccess(Repository repository) {
            if (this.Dispatcher.CheckAccess() == false) {
                this.Dispatcher.BeginInvoke(new Action<Repository>(Repository_AuthenticationSuccess), repository);
                return;
            }

            this.LogEntries.Add(new LogEntry("Authentication Success"));
        }

        private void Repository_RepositoryLoaded(Repository repository) {
            if (this.Dispatcher.CheckAccess() == false) {
                this.Dispatcher.BeginInvoke(new Action<Repository>(Repository_RepositoryLoaded), repository);
                return;
            }

            this.LogEntries.Add(new LogEntry("Repository Loaded"));

            this.RefreshBinding();
        }

        private void btnPackageDirectory_Click(object sender, RoutedEventArgs e) {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = this.txtPackageDirectory.Text;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                this.txtPackageDirectory.Text = dialog.SelectedPath;
            }
        }

        private void SetVersion(Version v) {
            this.txtPackageVersionMajor.Text = v.Major.ToString();
            this.txtPackageVersionMinor.Text = v.Minor.ToString();
            this.txtPackageVersionBuild.Text = v.Build.ToString();
            this.txtPackageVersionRevision.Text = v.Revision.ToString();
        }

        private void ListViewEmployeeDetails_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            foreach (Package package in e.AddedItems) {
                this.txtPackageName.Text = package.Name;
                this.txtPackageUid.Text = package.Uid;

                this.SetVersion(new Version(
                    package.LatestVersion.Version.Major,
                    package.LatestVersion.Version.Minor,
                    package.LatestVersion.Version.Build,
                    package.LatestVersion.Version.Revision + 1
                ));
            }
        }

        private void txtPackageName_TextChanged(object sender, TextChangedEventArgs e) {
            this.txtPackageUid.Text = this.txtPackageName.Text.SanitizeDirectory();
        }

        private bool IsNumericOnly(string text) {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void txtPackageVersion_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = !this.IsNumericOnly(e.Text);
        }

        private void txtRepositoryUrl_TextChanged(object sender, TextChangedEventArgs e) {
            this.Repository.Url = this.txtRepositoryUrl.Text;
        }

        private void txtUsername_TextChanged(object sender, TextChangedEventArgs e) {
            this.Repository.Username = this.txtUsername.Text;
        }

        private void txtPassword_TextChanged(object sender, TextChangedEventArgs e) {
            this.Repository.Password = this.txtPassword.Text;
        }

        private void btnQuery_Click(object sender, RoutedEventArgs e) {
            this.Repository.Packages.Clear();

            this.QueryRepository();
        }

        private void btnTestAuthentication_Click(object sender, RoutedEventArgs e) {
            this.TestAuthentication();
        }

        private void btnRebuildCache_Click(object sender, RoutedEventArgs e) {
            this.RebuildCache();
        }

        /// <summary>
        /// Zips up a directory, returns a memorystream of the contained zip file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private MemoryStream ZipDirectory(String path) {
            MemoryStream stream = new MemoryStream();

            using (ZipFile zip = new ZipFile()) {
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;

                zip.AddDirectory(path);

                zip.Save(stream);

                stream.Seek(0, SeekOrigin.Begin);
            }

            return stream;
        }

        private void btnPublish_Click(object sender, RoutedEventArgs e) {

            this.LogEntries.Add(new LogEntry("Building package.."));

            if (Directory.Exists(this.txtPackageDirectory.Text) == true) {

                Package package = new Package() {
                    Uid= this.txtPackageUid.Text,
                    Name = this.txtPackageName.Text
                };

                Version version = new Version(String.Format("{0:0}.{1:0}.{2:0}.{3:0}", this.txtPackageVersionMajor.Text, this.txtPackageVersionMinor.Text, this.txtPackageVersionBuild.Text, this.txtPackageVersionRevision.Text));

                MemoryStream stream = this.ZipDirectory(this.txtPackageDirectory.Text);

                if (stream != null) {
                    this.LogEntries.Add(new LogEntry("Starting publish.."));
                    this.Repository.BeginPublish(package, version, stream);
                }
            }
            else {
                this.LogEntries.Add(new LogEntry("Directory \"{0}\" does not exist", this.txtPackageDirectory.Text));
            }
        }
    }
}
