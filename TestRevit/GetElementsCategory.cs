using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Xml.Linq;
using Autodesk.Revit.Creation;

namespace TestRevit
{
    [Transaction(TransactionMode.Manual)]
    public class GetElementsCategory : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.DB.Document doc = uiDoc.Document;

            IList<ElementId> ids = (IList<ElementId>)new FilteredElementCollector(doc, doc.ActiveView.Id)
                .ToElementIds();

            string str = "";

            List<string> str1 = new List<string>();

            foreach (ElementId id in ids)
            {
                Element element = doc.GetElement(id);
                Category category = element.Category;

                if (category != null)
                {
                    str1.Add(category.Name + " - " + element.Name);
                }
            }

            //List<string> str2 = str1.Distinct().ToList();
            str1.Sort();

            foreach(string s in str1)
            {
                str += s + "\n";
            }

            TaskDialog.Show("Revit", str);

            /*Element selectedElement = null;
            foreach (ElementId id in uiDoc.Selection.GetElementIds())
            {
                selectedElement = doc.GetElement(id);
                break;
            }
                        
            Category category = selectedElement.Category;

            string prompt = "";
             prompt += "\n\tName:\t" + category.Name;

            TaskDialog.Show("Revit", prompt);*/

            return Result.Succeeded;
        }
    }
}
