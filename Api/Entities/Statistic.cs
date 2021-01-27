using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Entities
{
    public class Statistic
    {
        public int Id { get; set; }
        public User User { get; set; }
        public Licence Licence { get; set; }
        public int Year { get; set; }
        [Column(TypeName="xml")]
        public string StatisticXml { get; set; }
    }
}