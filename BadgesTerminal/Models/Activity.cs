using System;

namespace UwpCamButton
{
    public class Activity
    {
        public DateTime DateAction { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Activity(string name, string description)
        {
            DateAction = DateTime.Now;
            Name = name;
            Description = description;
        }

        public override string ToString()
        {
            return DateAction.ToString();
        }
    }
}
