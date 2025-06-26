using System;
using System.Collections.Generic;

namespace Assets.Scripts.Objects
{
    [Serializable]
    public class SectionObject
    {
        public int id;
        public string name;
        public string description;
        public string created;
        public string updated;

        public List<UnitObject> units;
    }

    [Serializable]
    public class UnitObject
    {
        public int id;
        public string name;
        public string description;
        public string created;
        public string updated;

        public List<EntryObject> entries;
    }

    [Serializable]
    public class EntryObject
    {
        public int id;
        public string name;
        public string description;
        public string value1;
        public string value2;
        public int levelOnKnowledge;
        public string lastReviewed;
    }
}
