using CineInfo_API.Interfaces;

namespace CineInfo_API.Data.DTOs {
    public class InputAddressDTO : IAddress {
        public string Neighborhood { get; set; }
        public string Street { get; set; }
        public int Number { get; set; }
    }
}
