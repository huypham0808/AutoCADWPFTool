using System.Windows.Input;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.ApplicationServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.ComponentModel;
using System.IO;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Windows.Forms;
using Clipboard = System.Windows.Forms.Clipboard;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Text;
using ChangeFileName.Utilities;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using Autodesk.AutoCAD.Geometry;
using UtilitiesTool_V4.Models;
namespace ChangeFileName.ViewModels
{
    public class ChangeFileNameViewModel : INotifyPropertyChanged
    {
        public ICommand ChangeFileNameCommand { get; }
        public ICommand ConverTextCommand { get; }
        public ICommand CopyTextCommand { get; }
        public ICommand GetCurrentLocalFileDwgCommand { get; }
        public ICommand GetSDriveProjectFolderCommand { get; }
        public ICommand PushFileToSDriveCommand { get; }
        public ICommand SaveDataToTextFileCommand { get; }
        public ICommand CreateTextStyleCommand { get; }
        public ICommand CreateDimStyleCommand { get; }
        public ICommand CreateMultileaderCommand { get; }
        public ICommand CreatAllCommands { get; }
        public ICommand ExtractBlockCommand { get; }
        public ICommand InsertBlockCommand { get; }
        public ICommand CountBlockCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Define Properties
        private string _NewFileName;
        private string _revisionNumber;
        private string _inputText;
        private string _resultText;

        private string _contentCopyButton;
        private string _contentBeforeCopy = "Copy";
        private string _contentAfterCopy = "Copied";
        private Brush _colorTextOriginal;
        private string _copyTextClipboard;

        private string _currentLocalFile;
        private string _sdriveCompany;
        private ObservableCollection<string> _filePathsToListView = new ObservableCollection<string>();
        private bool _isExpandedHistoryPath;
        private readonly string directoryHistoryFolder = @"C:\FRP-ST-SST-Plugin\Data\Data Project Path";
        private readonly string historyDataFileName = "\\History project folder directory.xml";
        private const int originalNumber = 16;
        #region Properties Define
        public string ContentCopyButton
        {
            get { return _contentCopyButton; }
            set
            {
                _contentCopyButton = value;
                OnPropertyChanged(nameof(ContentCopyButton));
            }
        }
        public string ContentBeforeCopy
        {
            get { return _contentBeforeCopy; }
            set
            {
                _contentBeforeCopy = value;
                OnPropertyChanged(nameof(ContentBeforeCopy));
            }
        }
        public string ContentAfterCopy
        {
            get { return _contentAfterCopy; }
            set
            {
                _contentAfterCopy = value;
                OnPropertyChanged(nameof(ContentAfterCopy));
            }
        }
        public string CopyTextClipboard
        {
            get { return _copyTextClipboard; }
            set
            {
                _copyTextClipboard = value;
                OnPropertyChanged(nameof(CopyTextClipboard));
            }
        }
        public Brush ColorTextOriginal
        {
            get { return _colorTextOriginal; }
            set
            {
                if (ResultText == "Text will be converted to UPPER here!" || StatusCreateTextStyle == TextStyleFail || StatusCreateDimStyle == DimStyleFail)
                {
                    _colorTextOriginal = Brushes.Blue;
                }
                else
                {
                    _colorTextOriginal = Brushes.Pink;
                    OnPropertyChanged(nameof(ColorTextOriginal));
                }
;
            }
        }

