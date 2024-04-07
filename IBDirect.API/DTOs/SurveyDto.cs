namespace IBDirect.API.DTOs
{
    public class SurveyDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public int? Q1 { get; set; }
        public int? Q2 { get; set; }
        public int? Q3 { get; set; }
        public int? Q4 { get; set; }
        public bool Q4a { get; set; }
        public int? Q5 { get; set; }
        public int? Q6 { get; set; }
        public int? Q7 { get; set; }
        public int? Q8 { get; set; }
        public int? Q9 { get; set; }
        public int? Q10 { get; set; }
        public int? Q11 { get; set; }
        public int? Q12 { get; set; }
        public int? ContScore { get; set; }
        public int? Q13 { get; set; }
        public bool Completed { get; set; }
    }
}
