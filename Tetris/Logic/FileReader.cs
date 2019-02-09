using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tetris.Classes {
    class FileReader {
        private static FileReader Instance = null;
        private string path = "score.xml";
        private XDocument document;

        private FileReader() {
            LoadFile();
        }

        public XDocument Document
        {
            get { return this.document; }
        }

        public static FileReader GetInstance
        {
            get
            {
                if (Instance == null) {
                    Instance = new FileReader();
                }
                return Instance;
            }
        }

        public string Path
        {
            get { return this.path; }
        }

        public int GetNumberOfRecords() {
            this.LoadFile();
            return this.document.Descendants("player").Count();
        }

        private void LoadFile() {
            try {
                document = XDocument.Load(path);
            } catch (FileNotFoundException e) {
                this.CreateFile();
            }
        }

        private void CreateFile() {
            document = new XDocument(
                    new XDeclaration("1.0", "utf-8", "true"),
                    new XElement("players"));
            document.Save(path);
            Console.WriteLine("File created");
        }
    }
}
