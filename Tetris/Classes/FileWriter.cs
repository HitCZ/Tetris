using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tetris.Classes;

namespace Tetris {
    class FileWriter {
        XDocument document;
        FileReader reader;
        private int numberOfRecords;
        private string path;

        public FileWriter() {
            this.reader = FileReader.GetInstance;
            document = this.reader.Document;
            this.path = reader.Path;
        }

        public void Write(Record record) {
            var id = numberOfRecords + 1;
            var newElement = new XElement("player",
                new XAttribute("id", reader.GetNumberOfRecords() + 1),
                new XElement("playername", record.PlayerName),
                new XElement("date", record.Date),
                new XElement("time", record.Time),
                new XElement("score", record.Score),
                new XElement("rows", record.SumOfCLearedRows),
                new XElement("level", record.ReachedLevel));

            document.Element("players").Add(newElement);
            document.Save(path);
        }
    }
}
