using Application.Controllers;
using Application.Models;
using Application.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestAutomation.Tests;

[TestFixture]
public class ComponentTests
{
    private Mock<IStudyGroupRepository> _repoMock;
    private StudyGroupController _controller;

    [SetUp]
    public void SetUp()
    {
        _repoMock = new Mock<IStudyGroupRepository>();
        _controller = new StudyGroupController(_repoMock.Object);
    }

    [Test]
    [Category("Regression")]
    [Category("Smoke")]
    public async Task CreateStudyGroup_ValidGroup_ReturnsOk()
    {
        // Arrange
        var group = new StudyGroup(0, "Physics Lovers", Subject.Physics, DateTime.UtcNow, [new User()]);

        // Act
        var result = await _controller.CreateStudyGroup(group);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }

    [Test]
    [Category("Regression")]
    [Category("Smoke")]
    public async Task GetStudyGroups_ReturnsList()
    {
        // Arrange
        _repoMock.Setup(r => r.GetStudyGroups())
            .ReturnsAsync([new StudyGroup(1, "Chem Group", Subject.Chemistry, DateTime.Now, [new User()])]);

        // Act
        var result = await _controller.GetStudyGroups() as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        var list = result.Value as List<StudyGroup>;
        Assert.That(list, Has.Count.EqualTo(1));
    }

    [Test]
    [Category("Regression")]
    [Category("Smoke")]
    public async Task SearchStudyGroups_ReturnsFiltered()
    {
        // Arrange
        _repoMock.Setup(r => r.SearchStudyGroups("Math", SortingOrder.Descending))
            .ReturnsAsync([new StudyGroup(2, "Math Lab", Subject.Math, DateTime.Now, [new User()])]);

        // Act
        var result = await _controller.SearchStudyGroups("Math", SortingOrder.Descending) as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        var list = result.Value as List<StudyGroup>;
        Assert.That(list, Has.Count.AtLeast(1));
        Assert.That(list.All(g => g.Name.Contains("Math")), Is.True);
    }

    [Test]
    public async Task SearchStudyGroups_ReturnsEmptyList()
    {
        // Arrange
        _repoMock.Setup(r => r.SearchStudyGroups("Math", SortingOrder.Descending))
            .ReturnsAsync([]);

        // Act
        var result = await _controller.SearchStudyGroups("Math", SortingOrder.Descending) as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        var list = result.Value as List<StudyGroup>;
        Assert.That(list, Has.Count.EqualTo(0));
    }

    [Test]
    [Category("Regression")]
    [Category("Smoke")]
    public async Task JoinStudyGroup_ValidUser_ReturnsOk()
    {
        // Act
        var result = await _controller.JoinStudyGroup(1, 101);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }

    [Test]
    [Category("Regression")]
    public async Task Join_MultipleStudyGroups_ReturnsOk()
    {
        // Arrange
        var userId = 42;
        var studyGroupIds = new[] { 1, 2, 3 };
        var joinCalls = 0;

        _repoMock.Setup(r => r.JoinStudyGroup(It.IsAny<int>(), It.IsAny<int>()))
                 .Callback(() => joinCalls++)
                 .Returns(Task.CompletedTask);

        // Act
        foreach (var groupId in studyGroupIds)
        {
            var result = await _controller.JoinStudyGroup(groupId, userId);
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        // Assert
        Assert.That(joinCalls, Is.EqualTo(studyGroupIds.Length), "JoinStudyGroup should be called once per group");
        _repoMock.Verify(r => r.JoinStudyGroup(It.IsAny<int>(), userId), Times.Exactly(studyGroupIds.Length));
    }

    [Test]
    [Category("Regression")]
    [Category("Smoke")]
    public async Task LeaveStudyGroup_ValidUser_ReturnsOk()
    {
        // Act
        var result = await _controller.LeaveStudyGroup(1, 101);

        // Assert
        Assert.That(result, Is.InstanceOf<OkResult>());
    }

    [Test]
    [Category("Regression")]
    public async Task CreateStudyGroup_DuplicateSubject_ReturnsBadRequest()
    {
        // Arrange
        _repoMock.Setup(r => r.UserHasGroupForSubject(It.IsAny<int>(), It.IsAny<Subject>()))
            .ReturnsAsync(true);
        var group = new StudyGroup(0, "Math Club", Subject.Math, DateTime.UtcNow, [new User()]);

        // Act
        var result = await _controller.CreateStudyGroup(group);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestResult>());
    }

    [TestCase(SortingOrder.Ascending)]
    [TestCase(SortingOrder.Descending)]
    [Category("Regression")]
    public async Task SearchStudyGroups_ReturnsSortedGroups(SortingOrder order)
    {
        // Arrange
        var groups = order switch
        {
            SortingOrder.Ascending => GetSampleGroups().OrderBy(g => g.CreateDate).ToList(),
            SortingOrder.Descending => GetSampleGroups().OrderByDescending(g => g.CreateDate).ToList(),
            _ => throw new ArgumentException("Unknown sort order")
        };

        _repoMock.Setup(r => r.SearchStudyGroups("Math", order)).ReturnsAsync(groups);

        // Act
        var result = await _controller.SearchStudyGroups("Math", order);

        // Assert
        var okResult = result as OkObjectResult;
        var returnedGroups = okResult?.Value as List<StudyGroup>;
        Assert.That(returnedGroups, Is.Not.Null);
        Assert.That(IsSortedByDate(returnedGroups, order), Is.True);
    }

    private static List<StudyGroup> GetSampleGroups()
        => [
                new StudyGroup(1, "Math Club", Subject.Math, new DateTime(2025, 1, 1), [new User()]),
                new StudyGroup(2, "Physics Friends", Subject.Physics, new DateTime(2024, 5, 1), [new User()]),
                new StudyGroup(3, "Chemistry Crew", Subject.Chemistry, new DateTime(2023, 10, 15), [new User()])
            ];

    private static bool IsSortedByDate(List<StudyGroup> groups, SortingOrder order)
    {
        var sorted = order switch
        {
            SortingOrder.Ascending => groups.OrderBy(g => g.CreateDate).ToList(),
            SortingOrder.Descending => groups.OrderByDescending(g => g.CreateDate).ToList(),
            _ => throw new ArgumentException("Unknown sort order")
        };

        return groups.SequenceEqual(sorted);
    }
}
