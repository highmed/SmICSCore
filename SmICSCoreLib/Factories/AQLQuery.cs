namespace SmICSCoreLib.Factories
{
    public class AQLQuery
    {
        public string Query { get; set; }
        public string Name { get; set; }

        public AQLQuery()
        {

        }
        public AQLQuery(string name, string query)
        {
            Name = name;
            Query = query;
        }
        
        public override string ToString()
        {
            return Query;
        }
    }
}
