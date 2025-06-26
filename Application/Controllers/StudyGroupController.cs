using Application.Models;
using Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers;

public class StudyGroupController(IStudyGroupRepository studyGroupRepository)
{
    public async Task<IActionResult> CreateStudyGroup(StudyGroup studyGroup)
    {
        await studyGroupRepository.CreateStudyGroup(studyGroup);
        return new OkResult();
    }

    public async Task<IActionResult> GetStudyGroups()
    {
        var studyGroups = await studyGroupRepository.GetStudyGroups();
        return new OkObjectResult(studyGroups);
    }

    public async Task<IActionResult> SearchStudyGroups(string subject, SortingOrder order)
    {
        var studyGroups = await studyGroupRepository.SearchStudyGroups(subject, order);
        return new OkObjectResult(studyGroups);
    }

    public async Task<IActionResult> JoinStudyGroup(int studyGroupId, int userId)
    {
        await studyGroupRepository.JoinStudyGroup(studyGroupId, userId);
        return new OkResult();
    }

    public async Task<IActionResult> LeaveStudyGroup(int studyGroupId, int userId)
    {
        await studyGroupRepository.LeaveStudyGroup(studyGroupId, userId);
        return new OkResult();
    }
}
