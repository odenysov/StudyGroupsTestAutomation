using Application.Models;

namespace TestAutomation.Tests
{
    public class UnitTests
    {
        [TestCase("Valid")]// 5 chars
        [TestCase("LongName30LongName30LongName30")]// 30 chars
        [Category("Regression")]
        public void StudyGroup_WithValidName_DoesNotThrowException(string name)
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var group = new StudyGroup(1, name, Subject.Math, DateTime.UtcNow, [new User()]);
            });
        }

        [TestCase("test")]// 4 chars
        [TestCase("LongName31LongName31LongName31L")]// 31 chars
        public void StudyGroup_WithInValidName_ThrowsException(string name)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
            new StudyGroup(1, name, Subject.Math, DateTime.Now, [new User()]));
            Assert.That(exception.Message, Does.Contain("between 5 and 30"));
        }

        [TestCase(Subject.Math)]
        [TestCase(Subject.Physics)]
        [TestCase(Subject.Chemistry)]
        [Category("Regression")]
        public void StudyGroup_WithValidSubject_DoesNotThrowException(Subject subject)
        {
            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                var group = new StudyGroup(1, "Valid Group", subject, DateTime.UtcNow, [new User()]);
            });
        }

        [Test]
        public void StudyGroup_WithInvalidSubject_ThrowsArgumentException()
        {
            // Arrange
            var invalidSubject = (Subject)999;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                var group = new StudyGroup(1, "Invalid Group", invalidSubject, DateTime.UtcNow, [new User()]);
            });
        }

        [Test]
        [Category("Regression")]
        public void StudyGroup_CreationDate_IsSetCorrectly()
        {
            // Arrange
            var now = DateTime.UtcNow;

            // Act
            var group = new StudyGroup(1, "Physics Squad", Subject.Physics, now, [new User()]);

            // Assert
            Assert.That(group.CreateDate, Is.EqualTo(now));
            Assert.That(group.CreateDate, Is.Not.EqualTo(default(DateTime)));
        }
    }
}