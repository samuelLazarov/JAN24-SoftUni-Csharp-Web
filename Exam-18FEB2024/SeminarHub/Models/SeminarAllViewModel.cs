namespace SeminarHub.Models
{
    public class SeminarAllViewModel
    {

        public SeminarAllViewModel(
            int id,
            string topic,
            string lecturer,
            string category,
            DateTime dateAndTime,
            string organizer)
        {
            Id = id;
            Topic = topic;
            Lecturer = lecturer;
            Category = category;
            DateAndTime = dateAndTime.ToString(Data.Common.DataConstants.DateFormat);
            Organizer = organizer;
        }

        public int Id { get; set; }
        public string Topic { get; set; }
        public string Lecturer { get; set; }
        public string Category { get; set; }
        public string DateAndTime { get; set; }
        public string Organizer { get; set; }

    }
}
