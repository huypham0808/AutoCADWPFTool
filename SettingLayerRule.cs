using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(LayerRule.LayerRule))]
namespace LayerRule
{
    public class LayerRule: IExtensionApplication
    {
        public void Initialize()
        {
            // Code khởi tạo khi tải ứng dụng
            //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nFRPDrawer loaded. Type FRPDRAWER to start.");
            Application.DocumentManager.MdiActiveDocument.CommandWillStart += OnCommandWillStart;
            MessageBox.Show("LayerRule loaded. Layer will be set automatically based on command.");
        }
        public void Terminate()
        {
            // Code dọn dẹp khi ứng dụng bị gỡ bỏ
            Application.DocumentManager.MdiActiveDocument.CommandWillStart -= OnCommandWillStart;
        }
        private void OnCommandWillStart(object sender, CommandEventArgs e)
        {
            string cmd = e.GlobalCommandName.ToUpper();

            // Editor để debug
            var ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage($"\nCommand started: {cmd}");

            string targetLayer = null;

            switch (cmd)
            {
                case "DLI":      
                case "DIMLINEAR":
                    targetLayer = "STN-DIM-2";
                    break;
                case "LINE":
                case "L":
                    targetLayer = "STN-EXST-THK";
                    break;
                case "PLINE":
                case "PL":
                    targetLayer = "STN-EXST-THK";
                    break;
                case "XLINE":
                case "XL":
                    targetLayer = "STN-HDN-L";
                    break;
                case "CIRCLE":
                case "C":
                    targetLayer = "STN-HDN-L";
                    break;
                case "RECTANG":
                case "RECTANGLE":
                    targetLayer = "STN-EXST-THK";
                    break;
            }

            if (!string.IsNullOrEmpty(targetLayer))
            {
                SetCurrentLayer(targetLayer);
            }
        }
        private void SetCurrentLayer(string layerName)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;

            using (var tr = db.TransactionManager.StartTransaction())
            {
                var lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

                // nếu chưa có thì tạo layer mới
                if (!lt.Has(layerName))
                {
                    lt.UpgradeOpen();
                    var ltr = new LayerTableRecord { Name = layerName };
                    lt.Add(ltr);
                    tr.AddNewlyCreatedDBObject(ltr, true);
                }

                db.Clayer = lt[layerName];
                tr.Commit();
            }
        }
    }
}
