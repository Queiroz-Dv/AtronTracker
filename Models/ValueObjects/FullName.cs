namespace Models.ValueObjects
{
    public class FullName
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        public override string ToString() => string.Concat(Name, Surname);
    }
}
