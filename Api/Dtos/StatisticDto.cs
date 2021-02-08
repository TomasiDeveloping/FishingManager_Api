using System.Collections.Generic;

namespace Api.Dtos
{
    public class StatisticDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public int Year { get; set; }
        public int LicenceId { get; set; }
        public string LicenceName { get; set; }
        public CatchStatistic Statistic { get; set; }
    }

    public class CatchStatistic
    {
        public string FishingClub { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Year { get; set; }
        public ICollection<Months> Months { get; set; }
    }

    public class Months
    {
        public string Month { get; set; }
        public ICollection<Days> Days { get; set; }
    }

    public class Days
    {
        public string Day { get; set; }
        public string Hour { get; set; }
        public ICollection<FishCatch> FishCatches { get; set; }
    }

    public class FishCatch
    {
        public string Fish { get; set; }
        public string Number { get; set; }
    }
}