﻿namespace TWAB.Models
{
    public class FilterDTO
    {
        public string Title {  get; set; }
        public string Category { get; set; }
        public string WorkingDimension { get; set; }
        public string ContractType { get; set; }
        public string City { get; set; }

        public bool IsAnyPropertyFilled()
        {
            return !string.IsNullOrWhiteSpace(Title) ||
                   !string.IsNullOrWhiteSpace(Category) ||
                   !string.IsNullOrWhiteSpace(WorkingDimension) ||
                   !string.IsNullOrWhiteSpace(ContractType) ||
                   !string.IsNullOrWhiteSpace(City);
        }
    }
}
