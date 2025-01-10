namespace Web.API;

public record Person(string FullName);

public class PeopleService
{
    private readonly List<Person> _people = new()
    {
        new Person("John Doe"),
        new Person("Jane Doe"),
        new Person("Julie Doe"),
    };

    public IEnumerable<Person> SearchPeople(string searchTearm)
    {
        return _people.Where(
            p => p.FullName.Contains(
                searchTearm, StringComparison.OrdinalIgnoreCase
        ));
    }
}