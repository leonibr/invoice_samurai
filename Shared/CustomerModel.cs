using System;

namespace InvoiceSamurai.Shared
{
    public record CustomerModel
    {
        public string Name { get; set; } = "Johnny Little Doe";
        public string Address { get; set; } = "123, Anywhere Street";
        public string Neigborhood { get; set; } = "Downtown";
        public string City { get; set; } = "Tampa";
        public string State { get; set; } = "FL";

        public DateTime Dob { get; set; } = new DateTime(1992, 2, 3);
    }
}
