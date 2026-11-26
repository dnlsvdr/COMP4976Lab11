using AI4SchoolAgent.Data;
using AI4SchoolAgent.Models;
using Microsoft.EntityFrameworkCore;

namespace AI4SchoolAgent.Services;

public class StudentsAgentTools
{
    private readonly IDbContextFactory<SchoolContext> _factory;

    public StudentsAgentTools(
        IDbContextFactory<SchoolContext> factory)
    {
        _factory = factory;
    }

    public async Task<List<Student>> FindStudentsByAnyFilterAsync(
        string? name = null,
        string? firstName = null,
        string? lastName = null,
        string? school = null,
        int? birthYear = null,
        string matchMode = "exact")
    {
        if (string.IsNullOrWhiteSpace(name) &&
            string.IsNullOrWhiteSpace(firstName) &&
            string.IsNullOrWhiteSpace(lastName) &&
            string.IsNullOrWhiteSpace(school) &&
            birthYear == null)
        {
            throw new ArgumentException(
                "Provide at least one filter.");
        }

        if (matchMode != "exact" && matchMode != "contains")
        {
            throw new ArgumentException(
                "Match mode must be exact or contains.");
        }

        await using var db = await _factory.CreateDbContextAsync();

        var students = await db.Students
            .AsNoTracking()
            .ToListAsync();

        bool Matches(string value, string filter)
        {
            filter = filter.Trim();

            return matchMode == "contains"
                ? value.Contains(
                    filter,
                    StringComparison.OrdinalIgnoreCase)
                : value.Equals(
                    filter,
                    StringComparison.OrdinalIgnoreCase);
        }

        return students.Where(s =>
            (string.IsNullOrWhiteSpace(name) ||
                Matches(s.FirstName, name) ||
                Matches(s.LastName, name)) &&

            (string.IsNullOrWhiteSpace(firstName) ||
                Matches(s.FirstName, firstName)) &&

            (string.IsNullOrWhiteSpace(lastName) ||
                Matches(s.LastName, lastName)) &&

            (string.IsNullOrWhiteSpace(school) ||
                Matches(s.School, school)) &&

            (birthYear == null ||
                s.DateOfBirth.Year == birthYear)
        ).ToList();
    }
}