using LibGit2Sharp;
using System;
using System.IO;
using System.Linq;

namespace GitDeploy
{
    public class GitRepositoryManager : IGitRepositoryManager
    {
        private readonly string _repoSource, _authorName, _authorEmail, _committerName, _committerEmail;
        private readonly UsernamePasswordCredentials _credentials;
        private readonly DirectoryInfo _localFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitRepositoryManager" /> class.
        /// </summary>
        /// <param name="username">The Git credentials username.</param>
        /// <param name="password">The Git credentials password.</param>
        /// <param name="gitRepoUrl">The Git repo URL.</param>
        /// <param name="localFolder">The full path to local folder.</param>
        public GitRepositoryManager(string username, string password, string gitRepoUrl, string localFolder,
            string authorname,
            string authoremail,
            string committername,
            string committeremail)
        {
            var folder = new DirectoryInfo(localFolder);

            if (!folder.Exists)
            {
                throw new Exception(string.Format("Source folder '{0}' does not exist.", _localFolder));
            }

            _localFolder = folder;

            _credentials = new UsernamePasswordCredentials
            {
                Username = username,
                Password = password
            };

            _repoSource = gitRepoUrl;

            _authorName = authorname;
            _authorEmail = authoremail;
            _committerName = committername;
            _committerEmail = committeremail;
        }

        /// <summary>
        /// Commits all changes.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="System.Exception"></exception>
        public void CommitAllChanges(string message, string file, string solutionFilePath)
        {
            try
            {
                Console.WriteLine("Committing solutions");
                using (var repo = new Repository(_localFolder.FullName))
                {
                    if (string.IsNullOrEmpty(file))
                    {
                        //var files = _localFolder.GetFiles("*.zip", SearchOption.AllDirectories).Select(f => f.FullName);
                        var files = _localFolder.GetFiles("*.zip").Select(f => f.FullName);
                        //string message = commitMessage + " ";

                        {
                            foreach (var f in files)
                            {
                                if (string.IsNullOrEmpty(file))
                                {
                                    //message += k + ",";
                                    repo.Index.Add(f.Replace(_localFolder.FullName, string.Empty));
                                }
                                else
                                {
                                    if (f.EndsWith(file))
                                        repo.Index.Add(f.Replace(_localFolder.FullName, string.Empty));
                                }
                            }
                        }
                    }
                    else
                    {
                        repo.Index.Add(file.Replace(_localFolder.FullName, string.Empty));
                    }

                    repo.Index.Add(solutionFilePath.Replace(_localFolder.FullName, string.Empty));

                    var offset = DateTimeOffset.Now;
                    Signature author = new Signature(_authorName, _authorEmail, offset);
                    Signature committer = new Signature(_committerName, _committerEmail, offset);
                    CommitOptions commitOptions = new CommitOptions();
                    commitOptions.AllowEmptyCommit = false;

                    repo.Commit(message, author, committer);
                }
            }
            catch (EmptyCommitException ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Pushes all commits.
        /// </summary>
        /// <param name="remoteName">Name of the remote server.</param>
        /// <param name="branchName">Name of the remote branch.</param>
        /// <exception cref="System.Exception"></exception>
        public void PushCommits(string remoteName, string branchName)
        {
            using (var repo = new Repository(_localFolder.FullName))
            {
                var remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == remoteName);
                if (remote == null)
                {
                    repo.Network.Remotes.Add(remoteName, _repoSource);
                    remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == remoteName);
                }
                var options = new PushOptions
                {
                    CredentialsProvider = (url, usernameFromUrl, types) => _credentials
                };
                options.OnPushStatusError += PushSatusErrorHandler;

                string pushRefs = "refs/heads/testsyed";
                Branch branchs = null;
                foreach (var branch in repo.Branches)
                {
                    if (branch.FriendlyName.ToLower().Equals(branchName.ToLower()))
                    {
                        branchs = branch;
                        pushRefs = branch.Reference.CanonicalName;
                    }
                }

                Console.WriteLine("Pushing Changes to the Repository ");
                //repo.Network.Push(branchs);
                repo.Network.Push(remote, pushRefs + ":" + pushRefs, options);
                repo.Network.Push(repo.Network.Remotes.FirstOrDefault(r => r.Name == "remotes/origin"), pushRefs, options);
                Console.WriteLine("Pushed changes");
            }
        }

        private void PushSatusErrorHandler(PushStatusError pushStatusErrors)
        {
            throw new NotImplementedException();
        }
    }

    public interface IGitRepositoryManager
    {
        void CommitAllChanges(string message, string file, string solutionFilePath);

        void PushCommits(string remoteName, string branchName);
    }
}