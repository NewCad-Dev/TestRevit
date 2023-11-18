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
    public class SelectFoundation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            /*IList<Element> elF = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                .WhereElementIsNotElementType()
                .ToElements();*/

            IList<FamilyInstance> fInstans = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                .WhereElementIsNotElementType()
                .OfType<FamilyInstance>()
                .ToList();

            IList<Floor> floorList = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralFoundation)
                .WhereElementIsNotElementType()
                .OfType<Floor>()
                .ToList();

            string foundation = "";

            foreach (Element el in floorList)
            {
                foundation += el.Name + " / ";
            }

            TaskDialog.Show("foundation", foundation);

            return Result.Succeeded;
        }
    }
}
