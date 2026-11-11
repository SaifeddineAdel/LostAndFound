using LostAndFound.Domain.Exceptions;

namespace LostAndFound.Domain.Entities;

public class Category
{
    private readonly List<SubCategory> _subCategories = [];

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool IsDeleted { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyCollection<SubCategory> SubCategories =>
        _subCategories.AsReadOnly();

    private Category()
    {
    }

    private Category(string name, string description)
    {
        Name = NormalizeName(name);
        Description = description.Trim();
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static Category Create(string name, string description)
    {
        return new Category(name, description);
    }

    public void UpdateDetails(
        string name,
        string description,
        bool restoreAfterUpdate = false)
    {
        Name = NormalizeName(name);
        Description = description.Trim();

        if (IsDeleted && restoreAfterUpdate)
        {
            IsDeleted = false;
        }

        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Delete()
    {
        if (IsDeleted)
        {
            throw new DomainException("Category is already deleted.");
        }

        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Restore()
    {
        if (!IsDeleted)
        {
            throw new DomainException("Category is already active.");
        }

        IsDeleted = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Category name cannot be empty.");
        }

        return name.Trim();
    }
}