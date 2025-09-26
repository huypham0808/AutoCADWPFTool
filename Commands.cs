using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using ChangeFileName.ViewModels;
using ChangeFileName.Views;
using System;
using AcAp = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(ChangeFileName.Commands))]
namespace ChangeFileName
{
    public class Commands
    {
        [CommandMethod("CSS_UtilitiesTool", CommandFlags.Modal)]
        public void CallForm()
        {
            ChangeFileNameViewModel vM = new ChangeFileNameViewModel();

            FileNameWindow fileNameWindow = new FileNameWindow
            {
                DataContext = vM
            };
           AcAp.ShowModelessWindow(fileNameWindow);
        }
        [CommandMethod("AutoRoomHatch", CommandFlags.Modal)]
        public void DetectRoomByPick()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Database db = doc.Database;
            Editor ed = doc.Editor;

            // Ask user to pick an internal point inside each room boundary
            PromptPointOptions ppo = new PromptPointOptions("\nPick a point inside a room boundary: ");
            PromptPointResult ppr = ed.GetPoint(ppo);

            if (ppr.Status != PromptStatus.OK)
            {
                ed.WriteMessage("\nNo point selected.");
                return;
            }

            using (Transaction tr = db.TransactionManager.StartTransaction())
            {
                BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
                BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

                // AutoCAD will trace the boundary from user-picked point
                Point3d pickPoint = ppr.Value;
                DBObjectCollection loops = ed.TraceBoundary(pickPoint, true);

                if (loops == null || loops.Count == 0)
                {
                    ed.WriteMessage("\nNo closed boundary found.");
                    return;
                }

                Random rand = new Random();

                foreach (Entity loop in loops)
                {
                    if (loop is Polyline pline && pline.Closed)
                    {
                        // Create hatch
                        Hatch hatch = new Hatch();
                        btr.AppendEntity(hatch);
                        tr.AddNewlyCreatedDBObject(hatch, true);

                        hatch.SetHatchPattern(HatchPatternType.PreDefined, "SOLID");

                        // Random Color
                        int r = rand.Next(0, 256);
                        int g = rand.Next(0, 256);
                        int b = rand.Next(0, 256);
                        hatch.Color = Color.FromRgb((byte)r, (byte)g, (byte)b);

                        // Append loop
                        ObjectId plId = btr.AppendEntity(pline);
                        tr.AddNewlyCreatedDBObject(pline, true);

                        ObjectIdCollection loopIds = new ObjectIdCollection { plId };
                        hatch.AppendLoop(HatchLoopTypes.Outermost, loopIds);

                        hatch.EvaluateHatch(true);
                    }
                }

                tr.Commit();
            }

            ed.Regen();
            ed.WriteMessage("\nHatch created successfully.");
        }
    }    
}
