using System;
using System.IO;

namespace D2NG
{
    class DataManager
    {
        private static DataManager sm_instance;

        public static DataManager Instance
        {
            get
            {
                if (sm_instance == null)
                {
                    sm_instance = new DataManager();
                }
                return sm_instance;
            }
        }

        public ItemDataType ItemData;
        public PlainTextDataType m_experiences,
                            m_magicalPrefixes,
                            m_magicalSuffixes,
                            m_rarePrefixes,
                            m_rareSuffixes,
                            m_uniqueItems,
                            m_monsterNames,
                            m_monsterFields,
                            m_superUniques,
                            m_itemProperties,
                            m_skills;

        public DataManager(String dataDirectory = "data")
        {
            String[] fileNames =
            {
                "experience.txt",
                "magical_prefixes.txt",
                "magical_suffixes.txt",
                "rare_prefixes.txt",
                "rare_suffixes.txt",
                "unique_items.txt",
                "monster_names.txt",
                "monster_fields.txt",
                "super_uniques.txt",
                "item_properties.txt",
                "skills.txt"
            };

            String itemDataFile = Path.Combine(dataDirectory, "item_data.txt");
            ItemData = new ItemDataType(itemDataFile);
            m_experiences = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[0]));
            m_magicalPrefixes = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[1]));
            m_magicalSuffixes = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[2]));
            m_rarePrefixes = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[3]));
            m_rareSuffixes = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[4]));
            m_uniqueItems = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[5]));
            m_monsterNames = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[6]));
            m_monsterFields = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[7]));
            m_superUniques = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[8]));
            m_itemProperties = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[9]));
            m_skills = new PlainTextDataType(Path.Combine(dataDirectory, fileNames[10]));
        }
    }
}