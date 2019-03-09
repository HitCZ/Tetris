using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Tetris.Logic.Constants;

namespace Tetris.Logic.Converters
{
    public class DocumentToRecordsConverter
    {
        public List<Record> GetRecordsFromDocument(XDocument document)
        {
            var result = from d in document.Descendants("player")
                select d;
            var records = new List<Record>();

            foreach (var item in result)
            {
                var name = item.Element(StringConstants.XML_ELEMENT_PLAYER_NAME)?.Value;
                var score = Convert.ToInt32(item.Element(StringConstants.XML_ELEMENT_SCORE)?.Value);
                var level = Convert.ToInt32(item.Element(StringConstants.XML_ELEMENT_LEVEL)?.Value);
                var rows = Convert.ToInt32(item.Element(StringConstants.XML_ELEMENT_ROWS)?.Value);
                var record = new Record(name, score, rows, level);

                records.Add(record);
            }

            return records;
        }
    }
}
