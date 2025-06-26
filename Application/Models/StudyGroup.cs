namespace Application.Models;

public class StudyGroup(
    int studyGroupId, 
    string name, 
    Subject subject, 
    DateTime createDate, 
    List<User> users)
{
    public int StudyGroupId { get; } = studyGroupId;

    public string Name { get; } = name;

    public Subject Subject { get; } = subject;

    public DateTime CreateDate { get; } = createDate;

    public List<User> Users { get; private set; } = users;

    public void AddUser(User user)
    {
        Users.Add(user);
    }

    public void RemoveUser(User user)
    {
        Users.Remove(user);
    }
}