        public bool IsButtonEnabled => !string.IsNullOrEmpty(InputText);
        public bool IsExpandedHistoryPath
        {
            get { return _isExpandedHistoryPath; }
            set
            {
                _isExpandedHistoryPath = value;
                OnPropertyChanged(nameof(IsExpandedHistoryPath));

            }
        }
        public string NewFileName
        {
            get { return _NewFileName; }
            set
            {
                if (_NewFileName != value)
                {
                    _NewFileName = value;
                    OnPropertyChanged(nameof(NewFileName));
                }

            }
        }
        public string RevisionNumber
        {
            get { return _revisionNumber; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _revisionNumber = string.Empty;
                }
                else
                {
                    if (value.All(char.IsDigit))
                    {
                        _revisionNumber = value;
                    }
                    else
                    {
                        _revisionNumber = string.Empty;
                    }
                }
                OnPropertyChanged(nameof(RevisionNumber));
            }
        }
        public string InputText
        {
            get { return _inputText; }
            set
            {
                _inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }
        public string ResultText
        {
            get { return _resultText; }
            set
            {
                _resultText = value;
                OnPropertyChanged(nameof(ResultText));
            }
        }
        public string CurrentLocalFilePath
        {
            get { return _currentLocalFile; }
            set
            {
                _currentLocalFile = value;
                OnPropertyChanged(nameof(CurrentLocalFilePath));
            }
        }
        public string SDriveCompanyPath
        {
            get { return _sdriveCompany; }
            set
            {
                _sdriveCompany = value;
                OnPropertyChanged(nameof(SDriveCompanyPath));
            }
        }
        public ObservableCollection<string> FilePathToListView
        {
            get { return _filePathsToListView; }
            set
            {
                _filePathsToListView = value;
                OnPropertyChanged(nameof(FilePathToListView));
            }
        }
        private string _textStyleName;

        public string TextStyleName
        {
            get { return _textStyleName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _textStyleName = string.Empty;
                }
                else
                {
                    if (value.All(char.IsDigit))
                    {
                        _textStyleName = value;
                    }
                    else
                    {
                        _textStyleName = string.Empty;
                    }
                }
                OnPropertyChanged(nameof(TextStyleName));
            }
        }
        private string _statusCreateTextStyle;

        public string StatusCreateTextStyle
        {
            get { return _statusCreateTextStyle; }
            set
            {
                _statusCreateTextStyle = value;
                OnPropertyChanged(nameof(StatusCreateTextStyle));
            }
        }
        private string _statusCreateDimStyle;

        public string StatusCreateDimStyle
        {
            get { return _statusCreateDimStyle; }
            set { _statusCreateDimStyle = value; OnPropertyChanged(nameof(StatusCreateDimStyle)); }
        }
        private string _statusCreateScaleVP;

        public string StatusCreateScaleVP
        {
            get { return _statusCreateScaleVP; }
            set { _statusCreateScaleVP = value; OnPropertyChanged(nameof(StatusCreateScaleVP)); }
        }


        private string _textStyleDone = "New Text style was created successfully!";
        private string _dimStyleDone = "New Dimension style was created successfully!";
        private string _multileaderStyleDone = "New Multileader style was created successfully!";
        private string _scaleViewPortDone = "New Scale for viewport was created successfully!";

        public string ScaleViewPortDone
        {
            get { return _scaleViewPortDone; }
            set { _scaleViewPortDone = value; OnPropertyChanged(nameof(ScaleViewPortDone)); }
        }

        public string TextStyleDone
        {
            get { return _textStyleDone; }
            set { _textStyleDone = value; OnPropertyChanged(nameof(TextStyleDone)); }
        }
        public string DimStyleDone
        {
            get { return _dimStyleDone; }
            set { _dimStyleDone = value; OnPropertyChanged(nameof(DimStyleDone)); }
        }
        public string MultileaderStyleDone
        {
            get { return _multileaderStyleDone; }
            set { _multileaderStyleDone = value; OnPropertyChanged(nameof(MultileaderStyleDone)); }
        }
        private string _textStyleFail = "Text style wasn't created / existing!";
        private string _dimStyleFail = "Dimension style wasn't created / existing!!";
        private string _multileaderFail = "Multileader style wasn't created / existing!!";
        private string _scaleViewPortFail = "Scale viewport wasn't created / existing!!";

        public string ScaleViewPortFail
        {
            get { return _scaleViewPortFail; }
            set { _scaleViewPortFail = value; OnPropertyChanged(nameof(ScaleViewPortFail)); }
        }

        public string TextStyleFail
        {
            get { return _textStyleFail; }
            set { _textStyleFail = value; OnPropertyChanged(nameof(TextStyleFail)); }
        }


        public string DimStyleFail
        {
            get { return _dimStyleFail; }
            set { _dimStyleFail = value; OnPropertyChanged(nameof(DimStyleFail)); }
        }


        public string MultileaderFail
        {
            get { return _multileaderFail; }
            set { _multileaderFail = value; OnPropertyChanged(nameof(MultileaderFail)); }
        }
        private string _blockName;

