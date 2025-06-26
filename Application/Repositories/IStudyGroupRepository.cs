using Application.Models;

namespace Application.Repositories;

public interface IStudyGroupRepository
{
    Task CreateStudyGroup(StudyGroup studyGroup);

    Task<List<StudyGroup>> GetStudyGroups();

    Task<List<StudyGroup>> SearchStudyGroups(string subject, SortingOrder order);

    Task JoinStudyGroup(int studyGroupId, int userId);

    Task LeaveStudyGroup(int studyGroupId, int userId);

    // Check if the user already created a group for the subject
    Task<bool> UserHasGroupForSubject(int userId, Subject subject);
}
