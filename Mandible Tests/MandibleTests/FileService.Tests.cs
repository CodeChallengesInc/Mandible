using CodeChallengeInc.Mandible.Constants;
using CodeChallengeInc.Mandible.Models;
using CodeChallengeInc.Mandible.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CodeChallengeInc.Mandible.Tests
{
    [TestClass]
    public class FileServiceTests
    {
        private Mock<IFileSystem> _mockFileSystem;
        private FileService _fileService;
        string submissionsPath = @"LoneAnt\Submissions";
        string slash = @"\";
        const string userName = "testUser";
        const string antName = "testAnt";
        List<string> expectedSubmissionNames = new List<string> { $"{userName}1_{antName}1", $"{userName}2_{antName}2" };
        List<string> fileNames = new List<string> { $"{userName}1_{antName}1.js", $"{userName}2_{antName}2.js" };
        string userSubmissionPath;
        List<string> filePaths;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockFileSystem = new Mock<IFileSystem>();
            _fileService = new FileService(_mockFileSystem.Object);

            _mockFileSystem.Setup(fs => fs.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns<string, string>((a, b) => $@"{a}\{b}");

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                submissionsPath = "LoneAnt/Submissions";
                slash = "/";
            }
            userSubmissionPath = $@"LoneAnt{slash}Submissions{slash}{userName}1_{antName}1.js";
            filePaths = new List<string> { $@"{submissionsPath}{slash}{userName}1_{antName}1.js", $@"{submissionsPath}{slash}{userName}2_{antName}2.js" };
        }

        [TestMethod]
        public void BackupUserSubmission_WhenCalled_ReadsFileAndWritesBackup()
        {

            string path = "testPath";
            string content = "testContent";
            string backupPath = "backupPath";
            string backupLocation = Path.Combine(backupPath, "testFile - 2022-01-01 00:00:00.txt");

            _mockFileSystem.Setup(fs => fs.File.ReadAllText(path)).Returns(content);
            _mockFileSystem.Setup(fs => fs.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns(backupLocation);


            _fileService.BackupUserSubmission(path);


            _mockFileSystem.Verify(fs => fs.File.ReadAllText(path), Times.Once);
            _mockFileSystem.Verify(fs => fs.File.WriteAllText(backupLocation, content), Times.Once);
        }

        [TestMethod]
        public void BackupUserSubmission_WhenFileDoesNotExist_ThrowsFileNotFoundException()
        {

            string path = "nonexistentPath";
            _mockFileSystem.Setup(fs => fs.File.ReadAllText(path)).Throws<FileNotFoundException>();

            Assert.ThrowsException<FileNotFoundException>(() => _fileService.BackupUserSubmission(path));
        }

        [TestMethod]
        public void BackupUserSubmission_WhenFileCannotBeRead_ThrowsIOException()
        {
            string path = "unreadablePath";
            _mockFileSystem.Setup(fs => fs.File.ReadAllText(path)).Throws<IOException>();

            Assert.ThrowsException<IOException>(() => _fileService.BackupUserSubmission(path));
        }

        [TestMethod]
        public void BackupUserSubmission_WhenBackupCannotBeWritten_ThrowsIOException()
        {

            string path = "validPath";
            string content = "validContent";
            string backupPath = "backupPath";
            string backupLocation = Path.Combine(backupPath, "backupFile");

            _mockFileSystem.Setup(fs => fs.File.ReadAllText(path)).Returns(content);
            _mockFileSystem.Setup(fs => fs.Path.Combine(It.IsAny<string>(), It.IsAny<string>())).Returns(backupLocation);
            _mockFileSystem.Setup(fs => fs.File.WriteAllText(backupLocation, content)).Throws<IOException>();

            Assert.ThrowsException<IOException>(() => _fileService.BackupUserSubmission(path));
        }

        [TestMethod]
        public void GetSubmissionNames_ShouldReturnCorrectNames_WhenFilesExist()
        {
            _mockFileSystem.Setup(fs => fs.Directory.EnumerateFiles(It.IsAny<string>())).Returns(filePaths);

            List<string> submissionNames = _fileService.GetSubmissionNames();

            Assert.AreEqual(expectedSubmissionNames.Count, submissionNames.Count);
            Assert.AreEqual(expectedSubmissionNames[0], submissionNames[0]);
            Assert.AreEqual(expectedSubmissionNames[1], submissionNames[1]);
        }

        [TestMethod]
        public void PurgeDefaultAnts_ShouldDeleteDefaultAnts_WhenDefaultAntsExist()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Inconclusive("Test is not supported on non-Windows platforms.");
                return;
            }

            List<string> defaultAntPaths = new List<string> { $@"{submissionsPath}{slash}cci_default1.js", $@"{submissionsPath}{slash}cci_default2.js" };

            _mockFileSystem.Setup(fs => fs.Directory.EnumerateFiles(It.IsAny<string>())).Returns(defaultAntPaths);
            _mockFileSystem.Setup(fs => fs.File.Exists(It.IsAny<string>())).Returns(true);
            _mockFileSystem.Setup(fs => fs.File.Delete(It.IsAny<string>()));
            _mockFileSystem.Setup(fs => fs.File.ReadAllText(It.IsAny<string>())).Returns(FileInformation.DefaultAntString);

            _fileService.PurgeDefaultAnts();


            foreach (var defaultAntPath in defaultAntPaths)
            {
                _mockFileSystem.Verify(fs => fs.File.Delete(defaultAntPath), Times.Once);
            }
        }

        [TestMethod]
        public void PurgeDefaultAnts_ShouldNotDeleteDefaultAnts_WhenDefaultAntDoesntExist()
        {
            List<string> defaultAntPaths = new List<string> { $@"{submissionsPath}{slash}cci_default1.js", $@"{submissionsPath}{slash}cci_default2.js" };

            _mockFileSystem.Setup(fs => fs.Directory.EnumerateFiles(It.IsAny<string>(), It.IsAny<string>())).Returns(defaultAntPaths);
            _mockFileSystem.Setup(fs => fs.File.Exists(It.IsAny<string>())).Returns(true);

            _fileService.PurgeDefaultAnts();

            foreach (var defaultAntPath in defaultAntPaths)
            {
                _mockFileSystem.Verify(fs => fs.File.Delete(defaultAntPath), Times.Never);
            }
        }

        [TestMethod]
        public void GetSubmissionsResponse_ShouldReturnCorrectSubmissions_WhenSubmissionsExist()
        {
            List<LoneAntSubmissionResponse> expectedSubmissions = new List<LoneAntSubmissionResponse>
            {
                new LoneAntSubmissionResponse { Username = $"{userName}1", AntName = $"{antName}1", Submission = "submission1" },
                new LoneAntSubmissionResponse { Username = $"{userName}2", AntName = $"{antName}2", Submission = "submission2" }
            };

            _mockFileSystem.Setup(fs => fs.Directory.EnumerateFiles(It.IsAny<string>())).Returns(filePaths);
            _mockFileSystem.Setup(fs => fs.File.ReadAllText(It.IsAny<string>())).Returns<string>(path => path.Contains($"{userName}1") ? "submission1" : "submission2");

            List<LoneAntSubmissionResponse> submissions = _fileService.GetSubmissionsReponse();

            foreach (var submission in submissions.Select((LoneAntSubmissionResponse value, int index) => new { value = value, index = index }))
            {
                var actual = submission.value;
                var expected = expectedSubmissions[submission.index];
                Assert.AreEqual(expected.Username, actual.Username);
                Assert.AreEqual(expected.AntName, actual.AntName);
                Assert.AreEqual(expected.Submission, actual.Submission);
            }
        }

        [TestMethod]
        public void GetSubmissionsResponse_ShouldReturnEmptyList_WhenSubmissionsDontExist()
        {
            List<LoneAntSubmissionResponse> expectedSubmissions = new List<LoneAntSubmissionResponse>
            {
                new LoneAntSubmissionResponse { Username = "user1", AntName = "ant1", Submission = "submission1" },
                new LoneAntSubmissionResponse { Username = "user2", AntName = "ant2", Submission = "submission2" }
            };

            _mockFileSystem.Setup(fs => fs.Directory.EnumerateFiles(It.IsAny<string>())).Returns(new List<string>());

            List<LoneAntSubmissionResponse> submissions = _fileService.GetSubmissionsReponse();

            foreach (var submission in submissions.Select((LoneAntSubmissionResponse value, int index) => new { value = value, index = index }))
            {
                var actual = submission.value;
                var expected = expectedSubmissions[submission.index];
                Assert.AreEqual(expected.Username, actual.Username);
                Assert.AreEqual(expected.AntName, actual.AntName);
                Assert.AreEqual(expected.Submission, actual.Submission);
            }
        }

        [TestMethod]
        public void GetUserSubmission_ShouldReturnCorrectSubmission_WhenSubmissionExists()
        {
            string submission = "Test submission";

            _mockFileSystem.Setup(fs => fs.File.ReadAllText(It.IsAny<string>())).Returns(submission);

            LoneAntSubmissionResponse result = _fileService.GetUserSubmission(userName, antName);

            Assert.AreEqual(userName, result.Username);
            Assert.AreEqual(antName, result.AntName);
            Assert.AreEqual(submission, result.Submission);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void GetUserSubmission_ShouldThrowFileNotFoundException_WhenSubmissionDoesNotExist()
        {
            _mockFileSystem.Setup(fs => fs.File.ReadAllText(It.IsAny<string>())).Throws<FileNotFoundException>();

            _fileService.GetUserSubmission(userName, antName);
        }

        [TestMethod]
        public void UserSubmissionExists_ShouldReturnTrue_WhenSubmissionExists()
        {
            _mockFileSystem.Setup(fs => fs.File.Exists(It.IsAny<string>())).Returns(true);

            bool result = _fileService.UserSubmissionExists(userName, antName);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UserSubmissionExists_ShouldReturnFalse_WhenSubmissionDoesNotExist()
        {
            _mockFileSystem.Setup(fs => fs.File.Exists(It.IsAny<string>())).Returns(false);

            bool result = _fileService.UserSubmissionExists(userName, antName);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void DeleteUserSubmission_ShouldDeleteSubmission_WhenSubmissionExists()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.Inconclusive("Test is not supported on non-Windows platforms.");
                return;
            }

            _mockFileSystem.Setup(fs => fs.File.Exists(It.IsAny<string>())).Returns(true);

            _fileService.DeleteUserSubmission($"{userName}1", $"{antName}1");

            _mockFileSystem.Verify(fs => fs.File.Delete(userSubmissionPath), Times.Once);
        }

        [TestMethod]
        public void DeleteUserSubmission_ShouldNotDeleteSubmission_WhenSubmissionDoesNotExist()
        {
            _mockFileSystem.Setup(fs => fs.File.Exists(It.IsAny<string>())).Returns(false);

            _fileService.DeleteUserSubmission($"{userName}1", $"{antName}1");

            _mockFileSystem.Verify(fs => fs.File.Delete(userSubmissionPath), Times.Never);
        }
    }
}
