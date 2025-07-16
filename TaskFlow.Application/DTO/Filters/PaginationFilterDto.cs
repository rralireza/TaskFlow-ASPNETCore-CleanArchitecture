namespace TaskFlow.Application.DTO.Filters;

public abstract class PaginationFilterDto
{
    public string? Title { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
