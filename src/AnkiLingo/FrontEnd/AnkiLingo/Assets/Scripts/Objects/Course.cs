using Assets.Scripts.Objects;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
   [Serializable]
    public class CourseObject
    {
        public int id;
        public string created;
        public string updated;
        public string name;
        public string description;
        public string icon;

        public List<SectionObject> sections;
    }
}
