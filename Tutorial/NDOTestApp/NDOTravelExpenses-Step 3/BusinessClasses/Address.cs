using System;
using NDO;
namespace BusinessClasses
{
    [NDOPersistent]
    public class Address
    {
        private string countryCode;
        private string zip;
        private string street;
        private string city;
        public Address()
        {
        }
        public string CountryCode
        {
            get { return countryCode; }
            set { countryCode = value; }
        }
        public string Zip
        {
            get { return zip; }
            set { zip = value; }
        }
        public string Street
        {
            get { return street; }
            set { street = value; }
        }
        public string City
        {
            get { return city; }
            set { city = value; }
        }
    }
}
