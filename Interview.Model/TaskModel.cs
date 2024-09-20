using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Interview.Model
{
    public class TaskModel
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public DateTime StartTime { get; set; }
        public int Status { get; set; }
        public virtual ProfileModel ProfileModel { get; set; }
    }
}
