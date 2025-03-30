using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPickerApp.Models
{
    public class ClassList
    {
        public string ClassName { get; set; }
        public List<Student> Students { get; set; } = new();
    }
}