        public string BlockName
        {
            get { return _blockName; }
            set { _blockName = value; OnPropertyChanged(nameof(BlockName)); }
        }
        //Load list to listview
        private string _blockNameTest;
        public string BlockNameTest
        {
            get { return _blockNameTest; }
            set { _blockNameTest = value; OnPropertyChanged(nameof(BlockNameTest)); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set 
            {   _isSelected = value; 
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        private ObservableCollection<ChangeFileNameViewModel> _listDetailItemTest { get; set; }
        public ObservableCollection<ChangeFileNameViewModel> ListDetailItemTest
        {
            get { return _listDetailItemTest; }
            set
            {
                _listDetailItemTest = value;
                OnPropertyChanged(nameof(ListDetailItemTest));
            }
        }
        public ChangeFileNameViewModel(string blockNameTest)
        {
            BlockNameTest = blockNameTest;
            IsSelected = false; // Default state
        }

        private ObservableCollection<string> _detailItem;

        public ObservableCollection<string> ListDetailItem
        {
            get { return _detailItem; }
            set { _detailItem = value; OnPropertyChanged(nameof(ListDetailItem)); }
        }

        private string _totalNumberOfBlock;

        public string TotalNumberOfBlock
        {
            get { return _totalNumberOfBlock; }
            set { _totalNumberOfBlock = value; OnPropertyChanged(nameof(TotalNumberOfBlock)); }
        }
        private string _blockNameStartWith;

        public string BlockNameStartWith
        {
            get { return _blockNameStartWith; }
            set { _blockNameStartWith = value; OnPropertyChanged(nameof(BlockNameStartWith)); }
        }

        #endregion
        //Constructor
        public ChangeFileNameViewModel()
        {
            RevisionNumber = "0";
            ContentCopyButton = ContentBeforeCopy;
            ResultText = "Text will be converted to UPPER here!";
            ColorTextOriginal = Brushes.Blue;
            ChangeFileNameCommand = new RelayCommand(ChangeShopDrawingFile);
            ConverTextCommand = new RelayCommand(ConvertToUpper);
            CopyTextCommand = new RelayCommand(CopyText);
            GetCurrentLocalFileDwgCommand = new RelayCommand(GetCurrentLocalFilePath);
            GetSDriveProjectFolderCommand = new RelayCommand(GetSDriveProjectPath);
            PushFileToSDriveCommand = new RelayCommand(PushFileToSDrive);
            IsExpandedHistoryPath = false;
            FilePathToListView = new ObservableCollection<string>();
            SaveDataToTextFileCommand = new RelayCommand(SaveListViewDataToXml);
            CreateTextStyleCommand = new RelayCommand(CreateNewTextStyle);
            CreateDimStyleCommand = new RelayCommand(CreateNewDimStyle);
            CreateMultileaderCommand = new RelayCommand(CreateNewMultileader);
            LoadDataFromXmlFile();
            StatusCreateTextStyle = "None!";
            StatusCreateDimStyle = "None!";
            StatusCreateScaleVP = "None!";
            CreatAllCommands = new RelayCommand(CreateAlls);
            CreateMultileaderCommand = new RelayCommand(CreateNewMultileader);
            ExtractBlockCommand = new RelayCommand(ExtractBlockToFile);
            //InsertBlockCommand = new RelayCommand(InsertABlockName);
            CountBlockCommand = new RelayCommand(CountBlock);
        }

        //Method
        private void ChangeShopDrawingFile()
        {
            Document doc = UtilMethod.AcadDoc();

            string activeDate = DateTime.Now.ToString("yyyy.MM.dd");
            string projectName = string.IsNullOrEmpty(NewFileName) ? "Project name" : NewFileName;
            string newFileName = activeDate + "-" + projectName + "-" + "-" + "Rev." + RevisionNumber + ".dwg";
            try
            {
                // Get the current document name
                string currentFileName = doc.Name;
                // Generate the new file path
                string newFilePath = currentFileName.Replace(Path.GetFileName(currentFileName), newFileName);
                // Rename the file
                doc.Database.SaveAs(newFilePath, true, DwgVersion.Current, doc.Database.SecurityParameters);
                File.Delete(currentFileName);
                MessageBox.Show("Rename file successfully!", "AutoCAD", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                return;
            }
        }
        private void ConvertToUpper()
        {
            ColorTextOriginal = Brushes.Gray;
            if (!string.IsNullOrEmpty(InputText))
            {
                ResultText = InputText.ToUpper();
                ContentCopyButton = ContentBeforeCopy;
            }
            else
            {
                MessageBox.Show("Please input text before to covert!", "AutoCAD Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }
        private void CopyText()
        {
            if (ResultText == "Text will be converted to UPPER here!")
            {
                MessageBox.Show("Please input text to convert!", "AutoCAD", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Clipboard.SetText(ResultText);
            ContentCopyButton = ContentAfterCopy;
            ColorTextOriginal = Brushes.LightGreen;

        }
        private void GetCurrentLocalFilePath()
        {
            string dwgFileName = (string)Application.GetSystemVariable("DWGNAME");
            string dwgPath = (string)Application.GetSystemVariable("DWGPREFIX");
            CurrentLocalFilePath = dwgPath + dwgFileName;
        }
        private void GetSDriveProjectPath()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select project folder",
                CheckFileExists = false,
                FileName = "Select Folder",

                ValidateNames = false,
                CheckPathExists = true,
                Filter = "Folders|no.files"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string folderSDrivePath = Path.GetDirectoryName(openFileDialog.FileName);
                SDriveCompanyPath = folderSDrivePath;

                if (FilePathToListView.Count < 3)
                {
                    FilePathToListView.Add(SDriveCompanyPath);
                }
                else
                {
                    FilePathToListView.RemoveAt(0);
                    FilePathToListView.Add(SDriveCompanyPath);
                }
            }
            else
            {
                MessageBox.Show("No folder selected!", "AutoCAD", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void PushFileToSDrive()
        {
            if (string.IsNullOrEmpty(CurrentLocalFilePath) || string.IsNullOrEmpty(SDriveCompanyPath))
            {
                UtilMethod.WarningMessageBox(@"Current local drive or S:\ ...drive NOT allow empty", "AutoCAD");
                return;
            }
            string sourceDir = CurrentLocalFilePath;
            string sourceDirPDF = sourceDir;
            string pdfDrawingDir = Path.ChangeExtension(sourceDirPDF, ".pdf");
            string fileName = Path.GetFileName(sourceDir);
            string pdfFileName = Path.GetFileName(pdfDrawingDir);
            string sDriveFolderPathDwg = SDriveCompanyPath + "\\" + fileName;
            string sDriveFolderPathPdf = SDriveCompanyPath + "\\" + pdfFileName;

            try
            {
                if (File.Exists(sDriveFolderPathDwg))
                {
                    File.Copy(sourceDir, sDriveFolderPathDwg, true);
                    File.Copy(pdfDrawingDir, sDriveFolderPathPdf, true);
                    UtilMethod.WarningMessageBox("Push file successfully!", "AutoCAD");
                }
                else if (!File.Exists(pdfDrawingDir))
                {
                    UtilMethod.WarningMessageBox("Print PDF before push file!", "AutoCAD");
                    return;
                }
                else
                {
                    File.Copy(sourceDir, sDriveFolderPathDwg);
                    File.Copy(pdfDrawingDir, sDriveFolderPathPdf);
                    UtilMethod.WarningMessageBox("Push file successfully!", "AutoCAD");
                }
            }
            catch
            {
                UtilMethod.WarningMessageBox("File is opening by someone", "AutoCAD");
                return;
            }
            IsExpandedHistoryPath = true;
        }
        private void LoadDataFromXmlFile()
        {
            string xmlFilePath = directoryHistoryFolder + historyDataFileName;
            try
            {
                using (StreamReader sr = new StreamReader(xmlFilePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        FilePathToListView.Add(line);
                    }
                }
            }
            catch
            {
                return;
            }
        }
        //Not in use
        private void SaveListViewDataToXml()
        {
            if (Directory.Exists(directoryHistoryFolder))
            {
                string xmlFilePath = directoryHistoryFolder + historyDataFileName;
                using (StreamWriter writer = new StreamWriter(xmlFilePath))
                {
                    foreach (var item in FilePathToListView)
                    {
                        writer.WriteLine(item);
                    }
                }
                return;
                //UtilMethod.WarningMessageBox("Saved current folder successfully!", "AutoCAD");
            }
        }
        private void CreateNewTextStyle()
        {
            Document doc = UtilMethod.AcadDoc();
            Database db = UtilMethod.AcadDb();

            try
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    doc.LockDocument();
                    SymbolTable st = (SymbolTable)trans.GetObject(db.TextStyleTableId, OpenMode.ForRead);

                    //Check text style isExisting
                    if (st.Has(TextStyleName))
                    {
                        //UtilMethod.WarningMessageBox($"Text style {TextStyleName} is existing already!", "AutoCAD");
                        StatusCreateTextStyle = TextStyleFail;
                        return;
                    }
                    ObjectId style16ID = st["16"];
                    TextStyleTableRecord style16 = (TextStyleTableRecord)trans.GetObject(style16ID, OpenMode.ForRead);

                    //Create new text style base on style 16
                    TextStyleTableRecord newStyle = new TextStyleTableRecord();
                    newStyle.Name = TextStyleName;

                    int textStyleNum = Convert.ToInt32(newStyle.Name);

                    //Clone properties of Style 16
                    newStyle.FileName = style16.FileName;
                    newStyle.BigFontFileName = style16.BigFontFileName;
                    newStyle.FlagBits = style16.FlagBits;
                    newStyle.ObliquingAngle = style16.ObliquingAngle;
                    newStyle.TextSize = 1 * ((double)textStyleNum / originalNumber);

                    //Add new text style to drawing
                    st.UpgradeOpen();
                    ObjectId newTextStyleID = st.Add(newStyle);
                    trans.AddNewlyCreatedDBObject(newStyle, true);
                    trans.Commit();
                    //UtilMethod.WarningMessageBox($"Text style {TextStyleName} was created successfully!", "AutoCAD");
                    StatusCreateTextStyle = TextStyleDone;
                    db.Textstyle = newStyle.ObjectId;
                }
            }
            catch (Exception ex)
            {
                //UtilMethod.WarningMessageBox($"An error occurs: {ex.Message}","Error");
                StatusCreateTextStyle = TextStyleFail;
                return;
            }
        }
        private void CreateNewDimStyle()
        {
            Document doc = UtilMethod.AcadDoc();
            Database db = UtilMethod.AcadDb();

            try
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {
                    doc.LockDocument();
                    // Open the DimStyle table for read
                    DimStyleTable dst = (DimStyleTable)trans.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                    string strDimStyleName = "Scale " + TextStyleName;

                    DimStyleTableRecord acDimStyleTblRec;
                    ObjectId dimStyleID16 = dst["Scale 16"];
                    DimStyleTableRecord dimStyle16 = (DimStyleTableRecord)trans.GetObject(dimStyleID16, OpenMode.ForRead);

                    // Check to see if the dimension style exists or not
                    if (dst.Has(strDimStyleName) == false)
                    {
                        if (dst.IsWriteEnabled == false) trans.GetObject(db.DimStyleTableId, OpenMode.ForWrite);

                        acDimStyleTblRec = new DimStyleTableRecord();
                        acDimStyleTblRec.Name = strDimStyleName;

                        dst.Add(acDimStyleTblRec);
                        trans.AddNewlyCreatedDBObject(acDimStyleTblRec, true);
                    }
                    else
                    {
                        //acDimStyleTblRec = trans.GetObject(dst[strDimStyleName], OpenMode.ForWrite) as DimStyleTableRecord;
                        StatusCreateDimStyle = DimStyleFail;
                        return;
                    }
                    acDimStyleTblRec.CopyFrom(dimStyle16);
                    acDimStyleTblRec.Name = strDimStyleName;
                    int textStyleNum = Convert.ToInt32(TextStyleName);
                    acDimStyleTblRec.Dimscale = textStyleNum;
                    dimStyle16.Dispose();
                    trans.Commit();
                    //UtilMethod.WarningMessageBox("Done", "AutoCAD");
                    StatusCreateDimStyle = DimStyleDone;
                    db.Dimstyle = acDimStyleTblRec.ObjectId;
                }
            }
            catch (Exception ex)
            {
                //UtilMethod.WarningMessageBox($"An error occurs: {ex.Message}", "Error");
                StatusCreateDimStyle = DimStyleFail;
                return;
            }
        }   
        private void CreateNewMultileader()
        {
            Document doc = UtilMethod.AcadDoc();
            Database db = UtilMethod.AcadDb();
            double textStyleNum = Convert.ToInt32(TextStyleName);
            try
            {
                using (var trans = db.TransactionManager.StartTransaction())
                {

                    ObjectId mlSTableId = db.MLeaderStyleDictionaryId;
                    DBDictionary mlSTable = (DBDictionary)trans.GetObject(mlSTableId, OpenMode.ForRead);

                    if (mlSTable.Contains(TextStyleName))
                    {
                        MessageBox.Show("Create multileader fail!", "AutoCAD Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    ObjectId standardMLeaderStyleId = (ObjectId)mlSTable["16"];
                    MLeaderStyle standardMLeaderStyle = (MLeaderStyle)trans.GetObject(standardMLeaderStyleId, OpenMode.ForRead);
                    //add a new mleader style...
                    MLeaderStyle newMleaderStyle = new MLeaderStyle();
                    newMleaderStyle.Name = TextStyleName;

                    newMleaderStyle.ContentType = standardMLeaderStyle.ContentType;
                    newMleaderStyle.ContentType = standardMLeaderStyle.ContentType;
                    newMleaderStyle.EnableDogleg = standardMLeaderStyle.EnableDogleg;
                    newMleaderStyle.LeaderLineColor = standardMLeaderStyle.LeaderLineColor;
                    newMleaderStyle.LeaderLineTypeId = standardMLeaderStyle.LeaderLineTypeId;
                    newMleaderStyle.LeaderLineWeight = standardMLeaderStyle.LeaderLineWeight;
                    newMleaderStyle.LeaderLineType = standardMLeaderStyle.LeaderLineType;
                    newMleaderStyle.TextAlignmentType = standardMLeaderStyle.TextAlignmentType;
                    newMleaderStyle.TextColor = standardMLeaderStyle.TextColor;
                    newMleaderStyle.Scale = standardMLeaderStyle.Scale;

                    ObjectId textStyleTableId = db.TextStyleTableId;
                    TextStyleTable textStyleTable = (TextStyleTable)trans.GetObject(textStyleTableId, OpenMode.ForRead);
                    ObjectId textStyleID = textStyleTable[TextStyleName];
                    newMleaderStyle.TextStyleId = textStyleID;
                    newMleaderStyle.MaxLeaderSegmentsPoints = 5;
                    newMleaderStyle.LandingGap = 1 / 2 * (textStyleNum / 16);
                    newMleaderStyle.ArrowSize = 1.25 * (textStyleNum / 16);
                    newMleaderStyle.ArrowSymbolId = standardMLeaderStyle.ArrowSymbolId;
                    newMleaderStyle.BreakSize = 11 / 16 * (textStyleNum / 16);
                    newMleaderStyle.ContentType = standardMLeaderStyle.ContentType;
                    newMleaderStyle.DoglegLength = 2 * textStyleNum / 16;
                    ObjectId mleaderStyleId = newMleaderStyle.PostMLeaderStyleToDb(db, TextStyleName);
                    mlSTable.UpgradeOpen();
                    trans.AddNewlyCreatedDBObject(newMleaderStyle, true);
                    mleaderStyleId = mlSTable.GetAt(TextStyleName);
                    trans.Commit();
                    MessageBox.Show("Create multileader!", "AutoCAD Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            catch (Exception ex)
            {
                UtilMethod.WarningMessageBox($"An error occurs: {ex.Message}", "Error");
                return;
            }

        }
        private void CreateScaleViewport()
        {
            Document doc = UtilMethod.AcadDoc();
            Database db = UtilMethod.AcadDb();
            double textStyleNum = Convert.ToInt32(TextStyleName);
            try
            {
                ObjectContextManager contextManager = db.ObjectContextManager;
                if (contextManager != null)
                {
                    // now get the Annotation Scaling context collection
                    // (named ACDB_ANNOTATIONSCALES_COLLECTION)                   
                    ObjectContextCollection contextCollection = contextManager.GetContextCollection("ACDB_ANNOTATIONSCALES");
                    // if ok                   
                    if (contextCollection != null)
                    {
                        // create a brand new scale context
                        double numberA = textStyleNum / 12;
                        if (!contextCollection.HasContext($"Scale {textStyleNum}"))
                        {
                            AnnotationScale annotationScale = new AnnotationScale();
                            annotationScale.Name = $"Scale {textStyleNum}";
                            annotationScale.PaperUnits = 1 / numberA;
                            annotationScale.DrawingUnits = 12;
                            // now add to the drawing's context collection                       
                            contextCollection.AddContext(annotationScale);
                            //MessageBox.Show("Create viewport successfully!", "AutoCAD Warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            StatusCreateScaleVP = ScaleViewPortDone;
                            return;
                        }
                        else
                        {
                            StatusCreateScaleVP = ScaleViewPortFail;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //UtilMethod.WarningMessageBox($"An error occurs: {ex.Message}", "Error");
                StatusCreateScaleVP = ScaleViewPortFail;
                return;
            }
        }
        private void CreateAlls()
        {
            if (TextStyleName != "" && !string.IsNullOrEmpty(TextStyleName))
            {
                CreateNewTextStyle();
                CreateNewDimStyle();
                //CreateNewMultileader();
                CreateScaleViewport();
            }
            else
            {
                TextStyleName = "Invalid input";
            }
        }
        private void ExtractBlockToFile()
        {
            {
                Document doc = UtilMethod.AcadDoc();
                Database db = UtilMethod.AcadDb();
                Editor ed = UtilMethod.AcadEd();

                PromptSaveFileOptions sfOptions = new PromptSaveFileOptions("Choose a folder to store block")
                {
                    Filter = "AutoCAD Drawing (*.dwg)|*.dwg",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                };
                PromptResult pr = ed.GetFileNameForSave(sfOptions);

                if (pr.Status != PromptStatus.OK)
                {
                    return;
                }
                string saveFolderPath = Path.GetDirectoryName(pr.StringResult);
                try
                {
                    using (Transaction tr = db.TransactionManager.StartTransaction())
                    {
                        doc.LockDocument();
                        BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                        BlockTableRecord modelSpace = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);

                        int extractedCount = 0;
                        //System.Collections.IList seletedBlockNames = ListDetailItemTest;
                        List<string> selectedBlockNamesms = ListDetailItemTest.Where(item => item.IsSelected == true).Select(item => item.BlockNameTest).ToList(); ;
                        // Duyệt qua tất cả các đối tượng trong Model Space
                        foreach (ObjectId objId in modelSpace)
                        {
                            Entity ent = tr.GetObject(objId, OpenMode.ForWrite) as Entity;
                            if (ent is BlockReference)
                            {
                                BlockReference blkRef = (BlockReference)ent;
                                ObjectId blockDefId = blkRef.BlockTableRecord;
                                BlockTableRecord blkRec = (BlockTableRecord)tr.GetObject(blockDefId, OpenMode.ForRead);

                                // Kiểm tra nếu đây không phải là block động và không phải là block layout hoặc annotation
                                if (!blkRec.IsDynamicBlock && !blkRec.IsLayout && !blkRec.IsAnonymous)
                                {
                                    BlockNameTest = blkRec.Name;
                                    string saveFilePath = Path.Combine(saveFolderPath, $"{BlockNameTest}.dwg");
                                    if (BlockNameTest == selectedBlockNamesms.Contains(BlockNameTest).ToString())
                                    {
                                        if (!File.Exists(saveFilePath)) // Tránh ghi đè nếu file đã tồn tại
                                        {
                                            ExtractBlockToFile(blockDefId, saveFilePath, BlockNameTest);
                                            extractedCount++;
                                        }
                                        else
                                        {
                                            ed.WriteMessage($"\nĐã bỏ qua block '{BlockNameTest}' vì file '{saveFilePath}' đã tồn tại.");
                                        } 
                                    }
                                }
                            }

                        }
                        tr.Commit();
                        ed.WriteMessage($"\nĐã trích xuất thành công {extractedCount} block thành các file .dwg riêng lẻ trong thư mục '{saveFolderPath}'.");
                    }
                }
                catch (Exception ex)
                {
                    ed.WriteMessage("Bug log: " + ex.Message.ToString());
                }
                
            }
        }
        private void ExtractBlockToFile(ObjectId blockDefId, string destinationFilePath, string blockName)
        {
            string dwgVersion = string.Empty;
            //dwgVersion = DwgVersion.Unknown;
            using (Database destDb = new Database(true, false))
            {
                ObjectIdCollection idCollection = new ObjectIdCollection { blockDefId };
                IdMapping idMap = new IdMapping();

                // Sao chép các đối tượng vào không gian hiện tại của database đích
                destDb.WblockCloneObjects(idCollection, destDb.CurrentSpaceId, idMap, DuplicateRecordCloning.Replace, false);
                InsertABlockName(blockName, destDb);
                destDb.SaveAs(destinationFilePath, DwgVersion.Current);
            }
        }
        private void InsertABlockName(string blockName, Database destDb)
        {
            Document doc = UtilMethod.AcadDoc();
            //Database db = UtilMethod.AcadDb();
            Editor ed = UtilMethod.AcadEd();
            //string blockName = BlockName.ToUpper().Trim();
            using (Transaction tr = destDb.TransactionManager.StartTransaction())
            {
                // Lấy BlockTable để kiểm tra xem block đã tồn tại chưa
                BlockTable bt = (BlockTable)tr.GetObject(destDb.BlockTableId, OpenMode.ForRead);

                // Kiểm tra xem block có tồn tại trong BlockTable hay không
                if (bt.Has(blockName))
                {
                    // Lấy ObjectId của BlockTableRecord (định nghĩa block)
                    ObjectId blockDefId = bt[blockName];
                    doc.LockDocument();
                    // Lấy Model Space BlockTableRecord để thêm block reference
                    BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                    // Tạo một điểm chèn tại gốc (0,0,0)
                    Point3d insertionPoint = Point3d.Origin;

                    // Tạo một instance của BlockReference
                    using (BlockReference blkRef = new BlockReference(insertionPoint, blockDefId))
                    {
                        // Thêm BlockReference vào Model Space
                        ms.AppendEntity(blkRef);
                        tr.AddNewlyCreatedDBObject(blkRef, true);                       
                        //ZoomWin(new Point3d(), new Point3d(), new Point3d(), 1.01075);
                        tr.Commit();
                        ed.WriteMessage($"\nĐã chèn block '{blockName}' tại tọa độ (0,0,0).");
                    }
                }
                else
                {
                    ed.WriteMessage($"\nKhông tìm thấy block '{blockName}' trong bản vẽ.");
                }
            }
            //doc.Zoom(new Point3d(), new Point3d(), new Point3d(), 1.01075);

        }
        private void CountBlock()
        {
            Document doc = UtilMethod.AcadDoc();
            Database db = UtilMethod.AcadDb();
            Editor ed = UtilMethod.AcadEd();
            try
            {
                using (var tr = db.TransactionManager.StartOpenCloseTransaction())
                {
                    //ListDetailItem = new ObservableCollection<string>();
                    if (ListDetailItemTest != null)
                    {
                        ListDetailItemTest.Clear();
                        TotalNumberOfBlock = string.Empty;
                    }
                    else
                    {
                        ListDetailItemTest = new ObservableCollection<ChangeFileNameViewModel>();
                    }
                    //ListDetailItemTest = new ObservableCollection<ChangeFileNameViewModel>();
                    var modelSpace = (BlockTableRecord)tr.GetObject(
                        SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead);

                    var brclass = Autodesk.AutoCAD.Runtime.RXObject.GetClass(typeof(BlockReference));

                    var blocks = modelSpace
                        .Cast<ObjectId>()
                        .Where(id => id.ObjectClass == brclass)
                        .Select(id => (BlockReference)tr.GetObject(id, OpenMode.ForRead))
                        .GroupBy(br => ((BlockTableRecord)tr.GetObject(
                            br.DynamicBlockTableRecord, OpenMode.ForRead)).Name);

                    int totalBlockCount = 0;
                    foreach (var group in blocks)
                    {
                        if (group.Key.ToString() != "Model" && !group.Key.StartsWith("*Layout") && group.Key.StartsWith(BlockNameStartWith))
                        {
                            //ListDetailItem.Add(group.Key.ToString());
                            //totalBlockCount += group.Count();
                            //ed.WriteMessage($"\n{group.Key}: {group.Count()}");
                            ListDetailItemTest.Add(new ChangeFileNameViewModel(group.Key.ToString()));
                            totalBlockCount += group.Count();
                            ed.WriteMessage($"\n{group.Key}: {group.Count()}");
                        }
                    }
                    TotalNumberOfBlock = $"There are total: {totalBlockCount} Block{(totalBlockCount > 1 ? "s" : "")}";
                    tr.Commit();
                }
            }
            catch (Exception ex)
            {
                UtilMethod.WarningMessageBox($"Error {ex.Message}", "Error");
                return;
            }          
        }
        private void SeletedDetail()
        {
            if (IsSelected == true && BlockNameTest != null)
            {
                ListDetailItem.Add(BlockNameTest);
            }
            else if (IsSelected == false && BlockNameTest != null && ListDetailItem.Contains(BlockNameTest))
            {
                ListDetailItem.Remove(BlockNameTest);
            }
        }
    }

}

