namespace MediatR.Commands
{
    public class OpenApiDetails
    {
        public OpenApiDetails()
        {
        }

        public OpenApiDetails(string groupName)
        {
            this.GroupName = groupName;
        }

        public OpenApiDetails(string groupName, string description)
        {
            this.GroupName = groupName;
            this.Description = description;
        }

        public string Description { get; set; }

        public string RequestBodyDescription { get; set; }

        public string ResponseDescription { get; set; }

        public string Summary { get; set; }

        public string Produces { get; set; } = "application/json";

        public string GroupPrefix { get; set; } = string.Empty;

        public string GroupName { get; set; }
    }
}
