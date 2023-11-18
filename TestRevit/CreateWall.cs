using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using System.Windows;

namespace TestRevit
{
    [Transaction(TransactionMode.Manual)]
    public class CreateWall : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            /*IList<ElementId> cad = new FilteredElementCollector(doc)
                .OfClass(typeof(ImageInstance))
                .WhereElementIsNotElementType()
                .ToElementIds() as IList<ElementId>;*/
            ElementId sel = uiDoc.Selection.PickObject(ObjectType.Element).ElementId;

            IList<PolyLine> layar = new List<PolyLine>();
            IList<Line> lines = new List<Line>();
            IList<Arc> arc = new List<Arc>();
            
            ImportInstance import = doc.GetElement(sel) as ImportInstance;
            GeometryElement geoElement = import.get_Geometry(new Options());

            foreach(GeometryObject geoObj in geoElement)
            {
                GeometryInstance geoInst = geoObj as GeometryInstance;
                GeometryElement geoElem = geoInst.GetInstanceGeometry();

                if (geoElem != null)
                {
                    foreach (GeometryObject geoObj2 in geoElem)
                    {
                        if (geoObj2 is PolyLine)
                        {
                            layar.Add(geoObj2 as PolyLine);
                        }
                        if (geoObj2 is Line)
                        {
                            lines.Add(geoObj2 as Line);
                        }
                        if(geoObj2 is Arc)
                        {
                            arc.Add(geoObj2 as Arc);
                        }
                    }
                }
            }

            IList<Curve> points = new List<Curve>();
            IList<Curve> pointsLine = new List<Curve>();
            IList<Curve> pointsArc = new List<Curve>();

            foreach (PolyLine polyLine in layar)
            {
                GraphicsStyle gStyle = doc.GetElement(polyLine.GraphicsStyleId) as GraphicsStyle;
                string layer = gStyle.GraphicsStyleCategory.Name;

                if (layer == "Wall")
                {
                    IList<XYZ> p = polyLine.GetCoordinates();
                    XYZ pNew = null;

                    for(int i = 1; i < p.Count; i++)
                    {
                        if (pNew == null)
                        {
                            pNew = p[i-1];
                        }
                        Line line = Line.CreateBound(pNew, p[i]);
                        points.Add(line);
                        pNew = p[i];
                    }                    
                }
            }

            foreach(Line line1 in lines)
            {
                GraphicsStyle gStyle = doc.GetElement(line1.GraphicsStyleId) as GraphicsStyle;
                string layer = gStyle.GraphicsStyleCategory.Name;

                if (layer == "Wall")
                {
                    IList<XYZ> p = line1.Tessellate();
                    XYZ pNew = null;

                    for (int i = 1; i < p.Count; i++)
                    {
                        if (pNew == null)
                        {
                            pNew = p[i - 1];
                        }
                        Line line = Line.CreateBound(pNew, p[i]);
                        pointsLine.Add(line);
                        pNew = p[i];
                    }
                }
            }

            foreach (Arc arc1 in arc)
            {
                GraphicsStyle gStyle = doc.GetElement(arc1.GraphicsStyleId) as GraphicsStyle;
                string layer = gStyle.GraphicsStyleCategory.Name;

                if (layer == "Wall")
                {
                    double d = arc1.Radius - Math.Sqrt(Math.Pow(arc1.Radius, 2) - Math.Pow((arc1.GetEndPoint(1) - arc1.GetEndPoint(0)).GetLength() / 2, 2));
                    XYZ midPointOfChord = (arc1.GetEndPoint(0) + arc1.GetEndPoint(1)) / 2.0;

                    // TODO: Check Formula
                    XYZ midPointOfArc = midPointOfChord + Transform.CreateRotation(XYZ.BasisZ, Math.PI / 2.0).OfVector((arc1.GetEndPoint(0) - arc1.GetEndPoint(1)).Normalize().Multiply(d));
                                     
                    Arc arc2 = Arc.Create(arc1.GetEndPoint(0), arc1.GetEndPoint(1), midPointOfArc);

                    pointsArc.Add(arc2);
                }
            }

            IList<Element> level = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .WhereElementIsNotElementType()
                .ToElements();

            ElementId id = null;

            foreach (Element levelElem in level)
            {
                if (levelElem.Name == "Level 1")
                {
                    id = levelElem.Id;
                }
            }

            using (Transaction tr = new Transaction(doc, "Create Wall"))
            {
                tr.Start();

                foreach (Curve curve in points)
                {
                    Wall.Create(doc, curve, id, false);
                }

                foreach (Curve curve in pointsLine)
                {
                    Wall.Create(doc, curve, id, false);
                }

                foreach (Curve curve in pointsArc)
                {
                    Wall.Create(doc, curve, id, false);
                }

                tr.Commit();
            }

            return Result.Succeeded;
        }
    }
}
