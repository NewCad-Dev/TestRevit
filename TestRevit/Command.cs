using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace TestRevit
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            Element element = doc.GetElement(uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "Pick test subject").ElementId);

            Parameter par = element.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM);
            Parameter par1 = element.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);

            double d = UnitUtils.Convert(par.AsDouble(), UnitTypeId.Feet, UnitTypeId.Millimeters);
            double d1 = UnitUtils.Convert(par1.AsDouble(), UnitTypeId.Feet, UnitTypeId.Millimeters);

            string str = $"Base Ofset - {d} mm, Top Ofset - {d1} mm";

            TaskDialog.Show("Column", str);

            return Result.Succeeded;
        }
    }
}
