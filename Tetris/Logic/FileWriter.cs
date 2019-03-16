using System.Xml.Linq;
using Tetris.Logic.Constants;

namespace Tetris.Logic
{
    internal class FileWriter {
        #region Fields
        
        private readonly XDocument document;
        private readonly FileReader reader;
        private readonly string path;

        #endregion Fields

        #region Constructor
        
        public FileWriter() {
            reader = FileReader.GetInstance;
            document = reader.Document;
            path = reader.Path;
        }


        #endregion Constructor
        
        #region Public Methods
        
        public void Write(Record record) {
            var newElement = new XElement(StringConstants.XML_PLAYER_TAG,
                new XAttribute(StringConstants.XML_ATTRIBUTE_ID, reader.GetNumberOfRecords() + 1),
                new XElement(StringConstants.XML_ELEMENT_PLAYER_NAME, record.PlayerPlayerName),
                new XElement(StringConstants.XML_ELEMENT_DATE, record.Date),
                new XElement(StringConstants.XML_ELEMENT_TIME, record.Time),
                new XElement(StringConstants.XML_ELEMENT_SCORE, record.Score),
                new XElement(StringConstants.XML_ELEMENT_ROWS, record.SumOfClearedRows),
                new XElement(StringConstants.XML_ELEMENT_LEVEL, record.ReachedLevel));

            document.Element(StringConstants.XML_ELEMENT_PLAYERS)?.Add(newElement);
            document.Save(path);
        }

        #endregion Public Methods
    }
}
