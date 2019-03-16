using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Tetris.Logic.Constants;

namespace Tetris.Logic
{
    internal class FileReader
    {

        #region Fields

        private static FileReader instance;

        #endregion Fields

        #region Constructor

        private FileReader()
        {
            LoadFile();
        }

        #endregion Constructor

        #region Properties

        public XDocument Document { get; private set; }

        public static FileReader GetInstance => instance ?? (instance = new FileReader());

        public string Path { get; } = "score.xaml";

        #endregion Properties

        #region Public Methods

        public int? GetNumberOfRecords()
        {
            LoadFile();
            var countOfDescendants = Document?.Descendants(StringConstants.XML_PLAYER_TAG).Count();

            if (countOfDescendants is null)
                throw new ArgumentNullException($"{nameof(Document)} is null.");

            return countOfDescendants;
        }

        #endregion Public Methods

        #region Private Methods
        
        private void LoadFile()
        {
            try
            {
                Document = XDocument.Load(Path);
            }
            catch (FileNotFoundException)
            {
                CreateFile();
            }
        }

        private void CreateFile()
        {
            Document = new XDocument(
                    new XDeclaration(
                        StringConstants.XML_VERSION, 
                        StringConstants.XML_ENCODING, 
                        StringConstants.XML_STANDALONE_TRUE),
                    new XElement(StringConstants.XML_ELEMENT_PLAYERS));
            Document.Save(Path);
        }

        #endregion Private Methods
    }
}
