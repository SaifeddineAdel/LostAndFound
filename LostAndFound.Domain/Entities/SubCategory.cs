using LostAndFound.Domain.Exceptions;

namespace LostAndFound.Domain.Entities;

public class SubCategory
{
    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public bool IsDeleted { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? UpdatedAt { get; private set; }

    public int CategoryId { get; private set; }

    public Category Category { get; private set; } = null!;

    private SubCategory()
    {
    }

    private SubCategory(int categoryId, string name)
    {
        CategoryId = categoryId;
        Name = NormalizeName(name);
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public static SubCategory Create(int categoryId, string name)
    {
        if (categoryId <= 0)
        {
            throw new DomainException(
                "Category ID must be greater than zero.");
        }

        return new SubCategory(categoryId, name);
    }

    public void UpdateName(
        string name,
        bool restoreAfterUpdate = false)
    {
        Name = NormalizeName(name);

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
            throw new DomainException("SubCategory is already deleted.");
        }

        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Restore()
    {
        if (!IsDeleted)
        {
            throw new DomainException("SubCategory is already active.");
        }

        IsDeleted = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("SubCategory name cannot be empty.");
        }

        return name.Trim();
    }
}