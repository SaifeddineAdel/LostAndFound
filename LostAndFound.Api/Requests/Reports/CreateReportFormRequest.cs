
using LostAndFound.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LostAndFound.Api.Requests.Reports;

public class CreateReportFormRequest
{
    [Required]
    public ReportType Type { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? PrivateDescription { get; set; }

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    [Range(1, int.MaxValue)]
    public int SubCategoryId { get; set; }

    [Range(1, int.MaxValue)]
    public int LocationId { get; set; }

    public DateTimeOffset ItemDate { get; set; }

    public List<IFormFile> Images { get; set; } = [];
}
