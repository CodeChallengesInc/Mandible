using CodeChallengeInc.Mandible.Controllers;
using CodeChallengeInc.Mandible.Interfaces;
using CodeChallengeInc.Mandible.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace CodeChallengeInc.Mandible.Tests
{
    [TestClass]
    public class LoneAntControllerTests
    {
        private Mock<IFileService> _fileServiceMock;
        private Mock<ILogger> _loggerMock;
        private LoneAntController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _fileServiceMock = new Mock<IFileService>();
            _loggerMock = new Mock<ILogger>();
            _controller = new LoneAntController(_fileServiceMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public void GetUserSubmission_WhenUserSubmissionExists_ReturnsOkResult()
        {
            var expectedResponse = new LoneAntSubmissionResponse
            {
                Username = "userName",
                Submission = "submission",
                AntName = "antName"
            };
            _fileServiceMock.Setup(fs => fs.UserSubmissionExists(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _fileServiceMock.Setup(fs => fs.GetUserSubmission(It.IsAny<string>(), It.IsAny<string>())).Returns(expectedResponse);

            var result = _controller.GetUserSubmission("userName", "antName");

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(expectedResponse, okResult!.Value);
        }

        [TestMethod]
        public void GetUserSubmission_WhenUserSubmissionDoesNotExist_ReturnsNotFoundResult()
        {
            _fileServiceMock.Setup(fs => fs.UserSubmissionExists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.GetUserSubmission("userName", "antName");

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var notFoundResult = result.Result as NotFoundObjectResult;
            var errorResponse = notFoundResult!.Value as ErrorResponse;
            Assert.AreEqual(404, errorResponse!.ErrorCode);
            Assert.AreEqual("userName_antName's submission not found.", errorResponse.ErrorMessage);
        }

        [TestMethod]
        public void SubmitUserEntry_WhenSubmissionIsValid_ReturnsNoContentResult()
        {
            string validSubmission = "valid submission";

            var result = _controller.SubmitUserEntry("userName", "antName", validSubmission);

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public void SubmitUserEntry_WhenSubmissionIsEmpty_ReturnsBadRequestResult()
        {
            string emptySubmission = string.Empty;

            var result = _controller.SubmitUserEntry("userName", "antName", emptySubmission);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            var errorResponse = badRequestResult!.Value as ErrorResponse;
            Assert.AreEqual(400, errorResponse!.ErrorCode);
            Assert.AreEqual("Submission cannot be empty.", errorResponse.ErrorMessage);
        }

        [TestMethod]
        public void SubmitUserEntry_WhenIOExceptionIsThrown_ReturnsStatusCode500Result()
        {
            string validSubmission = "valid submission";
            _fileServiceMock.Setup(fs => fs.CreateOrOverwriteUserSubmission(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<IOException>();

            var result = _controller.SubmitUserEntry("userName", "antName", validSubmission);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var statusCodeResult = result as ObjectResult;
            var errorResponse = statusCodeResult!.Value as ErrorResponse;
            Assert.AreEqual(500, errorResponse!.ErrorCode);
            Assert.AreEqual("An error occurred while processing your request.", errorResponse.ErrorMessage);
        }

        [TestMethod]
        public void SubmitUserEntry_WhenArgumentExceptionIsThrown_ReturnsBadRequestResult()
        {
            string validSubmission = "valid submission";
            _fileServiceMock.Setup(fs => fs.CreateOrOverwriteUserSubmission(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<ArgumentException>();

            var result = _controller.SubmitUserEntry("userName", "antName", validSubmission);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            var errorResponse = badRequestResult!.Value as ErrorResponse;
            Assert.AreEqual(400, errorResponse!.ErrorCode);
            Assert.AreEqual("Invalid argument provided.", errorResponse.ErrorMessage);
        }

        [TestMethod]
        public void SubmitUserEntry_WhenExceptionIsThrown_ReturnsStatusCode500Result()
        {
            string validSubmission = "valid submission";
            _fileServiceMock.Setup(fs => fs.CreateOrOverwriteUserSubmission(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            var result = _controller.SubmitUserEntry("userName", "antName", validSubmission);

            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var statusCodeResult = result as ObjectResult;
            var errorResponse = statusCodeResult!.Value as ErrorResponse;
            Assert.AreEqual(500, errorResponse!.ErrorCode);
            Assert.AreEqual("An error occurred while processing your request.", errorResponse.ErrorMessage);
        }

        [TestMethod]
        public void DeleteSubmission_WhenUserSubmissionExists_ReturnsNoContentResult()
        {
            _fileServiceMock.Setup(fs => fs.UserSubmissionExists(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = _controller.DeleteSubmission("userName", "antName");

            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public void DeleteSubmission_WhenUserSubmissionDoesNotExist_ReturnsNotFoundResult()
        {
            _fileServiceMock.Setup(fs => fs.UserSubmissionExists(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = _controller.DeleteSubmission("userName", "antName");

            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            var notFoundResult = result as NotFoundObjectResult;
            var errorResponse = notFoundResult!.Value as ErrorResponse;
            Assert.AreEqual(404, errorResponse!.ErrorCode);
            Assert.AreEqual("userName_antName's submission not found.", errorResponse.ErrorMessage);
        }

        [TestMethod]
        public void GetUserSubmissions_WhenSubmissionsExist_ReturnsOkResult()
        {
            var submissions = new List<LoneAntSubmissionResponse> { new LoneAntSubmissionResponse() };
            _fileServiceMock.Setup(fs => fs.GetSubmissionsReponse()).Returns(submissions);

            var result = _controller.GetUserSubmissions();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(submissions, okResult!.Value);
        }

        [TestMethod]
        public void GetUserSubmissions_WhenNoSubmissionsExist_ReturnsNotFoundResult()
        {
            var submissions = new List<LoneAntSubmissionResponse>();
            _fileServiceMock.Setup(fs => fs.GetSubmissionsReponse()).Returns(submissions);

            var result = _controller.GetUserSubmissions();

            Assert.IsInstanceOfType(result.Result, typeof(NotFoundObjectResult));
            var notFoundResult = result.Result as NotFoundObjectResult;
            var errorResponse = notFoundResult!.Value as ErrorResponse;
            Assert.AreEqual(404, errorResponse!.ErrorCode);
            Assert.AreEqual("No submissions found.", errorResponse.ErrorMessage);
        }

        [TestMethod]
        public void GetUserSubmissionNames_WhenSubmissionsExist_ReturnsOkResult()
        {
            var submissionNames = new List<string> { "submission1", "submission2" };
            _fileServiceMock.Setup(fs => fs.GetSubmissionNames()).Returns(submissionNames);

            var result = _controller.GetUserSubmissionNames();

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(submissionNames, okResult!.Value);
        }

        [TestMethod]
        public void GetUserSubmissionNames_WhenNoSubmissionsExist_ReturnsNoContentResult()
        {
            var submissionNames = new List<string>();
            _fileServiceMock.Setup(fs => fs.GetSubmissionNames()).Returns(submissionNames);

            var result = _controller.GetUserSubmissionNames();

            Assert.IsInstanceOfType(result.Result, typeof(NoContentResult));
        }
    }
}