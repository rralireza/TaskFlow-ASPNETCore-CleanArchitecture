namespace TaskFlow.Application.Intefaces.Services.Projects;

public interface IProjectPoliciesService
{
    bool DuplicateTitle(Guid userId, string title);

    bool CanCreateProject(Guid userId, byte role, int maxProjects);
}
